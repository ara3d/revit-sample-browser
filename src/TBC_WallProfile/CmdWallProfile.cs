#region Header

//
// CmdWallProfile.cs - determine wall
// elevation profile boundary loop polygons
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
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdWallProfile : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData cd,
            ref string msg,
            ElementSet els)
        {
            var use_execute_nr = 3;

            switch (use_execute_nr)
            {
                case 1: return Execute1(cd, ref msg, els);
                case 2: return Execute2(cd, ref msg, els);
                case 3: return Execute3(cd, ref msg, els);
            }

            return Result.Failed;
        }

        // http://thebuildingcoder.typepad.com/blog/2008/11/wall-elevation-profile.html
        public Result Execute1(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var walls = new List<Element>();

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
                = Util.GetWallProfilePolygons(walls, opt);

            var n = polygons.Count;

            Debug.Print(
                "{0} boundary loop{1} found.",
                n, Util.PluralSuffix(n));

            var creator = new Creator(doc);

            using var tx = new Transaction(doc);
            tx.Start("Draw Wall Elevation Profile Model Lines");
            creator.DrawPolygons(polygons);
            tx.Commit();

            return Result.Succeeded;
        }

        // http://thebuildingcoder.typepad.com/blog/2015/01/getting-the-wall-elevation-profile.html
        public Result Execute2(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            var creapp
                = app.Create;

            var credoc
                = doc.Create;

            var r = uidoc.Selection.PickObject(
                ObjectType.Element, "Select a wall");

            var e = uidoc.Document.GetElement(r);

            var wall = e as Wall;

            using var tx = new Transaction(doc);
            tx.Start("Wall Profile");

            var sideFaces
                = HostObjectUtils.GetSideFaces(wall,
                    ShellLayerType.Exterior);

            var e2 = doc.GetElement(sideFaces[0]);

            var face = e2.GetGeometryObjectFromReference(
                sideFaces[0]) as Face;

            var normal = face.ComputeNormal(new UV(0, 0));

            var offset = Transform.CreateTranslation(
                5 * normal);

            var colorRed = new Color(255, 0, 0);

            var curveLoops
                = face.GetEdgesAsCurveLoops();

            // ExporterIFCUtils.SortCurveLoops puts outer loops first (non-IFC use is fine).
            var curveLoopLoop
                = ExporterIFCUtils.SortCurveLoops(
                    curveLoops);

            foreach (var curveLoops2
                in curveLoopLoop)
            foreach (var curveLoop2 in curveLoops2)
            {
                var isCCW = curveLoop2.IsCounterclockwise(
                    normal);

                var curves = creapp.NewCurveArray();

                foreach (var curve in curveLoop2) curves.Append(curve.CreateTransformed(offset));

                //Plane plane = creapp.NewPlane( curves ); // 2016

                var plane = curveLoop2.GetPlane(); // 2017

                var sketchPlane
                    = SketchPlane.Create(doc, plane);

                var curveElements
                    = credoc.NewModelCurveArray(curves,
                        sketchPlane);

                if (isCCW)
                    foreach (ModelCurve mcurve in curveElements)
                    {
                        var overrides
                            = view.GetElementOverrides(
                                mcurve.Id);

                        overrides.SetProjectionLineColor(
                            colorRed);

                        view.SetElementOverrides(
                            mcurve.Id, overrides);
                    }
            }

            tx.Commit();

            return Result.Succeeded;
        }

        // Curved wall with curved window; Alexander Ignatovich, April 2015.
        public Result Execute3(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            var creapp
                = app.Create;

            var credoc
                = doc.Create;

            var r = uidoc.Selection.PickObject(
                ObjectType.Element, "Select a wall");

            var e = uidoc.Document.GetElement(r);

            var creator = new Creator(doc);

            if (e is not Wall wall) return Result.Cancelled;

            using var tx = new Transaction(doc);
            tx.Start("Wall Profile");

            var sideFaceReference
                = HostObjectUtils.GetSideFaces(
                        wall, ShellLayerType.Exterior)
                    .First();

            var face = wall.GetGeometryObjectFromReference(
                sideFaceReference) as Face;

            var normal = wall.Orientation.Normalize();
            var ftx = face.ComputeDerivatives(UV.Zero);
            var forigin = ftx.Origin;
            var fnormal = ftx.BasisZ;

            Debug.Print(
                "wall orientation {0}, face origin {1}, face normal {2}",
                Util.PointString(normal),
                Util.PointString(forigin),
                Util.PointString(fnormal));

            double d = 5;

            var voffset = d * normal;
            var offset = Transform.CreateTranslation(
                voffset);

            var colorRed = new Color(255, 0, 0);

            var curveLoops
                = face.GetEdgesAsCurveLoops();

            foreach (var curveLoop in curveLoops)
            {
                //CurveLoop curveLoopOffset = CurveLoop.CreateViaOffset(
                //  curveLoop, d, normal );

                var curves = creapp.NewCurveArray();

                foreach (var curve in curveLoop)
                    curves.Append(curve.CreateTransformed(
                        offset));

                var isCounterClockwize = curveLoop
                    .IsCounterclockwise(normal);

                var wallCurve = ((LocationCurve) wall.Location).Curve;

                if (wallCurve is Line)
                {
                    //Plane plane = creapp.NewPlane( curves ); // 2016

                    //Plane plane = curveLoopOffset.GetPlane(); // 2017

                    var plane = Plane.CreateByNormalAndOrigin( // 2019
                        normal, forigin + voffset);

                    Debug.Print(
                        "plane origin {0}, plane normal {1}",
                        Util.PointString(plane.Origin),
                        Util.PointString(plane.Normal));

                    var sketchPlane
                        = SketchPlane.Create(doc, plane);

                    var curveElements = credoc
                        .NewModelCurveArray(curves, sketchPlane);

                    if (isCounterClockwize)
                        Util.SetModelCurvesColor(curveElements,
                            view, colorRed);
                }
                else
                {
                    foreach (var curve in curves.Cast<Curve>())
                    {
                        var curveElements = creator.CreateModelCurves(curve);
                        if (isCounterClockwize) Util.SetModelCurvesColor(curveElements, view, colorRed);
                    }
                }
            }

            tx.Commit();

            return Result.Succeeded;
        }
    }
}