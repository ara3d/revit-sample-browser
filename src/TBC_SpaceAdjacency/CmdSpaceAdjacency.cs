#region Header

//
// CmdSpaceAdjacency.cs - determine space adjacencies.
//
// Copyright (C) 2009-2020 by Martin Schmid and Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdSpaceAdjacency : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            List<Element> spaces = new();
            if (!Util.GetSelectedElementsOrAll(
                spaces, uidoc, typeof(Space)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some space elements."
                    : "No space elements found.";
                return Result.Failed;
            }

            List<SpaceAdjacencySegment> segments = new();

            foreach (Space space in spaces) Util.GetBoundaries(segments, space);

            Dictionary<SpaceAdjacencySegment, SpaceAdjacencySegment> segmentPairs
                = new();

            Util.FindClosestSegments(segmentPairs, segments);

            Dictionary<Space, List<Space>> spaceAdjacencies
                = new();

            Util.DetermineAdjacencies(
                spaceAdjacencies, segmentPairs);

            Util.ReportAdjacencies(spaceAdjacencies);

            return Result.Failed;
        }
    }
}
