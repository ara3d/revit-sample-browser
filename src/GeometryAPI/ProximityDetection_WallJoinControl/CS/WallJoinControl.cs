// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace RevitMultiSample.ProximityDetection_WallJoinControl.CS
{
    /// <summary>
    ///     The class that is responsible for controlling the joint of walls
    /// </summary>
    public class WallJoinControl
    {
        /// <summary>
        ///     The singleton instance of WallJoinControl
        /// </summary>
        private static WallJoinControl _instance;

        /// <summary>
        ///     revit application
        /// </summary>
        private Application m_app;

        /// <summary>
        ///     revit document
        /// </summary>
        private Document m_doc;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <param name="doc">Revit document</param>
        private WallJoinControl(
            Application app,
            Document doc)
        {
            m_app = app;
            m_doc = doc;
        }

        /// <summary>
        ///     Get the singleton instance of WallJoinControl
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <param name="doc">Revit document</param>
        /// <returns>The singleton instance of WallJoinControl</returns>
        public static WallJoinControl GetInstance(
            Application app,
            Document doc)
        {
            return _instance ?? (_instance = new WallJoinControl(app, doc));
        }

        /// <summary>
        ///     Check every wall's joined walls by WallUtils method in initial, after disallow join, then after allow join states
        /// </summary>
        /// <param name="walls">The walls to be checked</param>
        /// <returns>The check result</returns>
        public XElement CheckJoinedWalls(IEnumerable<Wall> walls)
        {
            // create a node that place all walls.
            var wallsNode = new XElement("Walls", new XAttribute("Name", "Walls"));

            try
            {
                foreach (var wall in walls)
                {
                    // create a wall node
                    var wallNode = new XElement("Wall",
                        new XAttribute("name", wall.Name),
                        new XAttribute("Type", wall.WallType.Kind.ToString()));

                    if (!(wall.Location is LocationCurve locationCurve))
                    {
                        wallNode.Add(new XElement("Error",
                            new XAttribute("Exception", "This wall has not a LocationCurve!")));
                    }
                    else
                    {
                        // start
                        var endNode = new XElement("Start", new XAttribute("Name", "Start"));
                        wallNode.Add(CheckWallEnd(wall, locationCurve, 0, endNode));

                        // end
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

            // return the whole walls Node
            return wallsNode;
        }

        /// <summary>
        ///     Check wall's two ends in three states
        /// </summary>
        /// <param name="wall">The wall to be checked</param>
        /// <param name="locationCurve">The wall's location curve</param>
        /// <param name="end">The index indicates the start or end of this wall</param>
        /// <param name="endnode">Result XML node</param>
        /// <returns>The check result</returns>
        private XElement CheckWallEnd(Wall wall, LocationCurve locationCurve, int end, XElement endnode)
        {
            // Initial state
            var stateNode = new XElement("Initial", new XAttribute("Name", "Initial"));
            endnode.Add(GetState(wall, locationCurve, end, stateNode));

            // Disallow join
            WallUtils.DisallowWallJoinAtEnd(wall, end);

            // After DisallowWallJoinAtEnd Evoked state
            stateNode = new XElement("After_DisallowWallJoinAtEnd_Evoked",
                new XAttribute("Name", "After_DisallowWallJoinAtEnd_Evoked"));
            endnode.Add(GetState(wall, locationCurve, end, stateNode));

            // Allow join
            WallUtils.AllowWallJoinAtEnd(wall, end);

            // After AllowWallJoinAtEnd Evoked state
            stateNode = new XElement("After_DisallowWallJoinAtEnd_Evoked",
                new XAttribute("Name", "After_DisallowWallJoinAtEnd_Evoked"));
            endnode.Add(GetState(wall, locationCurve, end, stateNode));

            return endnode;
        }

        /// <summary>
        ///     Get wall's one end state
        /// </summary>
        /// <param name="wall">The wall to be checked</param>
        /// <param name="locationCurve">The wall's location curve</param>
        /// <param name="end">The index indicates the start or end of this wall</param>
        /// <param name="statenode">Result XML node</param>
        /// <returns>The check result</returns>
        private XElement GetState(Wall wall, LocationCurve locationCurve, int end, XElement statenode)
        {
            // Get geometry information
            statenode.Add(GetGeometryInfo(locationCurve, end));
            // Get IsWallJoinAllowedAtEnd API method's result
            statenode.Add(GetIsWallJoinAllowedAtEnd(wall, end));
            // Get all joined walls
            statenode.Add(GetJoinedWalls(locationCurve, end));

            return statenode;
        }

        /// <summary>
        ///     Get wall's one end geometry information
        /// </summary>
        /// <param name="locationCurve">The wall's location curve</param>
        /// <param name="end">The index indicates the start or end of this wall</param>
        /// <returns>The check result</returns>
        private XElement GetGeometryInfo(LocationCurve locationCurve, int end)
        {
            var geometryinfoNode = new XElement("GeometryInfo");
            // Output LocationCurve end point coordinate
            var endpoint = locationCurve.Curve.GetEndPoint(end);
            geometryinfoNode.Add(new XElement("PointCoordinate",
                new XAttribute("X", Math.Round(endpoint.X, 9).ToString()),
                new XAttribute("Y", Math.Round(endpoint.Y, 9).ToString()),
                new XAttribute("Z", Math.Round(endpoint.Z, 9).ToString())));

            return geometryinfoNode;
        }

        /// <summary>
        ///     Get wall's one end IsWallJoinAllowedAtEnd API method's result
        /// </summary>
        /// <param name="wall">The wall to be checked</param>
        /// <param name="end">The index indicates the start or end of this wall</param>
        /// <returns>The check result</returns>
        private XElement GetIsWallJoinAllowedAtEnd(Wall wall, int end)
        {
            return new XElement("IsWallJoinAllowedAtEnd",
                new XAttribute("Value", WallUtils.IsWallJoinAllowedAtEnd(wall, end)));
        }

        /// <summary>
        ///     Get wall's one end all joined walls
        /// </summary>
        /// <param name="locationCurve">The wall's location curve</param>
        /// <param name="end">The index indicates the start or end of this wall</param>
        /// <returns>The check result</returns>
        private XElement GetJoinedWalls(LocationCurve locationCurve, int end)
        {
            // retrieve joined elements 
            var array = locationCurve.get_ElementsAtJoin(end);

            var joinedwallsNode = new XElement("JoinedWalls",
                new XAttribute("Count", array.Size.ToString()));

            // output array
            foreach (Element ele in array)
                if (ele is Wall)
                    joinedwallsNode.Add(new XElement("JoinedWall",
                        new XAttribute("Name", ele.Name)));

            return joinedwallsNode;
        }
    }
}
