// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ProximityDetection_WallJoinControl.CS
{
    public class ProximityDetection
    {
        private static ProximityDetection _instance;
        private Application m_app;
        private readonly Document m_doc;

        private ProximityDetection(
            Application app,
            Document doc)
        {
            m_app = app;
            m_doc = doc;
        }

        public static ProximityDetection GetInstance(
            Application app,
            Document doc)
        {
            return _instance ?? (_instance = new ProximityDetection(app, doc));
        }

        public XElement FindColumnsInWall(IEnumerable<Wall> walls)
        {
            var wallsNode = new XElement("Walls", new XAttribute("Name", "Walls"));

            try
            {
                foreach (var wall in walls)
                {
                    var wallNode = new XElement("Wall", new XAttribute("Name", wall.Name));

                    var collector = new FilteredElementCollector(m_doc);
                    var columnCategories = new List<BuiltInCategory>
                    {
                        BuiltInCategory.OST_Columns,
                        BuiltInCategory.OST_StructuralColumns
                    };
                    collector.WherePasses(new ElementMulticategoryFilter(columnCategories));

                    collector.WherePasses(new ElementIntersectsElementFilter(wall));

                    var columnsNode = new XElement("columns",
                        new XAttribute("Count", collector.Count().ToString()));

                    foreach (var column in collector)
                    {
                        columnsNode.Add(new XElement("column", new XAttribute("Name", column.Name)));
                    }

                    wallNode.Add(columnsNode);
                    wallsNode.Add(wallNode);
                }
            }
            catch (Exception ex)
            {
                wallsNode.Add(new XElement("Error", new XAttribute("Exception", ex.ToString())));
            }

            return wallsNode;
        }

        public XElement FindBlockingElements(ICollection<Element> egresses)
        {
            var egressesNode = new XElement("Egresses", new XAttribute("Name", "Egresses"));

            try
            {
                foreach (var egressElement in egresses)
                {
                    var egressNode = new XElement("Egress",
                        new XAttribute("Name", egressElement.Name));

                    var count = 1;
                    // Assumes door geometry is a single GeometryInstance with solids in instance geometry.
                    var objects = egressElement.get_Geometry(new Options()).GetEnumerator();
                    objects.MoveNext();
                    var gi = objects.Current as GeometryInstance;
                    var objects1 = gi.GetInstanceGeometry().GetEnumerator();

                    while (objects1.MoveNext())
                    {
                        var egressGObj = objects1.Current;

                        if (egressGObj is Solid egressVolume)
                        {
                            var solidNode = new XElement($"ElementSolid{count}");
                            var blockingcollector = new FilteredElementCollector(m_doc);
                            blockingcollector.WhereElementIsNotElementType();

                            blockingcollector.WherePasses(new ElementIntersectsSolidFilter(egressVolume));

                            IEnumerable<Element> blockingElement = blockingcollector;

                            var exclusions = new List<ElementId> { egressElement.Id };
                            blockingcollector.Excluding(exclusions);

                            var blockingegressNode = new XElement("blocking_egress_elements",
                                new XAttribute("Count", blockingElement.Count().ToString()));

                            foreach (var blockingelement in blockingElement)
                            {
                                blockingegressNode.Add(new XElement("blocking_egress_element",
                                    new XAttribute("Name", blockingelement.Name)));
                            }

                            solidNode.Add(blockingegressNode);
                            egressNode.Add(solidNode);

                            count++;
                        }
                    }

                    egressesNode.Add(egressNode);
                }
            }
            catch (Exception ex)
            {
                egressesNode.Add(new XElement("Error", new XAttribute("Exception", ex.ToString())));
            }

            return egressesNode;
        }

        public XElement FindNearbyWalls(IEnumerable<Wall> walls)
        {
            var wallsNode = new XElement("Walls", new XAttribute("Name", "Walls"));

            try
            {
                foreach (var wall in walls)
                {
                    var wallNode = new XElement("Wall", new XAttribute("Name", wall.Name));

                    var endNode = new XElement("Start", new XAttribute("Name", "Start"));

                    var wallEndPoint = (wall.Location as LocationCurve).Curve.GetEndPoint(0);
                    var wallHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

                    var collector = NearbyWallsFilter(wallEndPoint, wallHeight, 10.0); // 10 ft

                    var exclusions = new List<ElementId> { wall.Id };
                    collector.Excluding(exclusions);

                    var nearbyWalls = collector.OfType<Wall>();

                    var nearbyWallsNode = new XElement("near_by_walls",
                        new XAttribute("Count", nearbyWalls.Count().ToString()));

                    foreach (var nearbywall in nearbyWalls)
                    {
                        nearbyWallsNode.Add(new XElement("near_by_wall",
                            new XAttribute("Name", nearbywall.Name)));
                    }

                    endNode.Add(nearbyWallsNode);
                    wallNode.Add(endNode);

                    endNode = new XElement("End", new XAttribute("Name", "End"));

                    wallEndPoint = (wall.Location as LocationCurve).Curve.GetEndPoint(1);

                    collector = NearbyWallsFilter(wallEndPoint, wallHeight, 10.0);

                    exclusions = new List<ElementId> { wall.Id };
                    collector.Excluding(exclusions);

                    nearbyWalls = collector.OfType<Wall>();

                    nearbyWallsNode = new XElement("near_by_walls",
                        new XAttribute("Count", nearbyWalls.Count().ToString()));

                    foreach (var nearbywall in nearbyWalls)
                    {
                        nearbyWallsNode.Add(new XElement("near_by_wall",
                            new XAttribute("Name", nearbywall.Name)));
                    }

                    endNode.Add(nearbyWallsNode);
                    wallNode.Add(endNode);

                    wallsNode.Add(wallNode);
                }
            }
            catch (Exception ex)
            {
                wallsNode.Add(new XElement("Error", new XAttribute("Exception", ex.ToString())));
            }

            return wallsNode;
        }

        private FilteredElementCollector NearbyWallsFilter(XYZ point, double height, double radius)
        {
            var curveloops = new List<CurveLoop>();
            var circle = new CurveLoop();
            circle.Append(Arc.Create(point, radius
                , 0, Math.PI,
                XYZ.BasisX, XYZ.BasisY));
            circle.Append(Arc.Create(point, radius
                , Math.PI, 2 * Math.PI,
                XYZ.BasisX, XYZ.BasisY));
            curveloops.Add(circle);

            var wallEndCylinder =
                GeometryCreationUtilities.CreateExtrusionGeometry(curveloops, XYZ.BasisZ, height);

            var collector = new FilteredElementCollector(m_doc);
            collector.OfCategory(BuiltInCategory.OST_Walls);

            collector.WherePasses(new ElementIntersectsSolidFilter(wallEndCylinder));

            return collector;
        }
    }
}
