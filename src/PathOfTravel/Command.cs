// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PathOfTravel.CS
{
    public enum PathCreateOptions
    {
        SingleRoomCornersToSingleDoor,

        AllRoomCenterToSingleDoor,

        AllRoomCornersToAllDoors
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                if (uiDoc.ActiveView is not ViewPlan viewPlan)
                {
                    TaskDialog td = new("Cannot create PathOfTravel.")
                    {
                        MainInstruction = "PathOfTravel can only be created for plan views."
                    };

                    td.Show();

                    return Result.Succeeded;
                }

                using CreateForm createForm = new();
                if (DialogResult.OK == createForm.ShowDialog())
                {
                    switch (createForm.PathCreateOption)
                    {
                        case PathCreateOptions.SingleRoomCornersToSingleDoor:
                            CreatePathsOfTravelInOneRoomMultiplePointsToOneDoor(uiDoc);
                            break;
                        case PathCreateOptions.AllRoomCenterToSingleDoor:
                            CreatePathsOfTravelRoomCenterpointsToSingleDoor(uiDoc);
                            break;
                        default:
                            CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(uiDoc);
                            break;
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private void CreatePathsOfTravelInOneRoomMultiplePointsToOneDoor(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;
            var viewPlan = uiDoc.ActiveView as ViewPlan;

            var reference = uiDoc.Selection.PickObject(ObjectType.Element, new RoomSelectionFilter(), "Select a room");
            var room = doc.GetElement(reference) as Room;

            var roomReference =
                uiDoc.Selection.PickObject(ObjectType.Element, new DoorSelectionFilter(), "Select a target door");
            var doorElement = doc.GetElement(roomReference) as Instance;
            var trf = doorElement.GetTransform();
            var endPoint = trf.Origin;

            ResultsSummary resultsSummary = new()
            {
                NumDoors = 1
            };

            var stopwatch = Stopwatch.StartNew();

            GeneratePathsOfTravelForOneRoomOneDoor(doc, viewPlan, room, endPoint, resultsSummary);

            stopwatch.Stop();
            resultsSummary.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            ShowResults(resultsSummary);
        }

        private void CreatePathsOfTravelRoomCenterpointsToSingleDoor(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;
            var viewPlan = uiDoc.ActiveView as ViewPlan;
            var levelId = viewPlan.GenLevel.Id;

            var reference =
                uiDoc.Selection.PickObject(ObjectType.Element, new DoorSelectionFilter(), "Select a target door");
            var doorElement = doc.GetElement(reference) as Instance;
            var trf = doorElement.GetTransform();
            var endPoint = trf.Origin;

            FilteredElementCollector fec = new(doc);
            fec.WherePasses(new RoomFilter());

            List<XYZ> startPoints = new();

            foreach (var room in fec.Cast<Room>().Where(rm => rm.Level.Id == levelId))
            {
                if (room.Location is not LocationPoint location)
                    continue;
                var roomPoint = location.Point;
                startPoints.Add(roomPoint);
            }

            using Transaction t = new(doc, "Generate paths of travel");
            t.Start();
            Autodesk.Revit.DB.Analysis.PathOfTravel.CreateMapped(viewPlan, startPoints, [endPoint], out _);
            t.Commit();
        }

        private void CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;
            var viewPlan = uiDoc.ActiveView as ViewPlan;

            CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(doc, viewPlan, false);
        }

        /// <remarks>
        ///     Near-corner points are offset 1.5 ft (18 in) from room boundaries. Only the first boundary region is processed;
        ///     geometry may miss logical corners or land inside furniture.
        /// </remarks>
        private static void AppendRoomNearCornerPoints(Room room, List<XYZ> nearCornerPoints)
        {
            var segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
            if (segments == null || segments.Count == 0)
                return;

            // First region only
            var firstSegments = segments[0];
            var numSegments = firstSegments.Count;

            for (var i = 0; i < numSegments; i++)
            {
                var seg1 = firstSegments.ElementAt(i);
                var seg2 = firstSegments.ElementAt(i == numSegments - 1 ? 0 : i + 1);

                var curve1 = seg1.GetCurve();
                var curve2 = seg2.GetCurve();

                var offsetCurve1 = curve1.CreateOffset(-1.5, XYZ.BasisZ);
                var offsetCurve2 = curve2.CreateOffset(-1.5, XYZ.BasisZ);

                IntersectionResultArray intersections = null;
                var result = offsetCurve1.Intersect(offsetCurve2, out intersections);

                // First intersection only
                if (result == SetComparisonResult.Overlap && intersections.Size == 1)
                    nearCornerPoints.Add(intersections.get_Item(0).XYZPoint);
            }
        }

        private static List<XYZ> GetRoomNearCornerPoints(Room room)
        {
            List<XYZ> nearCornerPoints = new();

            AppendRoomNearCornerPoints(room, nearCornerPoints);

            return nearCornerPoints;
        }

        private static void CreatePathsOfTravelInAllRoomsAllDoorsMultiplePointsManyToMany(Document doc,
            ViewPlan viewPlan, bool mapAllStartsToAllEnds)
        {
            FilteredElementCollector fec = new(doc, viewPlan.Id);
            fec.WherePasses(new RoomFilter());

            FilteredElementCollector fec2 = new(doc, viewPlan.Id);
            fec2.OfCategory(BuiltInCategory.OST_Doors);

            ResultsSummary resultsSummary = new();

            List<XYZ> endPoints = new();

            var rooms = fec.Cast<Room>().ToList();

            foreach (var element in fec2)
            {
                var doorElement = (Instance)element;
                var trf = doorElement.GetTransform();
                endPoints.Add(trf.Origin);
            }

            resultsSummary.NumDoors = endPoints.Count;

            using (TransactionGroup group = new(doc, "Generate all paths of travel"))
            {
                group.Start();

                GeneratePathsOfTravelForRoomsToEndpointsManyToMany(doc, viewPlan, rooms, endPoints, resultsSummary,
                    mapAllStartsToAllEnds);

                group.Assimilate();
            }

            ShowResults(resultsSummary);
        }

        private static void GeneratePathsOfTravelForRoomsToEndpointsManyToMany(Document doc, ViewPlan viewPlan,
            List<Room> rooms, List<XYZ> endPoints, ResultsSummary resultsSummary,
            bool mapAllStartsToAllEnds)
        {
            List<XYZ> allSourcePoints = new();
            foreach (var room in rooms)
            {
                AppendRoomNearCornerPoints(room, allSourcePoints);
            }
            //            foreach (Room room in rooms)
            //            {
            //                LocationPoint location = room.Location as LocationPoint;
            //                if (location == null)
            //                    continue;
            //                XYZ roomPoint = location.Point;
            //                allSourcePoints.Add(roomPoint);
            //            }

            resultsSummary.NumSourcePoints += allSourcePoints.Count;

            List<XYZ> inputStartPoints = null;
            List<XYZ> inputEndPoints = null;

            // mapAllStartsToAllEnds: manual cross-product for testing; CreateMapped is preferred in production.
            if (mapAllStartsToAllEnds)
            {
                List<XYZ> allSourcePointsMappedToEnds = new();
                List<XYZ> allEndPointsMappedToEnds = new();
                foreach (var source in allSourcePoints)
                {
                    foreach (var end in endPoints)
                    {
                        allSourcePointsMappedToEnds.Add(source);
                        allEndPointsMappedToEnds.Add(end);
                    }
                }

                inputStartPoints = allSourcePointsMappedToEnds;
                inputEndPoints = allEndPointsMappedToEnds;
            }
            else
            {
                inputStartPoints = allSourcePoints;
                inputEndPoints = endPoints;
            }

            GeneratePathsOfTravel(doc, viewPlan, inputStartPoints, inputEndPoints, resultsSummary,
                !mapAllStartsToAllEnds);
        }

        private static void GeneratePathsOfTravel(Document doc, ViewPlan viewPlan, List<XYZ> startPoints,
            List<XYZ> endPoints, ResultsSummary resultsSummary, bool mapAllStartsToAllEnds)
        {
            var stopwatch = Stopwatch.StartNew();

            using (Transaction t = new(doc, "Generate paths of travel"))
            {
                t.Start();

                IList<PathOfTravelCalculationStatus> statuses;
                var pathsOfTravel = mapAllStartsToAllEnds
                    ? Autodesk.Revit.DB.Analysis.PathOfTravel.CreateMapped(viewPlan, startPoints, endPoints, out statuses)
                    : Autodesk.Revit.DB.Analysis.PathOfTravel.CreateMultiple(viewPlan, startPoints, endPoints, out statuses);
                var i = 0;

                foreach (var pathOfTravel in pathsOfTravel)
                {
                    if (pathOfTravel == null)
                    {
                        resultsSummary.NumFailures++;
                        resultsSummary.FailuresFound.Add(statuses[i]);
                    }
                    else
                    {
                        resultsSummary.NumSuccesses++;
                    }

                    i++;
                }

                t.Commit();
            }

            stopwatch.Stop();
            resultsSummary.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        }

        private static void GeneratePathsOfTravelForOneRoomManyDoors(Document doc, ViewPlan viewPlan, Room room,
            List<XYZ> endPoints, ResultsSummary resultsSummary)
        {
            var sourcePoints = GetRoomNearCornerPoints(room);
            resultsSummary.NumSourcePoints += sourcePoints.Count;


            using Transaction t = new(doc, "Generate paths of travel");
            t.Start();
            var pathsOfTravel = Autodesk.Revit.DB.Analysis.PathOfTravel.CreateMapped(viewPlan, sourcePoints, endPoints, out _);

            foreach (var pOt in pathsOfTravel)
            {
                if (pOt == null) resultsSummary.NumFailures++;
                else resultsSummary.NumSuccesses++;
            }

            t.Commit();
        }

        private static void GeneratePathsOfTravelForOneRoomOneDoor(Document doc, ViewPlan viewPlan, Room room,
            XYZ endPoint, ResultsSummary resultsSummary)
        {
            GeneratePathsOfTravelForOneRoomManyDoors(doc, viewPlan, room, [endPoint], resultsSummary);
        }

        private static void ShowResults(ResultsSummary resultsSummary)
        {
            CultureInfo ci = new("en-us");

            var numOfPathsToCreate = resultsSummary.NumSourcePoints * resultsSummary.NumDoors;

            var successRatePercent = resultsSummary.NumSuccesses / (double)numOfPathsToCreate;

            TaskDialog td = new("Results of PathOfTravel creation")
            {
                MainInstruction =
                $"Path of Travel succeeded on {successRatePercent.ToString("P01", ci)} of known points"
            };
            var details = string.Format(
                "There were {0} room source points found in room analysis (via offsetting boundaries). " +
                "They would be connected to {2} door target points. {1} failed to generate a Path of Travel out of {4}  " +
                "Processing took {3} milliseconds.",
                resultsSummary.NumSourcePoints, resultsSummary.NumFailures, resultsSummary.NumDoors,
                resultsSummary.ElapsedMilliseconds, numOfPathsToCreate);
            if (resultsSummary.NumFailures > 0)
                details += "  Most likely reason for failures is an obstacle on or nearby to the source point.";
            td.MainContent = details;

            td.Show();
        }

        private class DoorSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return element.Category.Id == new ElementId(BuiltInCategory.OST_Doors);
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return false;
            }
        }

        private class RoomSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return element is Room;
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return false;
            }
        }

        private class ResultsSummary
        {
            public int NumSourcePoints { get; set; }

            public int NumDoors { get; set; }

            public int NumSuccesses { get; set; }

            public int NumFailures { get; set; }

            public long ElapsedMilliseconds { get; set; }

            public List<PathOfTravelCalculationStatus> FailuresFound { get; } = [];
        }
    }
}
