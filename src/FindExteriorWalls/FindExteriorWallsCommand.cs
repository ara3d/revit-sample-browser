// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from RevitFindExteriorWalls by Pekshev / Jeremy Tammik (MIT).
// https://github.com/jeremytammik/RevitFindExteriorWalls

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PickObjectsCanceled = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.FindExteriorWalls.CS
{
    /// <summary>
    /// Identifies exterior walls from a user-selected set using ray casting and end-join propagation.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FindExteriorWallsCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;
            var selection = uidoc.Selection;

            try
            {
                if (doc.ActiveView.ViewType != ViewType.FloorPlan)
                {
                    TaskDialog.Show("Wrong view", "Must be floor plan");
                    return Result.Cancelled;
                }

                List<Wall> selectedWalls;
                try
                {
                    selectedWalls = selection
                        .PickElementsByRectangle(new WallsSelectionFilter(), "Select walls")
                        .Cast<Wall>()
                        .ToList();
                }
                catch (PickObjectsCanceled)
                {
                    return Result.Cancelled;
                }

                if (!selectedWalls.Any())
                    return Result.Cancelled;

                var exteriorWalls = new List<Wall>();

                for (var i = 0; i < selectedWalls.Count; i++)
                {
                    if (selectedWalls[i] == null)
                        continue;

                    var wall = selectedWalls[i];
                    var wallCurve = ((LocationCurve)wall.Location).Curve;
                    var isExterior = true;

                    for (var k = 1; k <= 3; k++)
                    {
                        Curve tempCurve = Line.CreateBound(
                            wallCurve.GetEndPoint(0),
                            wallCurve.GetCenterPoint());
                        if (k == 2)
                            tempCurve = wallCurve;
                        if (k == 3)
                        {
                            tempCurve = Line.CreateBound(
                                wallCurve.GetCenterPoint(),
                                wallCurve.GetEndPoint(1));
                        }

                        var intersectionsOnLeft = 0;
                        var intersectionsOnRight = 0;
                        var leftLine = tempCurve.GetPerpendicularLine(wall, 0);
                        var rightLine = tempCurve.GetPerpendicularLine(wall, 1);

                        for (var j = 0; j < selectedWalls.Count; j++)
                        {
                            if (selectedWalls[j] == null || i == j)
                                continue;

                            var checkedWall = selectedWalls[j];
                            var checkedWallCurve = ((LocationCurve)checkedWall.Location).Curve;

                            if (wallCurve is Line line1 && checkedWallCurve is Line line2 &&
                                Math.Abs(Math.Abs(line1.Direction.DotProduct(line2.Direction))) < 0.0001)
                                continue;

                            if (leftLine.IntersectToByMovingZ(checkedWallCurve))
                                intersectionsOnLeft++;
                            if (rightLine.IntersectToByMovingZ(checkedWallCurve))
                                intersectionsOnRight++;
                        }

                        if (!(intersectionsOnLeft == 0 && intersectionsOnRight != 0) &&
                            !(intersectionsOnRight == 0 && intersectionsOnLeft != 0))
                        {
                            isExterior = false;
                            break;
                        }
                    }

                    if (isExterior && !exteriorWalls.Contains(wall))
                        exteriorWalls.Add(wall);
                }

                var hasIntersections = true;
                var overflow = 0;
                while (hasIntersections)
                {
                    hasIntersections = false;
                    foreach (var selectedWall in selectedWalls)
                    {
                        if (exteriorWalls.Contains(selectedWall))
                            continue;

                        var intersectedByEndsWalls = GetIntersectedByEndsWalls(
                            selectedWalls,
                            exteriorWalls,
                            selectedWall);

                        var allInExterior = true;
                        foreach (var wall in intersectedByEndsWalls)
                        {
                            if (!exteriorWalls.HasWallById(wall))
                            {
                                allInExterior = false;
                                break;
                            }
                        }

                        if (allInExterior)
                        {
                            exteriorWalls.Add(selectedWall);
                            hasIntersections = true;
                        }
                    }

                    overflow++;
                    if (overflow == 1000)
                    {
                        TaskDialog.Show("Error", "Overflow error");
                        break;
                    }
                }

                selection.SetElementIds(exteriorWalls.Select(w => w.Id).ToList());
                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                message += exception.Message;
                return Result.Failed;
            }
        }

        static List<Wall> GetIntersectedByEndsWalls(
            List<Wall> selectedWalls,
            List<Wall> exteriorWalls,
            Wall currentWall)
        {
            var intersectedWalls = new List<Wall>();
            var intersectedWithFirstEnd = GetWallsIntersectedWithCurveByEnd(exteriorWalls, currentWall, 0);
            var intersectedWithSecondEnd = GetWallsIntersectedWithCurveByEnd(exteriorWalls, currentWall, 1);
            var locCurve = (LocationCurve)currentWall.Location;

            var elementsAtEnd = locCurve.get_ElementsAtJoin(0);
            foreach (Element e in elementsAtEnd)
            {
                if (e.Id == currentWall.Id)
                    continue;

                if (selectedWalls.HasWallById((Wall)e))
                {
                    intersectedWalls.Add((Wall)e);
                    if (intersectedWithFirstEnd.Any(w => w.Id == e.Id))
                    {
                        intersectedWithFirstEnd.Remove(
                            intersectedWithFirstEnd.First(w => w.Id == e.Id));
                    }
                }
            }

            elementsAtEnd = locCurve.get_ElementsAtJoin(1);
            foreach (Element e in elementsAtEnd)
            {
                if (e.Id == currentWall.Id)
                    continue;

                if (selectedWalls.HasWallById((Wall)e))
                {
                    intersectedWalls.Add((Wall)e);
                    if (intersectedWithSecondEnd.Any(w => w.Id == e.Id))
                    {
                        intersectedWithSecondEnd.Remove(
                            intersectedWithSecondEnd.First(w => w.Id == e.Id));
                    }
                }
            }

            intersectedWalls.AddRange(intersectedWithFirstEnd);
            intersectedWalls.AddRange(intersectedWithSecondEnd);
            return intersectedWalls;
        }

        static List<Wall> GetWallsIntersectedWithCurveByEnd(
            List<Wall> exteriorWalls,
            Wall currentWall,
            int endIndex)
        {
            var intersectedWalls = new List<Wall>();
            var currentCurve = ((LocationCurve)currentWall.Location).Curve;

            foreach (var exteriorWall in exteriorWalls)
            {
                var exteriorWallCurve = ((LocationCurve)exteriorWall.Location).Curve;
                if (exteriorWall.Id == currentWall.Id)
                    continue;

                if (currentCurve is Line line1 && exteriorWallCurve is Line line2 &&
                    Math.Abs(Math.Abs(line1.Direction.DotProduct(line2.Direction))) < 0.0001)
                    continue;

                if (currentCurve.IntersectToByMovingZ(exteriorWallCurve, out var intersectionResultArray) &&
                    intersectionResultArray.Size == 1)
                {
                    if (Math.Abs(intersectionResultArray.get_Item(0).XYZPoint.DistanceTo(
                            currentCurve.GetEndPoint(endIndex))) < 0.0001)
                    {
                        intersectedWalls.Add(exteriorWall);
                    }
                }
            }

            return intersectedWalls;
        }
    }
}
