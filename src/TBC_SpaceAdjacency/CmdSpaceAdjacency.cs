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

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

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

            var spaces = new List<Element>();
            if (!Util.GetSelectedElementsOrAll(
                spaces, uidoc, typeof(Space)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some space elements."
                    : "No space elements found.";
                return Result.Failed;
            }

            var segments = new List<SpaceAdjacencySegment>();

            foreach (Space space in spaces) Util.GetBoundaries(segments, space);

            var segmentPairs
                = new Dictionary<SpaceAdjacencySegment, SpaceAdjacencySegment>();

            Util.FindClosestSegments(segmentPairs, segments);

            var spaceAdjacencies
                = new Dictionary<Space, List<Space>>();

            Util.DetermineAdjacencies(
                spaceAdjacencies, segmentPairs);

            Util.ReportAdjacencies(spaceAdjacencies);

            return Result.Failed;
        }
    }
}
