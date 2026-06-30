#region Header

//
// CmdListViews.cs - determine all the view
// ports of a drawing sheet and vice versa
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdListViews : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var run_ViewSheetSet_Views_benchmark = true;

            if (run_ViewSheetSet_Views_benchmark)
            {
                var s = Util.GetViewSheetSetViewsBenchmark(doc);
                var td = new TaskDialog(
                    "ViewSheetSet.Views Benchmark");
                td.MainContent = s;
                td.Show();
                return Result.Succeeded;
            }

            var sheets
                = new FilteredElementCollector(doc);

            sheets.OfClass(typeof(ViewSheet));

            // map with key = sheet element id and
            // value = list of viewport element ids:

            var
                mapSheetToViewport =
                    new Dictionary<ElementId, List<ElementId>>();

            // map with key = viewport element id and
            // value = sheet element id:

            var mapViewportToSheet
                = new Dictionary<ElementId, ElementId>();

            foreach (ViewSheet sheet in sheets)
            {
                //int n = sheet.Views.Size; // 2014

                var viewIds = sheet.GetAllPlacedViews(); // 2015

                var n = viewIds.Count;

                Debug.Print(
                    "Sheet {0} contains {1} view{2}: ",
                    Util.ElementDescription(sheet),
                    n, Util.PluralSuffix(n));

                var idSheet = sheet.Id;

                var i = 0;

                foreach (var id in viewIds)
                {
                    var v = doc.GetElement(id) as View;

                    BoundingBoxXYZ bb;

                    bb = v.get_BoundingBox(doc.ActiveView);

                    Debug.Assert(null == bb,
                        "expected null view bounding box");

                    bb = v.get_BoundingBox(sheet);

                    Debug.Assert(null == bb,
                        "expected null view bounding box");

                    var viewport = Util.GetViewport(sheet, v);

                    // null if not in active view:

                    bb = viewport.get_BoundingBox(doc.ActiveView);

                    var outline = v.Outline;

                    Debug.WriteLine("  {0} {1} bb {2} outline {3}", ++i, Util.ElementDescription(v), null == bb ? "<null>" : Util.BoundingBoxString(bb),
                        Util.BoundingBoxString(outline));

                    if (!mapSheetToViewport.ContainsKey(idSheet))
                        mapSheetToViewport.Add(idSheet,
                            new List<ElementId>());
                    mapSheetToViewport[idSheet].Add(v.Id);

                    Debug.Assert(
                        !mapViewportToSheet.ContainsKey(v.Id),
                        "expected viewport to be contained"
                        + " in only one single sheet");

                    mapViewportToSheet.Add(v.Id, idSheet);
                }
            }

            return Result.Cancelled;
        }
    }
}
