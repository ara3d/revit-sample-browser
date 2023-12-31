// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.ProximityDetection_WallJoinControl.CS
{
    /// <summary>
    ///     The class that is responsible for proximity detection
    /// </summary>
    public class ProximityDetection
    {
        /// <summary>
        ///     The singleton instance of ProximityDetection
        /// </summary>
        private static ProximityDetection _instance;

        /// <summary>
        ///     revit application
        /// </summary>
        private Application m_app;

        /// <summary>
        ///     revit document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <param name="doc">Revit document</param>
        private ProximityDetection(
            Application app,
            Document doc)
        {
            m_app = app;
            m_doc = doc;
        }

        /// <summary>
        ///     Get the singleton instance of ProximityDetection
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <param name="doc">Revit document</param>
        /// <returns>The singleton instance of ProximityDetection</returns>
        public static ProximityDetection GetInstance(
            Application app,
            Document doc)
        {
            return _instance ?? (_instance = new ProximityDetection(app, doc));
        }

        /// <summary>
        ///     Find columns in wall
        /// </summary>
        /// <param name="walls">The walls to be detected</param>
        /// <returns>The detection result</returns>
        public XElement FindColumnsInWall(IEnumerable<Wall> walls)
        {
            // create a node that place all walls.
            var wallsNode = new XElement("Walls", new XAttribute("Name", "Walls"));

            try
            {
                foreach (var wall in walls)
                {
                    var wallNode = new XElement("Wall", new XAttribute("Name", wall.Name));

                    // Iterate to find columns and structural columns
                    var collector = new FilteredElementCollector(m_doc);
                    var columnCategories = new List<BuiltInCategory>
                    {
                        BuiltInCategory.OST_Columns,
                        BuiltInCategory.OST_StructuralColumns
                    };
                    collector.WherePasses(new ElementMulticategoryFilter(columnCategories));

                    // Apply element intersection filter
                    var testElementIntersectsElementFilter =
                        new ElementIntersectsElementFilter(wall);

                    collector.WherePasses(testElementIntersectsElementFilter);

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

            // return the whole walls Node
            return wallsNode;
        }

        /// <summary>
        ///     Find elements blocking egress
        /// </summary>
        /// <param name="egresses">The egresses to be detected</param>
        /// <returns>The detection result</returns>
        public XElement FindBlockingElements(ICollection<Element> egresses)
        {
            // create a node that place all egresses.
            var egressesNode = new XElement("Egresses", new XAttribute("Name", "Egresses"));

            try
            {
                // find the elements blocking egress
                foreach (var egressElement in egresses)
                {
                    var egressNode = new XElement("Egress",
                        new XAttribute("Name", egressElement.Name));

                    var count = 1;
                    var objects = egressElement.get_Geometry(new Options()).GetEnumerator();
                    objects.MoveNext();
                    var gi = objects.Current as GeometryInstance;
                    var objects1 = gi.GetInstanceGeometry().GetEnumerator();

                    //foreach (GeometryObject egressGObj in 
                    //   (egressElement.get_Geometry(new Autodesk.Revit.DB.Options()).Objects.get_Item(0) as GeometryInstance).GetInstanceGeometry().Objects)
                    while (objects1.MoveNext())
                    {
                        var egressGObj = objects1.Current;

                        if (egressGObj is Solid egressVolume)
                        {
                            var solidNode = new XElement($"ElementSolid{count}");
                            // Iterate to find all instance types
                            var blockingcollector = new FilteredElementCollector(m_doc);
                            blockingcollector.WhereElementIsNotElementType();

                            // Apply geometric filter
                            var testElementIntersectsSolidFilter =
                                new ElementIntersectsSolidFilter(egressVolume);
                            blockingcollector.WherePasses(testElementIntersectsSolidFilter);

                            IEnumerable<Element> blockingElement = blockingcollector;

                            // Exclude the door itself  
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

            // return the whole Egresses Node
            return egressesNode;
        }

        /// <summary>
        ///     Find walls (nearly joined to) end of walls
        /// </summary>
        /// <param name="walls">The walls to be detected</param>
        /// <returns>The detection result</returns>
        public XElement FindNearbyWalls(IEnumerable<Wall> walls)
        {
            // create a node that place all walls.
            var wallsNode = new XElement("Walls", new XAttribute("Name", "Walls"));

            try
            {
                foreach (var wall in walls)
                {
                    var wallNode = new XElement("Wall", new XAttribute("Name", wall.Name));

                    // Start
                    var endNode = new XElement("Start", new XAttribute("Name", "Start"));

                    var wallEndPoint = (wall.Location as LocationCurve).Curve.GetEndPoint(0);
                    var wallHeight = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

                    var collector = NearbyWallsFilter(wallEndPoint, wallHeight, 10.0); // 10 ft

                    // Exclude the wall itself
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

                    // End
                    endNode = new XElement("End", new XAttribute("Name", "End"));

                    wallEndPoint = (wall.Location as LocationCurve).Curve.GetEndPoint(1);

                    collector = NearbyWallsFilter(wallEndPoint, wallHeight, 10.0);

                    // Exclude the wall itself
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

            // return the whole walls Node
            return wallsNode;
        }

        /// <summary>
        ///     Find the nearby walls on specific point and in specific height
        /// </summary>
        /// <param name="point">The given point</param>
        /// <param name="height">The given height</param>
        /// <param name="radius">The radius in which walls can be detected</param>
        /// <returns>The detection result</returns>
        private FilteredElementCollector NearbyWallsFilter(XYZ point, double height, double radius)
        {
            // build cylindrical shape around wall endpoint
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

            // Iterate document to find walls
            var collector = new FilteredElementCollector(m_doc);
            collector.OfCategory(BuiltInCategory.OST_Walls);

            // Apply geometric filter
            var testElementIntersectsSolidFilter =
                new ElementIntersectsSolidFilter(wallEndCylinder);
            collector.WherePasses(testElementIntersectsSolidFilter);

            return collector;
        }
    }
}
