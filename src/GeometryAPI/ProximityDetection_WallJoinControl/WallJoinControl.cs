// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ProximityDetection_WallJoinControl.CS
{
    public class WallJoinControl
    {
        private static WallJoinControl _instance;
        private readonly Application m_app;
        private readonly Document m_doc;

        private WallJoinControl(
            Application app,
            Document doc)
        {
            m_app = app;
            m_doc = doc;
        }

        public static WallJoinControl GetInstance(
            Application app,
            Document doc)
        {
            return _instance ??= new WallJoinControl(app, doc);
        }

        public XElement CheckJoinedWalls(IEnumerable<Wall> walls)
        {
            XElement wallsNode = new("Walls", new XAttribute("Name", "Walls"));

            try
            {
                foreach (var wall in walls)
                {
                    XElement wallNode = new("Wall",
                        new XAttribute("name", wall.Name),
                        new XAttribute("Type", wall.WallType.Kind.ToString()));

                    if (wall.Location is not LocationCurve locationCurve)
                    {
                        wallNode.Add(new XElement("Error",
                            new XAttribute("Exception", "This wall has not a LocationCurve!")));
                    }
                    else
                    {
                        XElement endNode = new("Start", new XAttribute("Name", "Start"));
                        wallNode.Add(CheckWallEnd(wall, locationCurve, 0, endNode));

                        endNode = new XElement("End", new XAttribute("Name", "End"));
                        wallNode.Add(CheckWallEnd(wall, locationCurve, 1, endNode));
                    }

                    wallsNode.Add(wallNode);
                }
            }
            catch (Exception ex)
            {
                wallsNode.Add(new XElement("Error", new XAttribute("Exception", ex.ToString())));
            }

            return wallsNode;
        }

        private XElement CheckWallEnd(Wall wall, LocationCurve locationCurve, int end, XElement endnode)
        {
            XElement stateNode = new("Initial", new XAttribute("Name", "Initial"));
            endnode.Add(GetState(wall, locationCurve, end, stateNode));

            WallUtils.DisallowWallJoinAtEnd(wall, end);

            stateNode = new XElement("After_DisallowWallJoinAtEnd_Evoked",
                new XAttribute("Name", "After_DisallowWallJoinAtEnd_Evoked"));
            endnode.Add(GetState(wall, locationCurve, end, stateNode));

            WallUtils.AllowWallJoinAtEnd(wall, end);

            stateNode = new XElement("After_DisallowWallJoinAtEnd_Evoked",
                new XAttribute("Name", "After_DisallowWallJoinAtEnd_Evoked"));
            endnode.Add(GetState(wall, locationCurve, end, stateNode));

            return endnode;
        }

        private XElement GetState(Wall wall, LocationCurve locationCurve, int end, XElement statenode)
        {
            statenode.Add(GetGeometryInfo(locationCurve, end));
            statenode.Add(GetIsWallJoinAllowedAtEnd(wall, end));
            statenode.Add(GetJoinedWalls(locationCurve, end));

            return statenode;
        }

        private XElement GetGeometryInfo(LocationCurve locationCurve, int end)
        {
            XElement geometryinfoNode = new("GeometryInfo");
            var endpoint = locationCurve.Curve.GetEndPoint(end);
            geometryinfoNode.Add(new XElement("PointCoordinate",
                new XAttribute("X", Math.Round(endpoint.X, 9).ToString()),
                new XAttribute("Y", Math.Round(endpoint.Y, 9).ToString()),
                new XAttribute("Z", Math.Round(endpoint.Z, 9).ToString())));

            return geometryinfoNode;
        }

        private XElement GetIsWallJoinAllowedAtEnd(Wall wall, int end)
        {
            return new XElement("IsWallJoinAllowedAtEnd",
                new XAttribute("Value", WallUtils.IsWallJoinAllowedAtEnd(wall, end)));
        }

        private XElement GetJoinedWalls(LocationCurve locationCurve, int end)
        {
            var array = locationCurve.get_ElementsAtJoin(end);

            XElement joinedwallsNode = new("JoinedWalls",
                new XAttribute("Count", array.Size.ToString()));

            foreach (Element ele in array)
            {
                if (ele is Wall)
                    joinedwallsNode.Add(new XElement("JoinedWall",
                        new XAttribute("Name", ele.Name)));
            }

            return joinedwallsNode;
        }
    }
}
