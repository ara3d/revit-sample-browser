// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.DimensionLeaderEnd.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveHorizontally : IExternalCommand
    {
        private readonly double m_delta = -10;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            // Get the handle of current document.
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            using (var transaction = new Transaction(doc))
            {
                // Get the element selection of current document.
                var selectedIds = uidoc.Selection.GetElementIds();

                if (0 == selectedIds.Count)
                    // If no elements selected.
                    TaskDialog.Show("Revit", "You haven't selected any elements.");
                else
                    foreach (var id in selectedIds)
                    {
                        if (doc.GetElement(id) is Dimension dim)
                        {
                            var dimLine = dim.Curve as Line;
                            if (dimLine != null)
                            {
                                transaction.Start("Set leader end position.");
                                try
                                {
                                    var dir = dimLine.Direction;
                                    if (dim.Segments.IsEmpty)
                                    {
                                        var leaderPos = ComputeLeaderPosition(dir, dim.Origin);
                                        dim.LeaderEndPosition = leaderPos;
                                    }
                                    else
                                    {
                                        foreach (DimensionSegment ds in dim.Segments)
                                        {
                                            var leaderPos = ComputeLeaderPosition(dir, ds.Origin);
                                            ds.LeaderEndPosition = leaderPos;
                                        }
                                    }

                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    TaskDialog.Show("Can't set dimension leader end point: {0}", ex.Message);
                                    transaction.RollBack();
                                }
                            }
                        }
                    }

                return Result.Succeeded;
            }
        }

        private XYZ ComputeLeaderPosition(XYZ dir, XYZ origin)
        {
            var leaderPos = new XYZ();
            leaderPos = dir * m_delta;
            leaderPos = leaderPos.Add(origin);
            return leaderPos;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveToPickedPoint : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            // Get the handle of current document.
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            using (var transaction = new Transaction(doc))
            {
                // Get the element selection of current document.
                var selection = uidoc.Selection;
                var selectedIds = uidoc.Selection.GetElementIds();

                if (0 == selectedIds.Count)
                    // If no elements selected.
                    TaskDialog.Show("Revit", "You haven't selected any elements.");
                else
                    foreach (var id in selectedIds)
                    {
                        if (doc.GetElement(id) is Dimension dim)
                        {
                            var startPoint = selection.PickPoint(ObjectSnapTypes.None, "Pick start");
                            transaction.Start("Set leader end point");
                            try
                            {
                                if (dim.Segments.IsEmpty)
                                {
                                    dim.LeaderEndPosition = startPoint;
                                }
                                else
                                {
                                    var deltaVec = dim.Segments.get_Item(1).Origin
                                        .Subtract(dim.Segments.get_Item(0).Origin);
                                    var offset = new XYZ();
                                    foreach (DimensionSegment ds in dim.Segments)
                                    {
                                        ds.LeaderEndPosition = startPoint.Add(offset);
                                        offset = offset.Add(deltaVec);
                                    }
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                TaskDialog.Show("Can't set dimension leader end point: {0}", ex.Message);
                                transaction.RollBack();
                            }
                        }
                    }

                return Result.Succeeded;
            }
        }
    }
}
