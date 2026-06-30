#region Header

//
// CmdSlabBoundaryArea.cs - determine
// slab boundary polygon loops and areas
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
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
    [Transaction(TransactionMode.Manual)]
    internal class CmdSlabBoundaryArea : IExternalCommand
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
                = Util.GetFloorBoundaryPolygons(
                    floors, opt);

            var flat_polygons
                = Util.Flatten(polygons);

            int i = 0, n = flat_polygons.Count;
            var areas = new double[n];
            double a, maxArea = 0.0;
            foreach (var polygon in flat_polygons)
            {
                a = Util.GetSignedPolygonArea(polygon);
                if (Math.Abs(maxArea) < Math.Abs(a)) maxArea = a;
                areas[i++] = a;
            }

            Debug.Print(
                "{0} boundary loop{1} found.",
                n, Util.PluralSuffix(n));

            for (i = 0; i < n; ++i)
                Debug.Print(
                    "  Loop {0} area is {1} square feet{2}",
                    i,
                    Util.RealString(areas[i]),
                    areas[i].Equals(maxArea)
                        ? ", outer loop of largest floor slab"
                        : "");

            using var t = new Transaction(doc);
            t.Start("Draw Polygons");

            var creator = new Creator(doc);
            creator.DrawPolygons(polygons);

            t.Commit();

            return Result.Succeeded;
        }
    }
}
