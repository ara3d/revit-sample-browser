#region Header

//
// CmdWallProfileAreas.cs - determine wall
// elevation profile boundary loop polygon areas
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdWallProfileArea : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            List<Element> walls = new();
            if (!Util.GetSelectedElementsOrAll(
                walls, uidoc, typeof(Wall)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some wall elements."
                    : "No wall elements found.";
                return Result.Failed;
            }

            var opt = app.Application.Create.NewGeometryOptions();

            var polygons
                = Util.GetWallProfilePolygons(
                    walls, opt);

            int i = 0, n = polygons.Count;
            var areas = new double[n];
            double d, a, maxArea = 0.0;
            XYZ normal;
            foreach (var polygon in polygons)
            {
                Util.GetPolygonPlane(polygon,
                    out normal, out d, out a);
                if (Math.Abs(maxArea) < Math.Abs(a)) maxArea = a;
                areas[i++] = a;

#if DEBUG

                var t = Util.GetTransformToZ(normal);

                var polygonHorizontal
                    = Util.ApplyTransform(polygon, t);

                var polygon2d
                    = Util.Flatten(
                        polygonHorizontal);

                var a2
                    = Util.GetSignedPolygonArea(
                        polygon2d);

                Debug.Assert(Util.IsEqual(a, a2),
                    "expected same area from 2D and 3D calculations");
#endif
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
                        ? ", outer loop of largest wall"
                        : "");

            Creator creator = new(doc);

            using Transaction tx = new(doc);
            tx.Start("Draw wall profile loops");
            creator.DrawPolygons(polygons);
            tx.Commit();

            return Result.Succeeded;
        }
    }
}
