#region Header

//
// CmdIntersectJunctionBox.cs - determine conduits intersecting junction box
//
// Copyright (C) 2018-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdIntersectJunctionBox : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var test_strange_intersect_result = true;
            if (test_strange_intersect_result)
            {
                Util.TestFaceIntersect(doc);
                return Result.Succeeded;
            }

            var e = Util.SelectSingleElement(
                uidoc, "a junction box");

            var bb = e.get_BoundingBox(null);

            var outLne = new Outline(bb.Min, bb.Max);

            ElementQuickFilter fbb
                = new BoundingBoxIntersectsFilter(outLne);

            var conduits
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Conduit))
                    .WherePasses(fbb);

            var nbb = conduits.GetElementCount();

            ElementSlowFilter intersect_junction
                = new ElementIntersectsElementFilter(e);

            conduits = new FilteredElementCollector(doc)
                .OfClass(typeof(Conduit))
                .WherePasses(intersect_junction);

            var nintersect = conduits.GetElementCount();

            Debug.Assert(nintersect <= nbb,
                "expected element intersection to be stricter"
                + "than bounding box containment");

            return Result.Succeeded;
        }
    }
}
