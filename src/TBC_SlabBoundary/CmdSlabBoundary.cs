#region Header

//
// CmdSlabBoundary.cs - determine polygonal slab boundary loops
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdSlabBoundary : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var floors = new List<Element>();

            if (!Util.GetSelectedElementsOrAll(
                floors, uidoc, typeof(Floor)))
            {
                var sel = uidoc.Selection;

                message = 0 < sel.GetElementIds().Count
                    ? "Please select some floor elements."
                    : "No floor elements found.";

                return Result.Failed;
            }

            var opt = app.Application.Create.NewGeometryOptions();

            var polygons
                = Util.GetFloorBoundaryPolygons(floors, opt);

            var n = polygons.Count;

            Debug.Print(
                "{0} boundary loop{1} found.",
                n, Util.PluralSuffix(n));

            var creator = new Creator(doc);

            using var t = new Transaction(doc);
            t.Start("Draw Slab Boundaries");

            creator.DrawPolygons(polygons);

            t.Commit();

            return Result.Succeeded;
        }
    }
}
