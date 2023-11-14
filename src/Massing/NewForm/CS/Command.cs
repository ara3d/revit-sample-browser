// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewForm.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class show how to create extrusion form by Revit API.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class MakeExtrusionForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "MakeExtrusionForm");
            transaction.Start();

            // Create one profile
            var ref_ar = new ReferenceArray();

            var ptA = new XYZ(10, 10, 0);
            var ptB = new XYZ(90, 10, 0);
            var modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(90, 10, 0);
            ptB = new XYZ(10, 90, 0);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 90, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            // The extrusion form direction
            var direction = new XYZ(0, 0, 50);

            doc.FamilyCreate.NewExtrusionForm(true, ref_ar, direction);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class show how to create cap form by Revit API.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeCapForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "MakeCapForm");
            transaction.Start();

            // Create one profile
            var ref_ar = new ReferenceArray();

            var ptA = new XYZ(10, 10, 0);
            var ptB = new XYZ(100, 10, 0);
            Line.CreateBound(ptA, ptB);
            var modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(100, 10, 0);
            ptB = new XYZ(50, 50, 0);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(50, 50, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            doc.FamilyCreate.NewFormByCap(true, ref_ar);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class show how to create revolve form by Revit API.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeRevolveForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "MakeRevolveForm");
            transaction.Start();

            // Create one profile
            var ref_ar = new ReferenceArray();
            var norm = XYZ.BasisZ;

            var ptA = new XYZ(0, 0, 10);
            var ptB = new XYZ(100, 0, 10);
            var modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB, norm);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(100, 0, 10);
            ptB = new XYZ(100, 100, 10);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB, norm);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(100, 100, 10);
            ptB = new XYZ(0, 0, 10);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB, norm);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            // Create axis for revolve form
            ptA = new XYZ(-5, 0, 10);
            ptB = new XYZ(-5, 10, 10);
            var axis = FormUtils.MakeLine(commandData.Application, ptA, ptB, norm);
            axis.ChangeToReferenceLine();

            doc.FamilyCreate.NewRevolveForms(true, ref_ar, axis.GeometryCurve.Reference, 0, Math.PI / 4);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class show how to create swept blend form by Revit API.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeSweptBlendForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "MakeSweptBlendForm");
            transaction.Start();

            // Create first profile
            var ref_ar = new ReferenceArray();
            var ptA = new XYZ(10, 10, 0);
            var ptB = new XYZ(50, 10, 0);
            var modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(50, 10, 0);
            ptB = new XYZ(10, 50, 0);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 50, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            // Create second profile
            var ref_ar2 = new ReferenceArray();
            ptA = new XYZ(10, 10, 90);
            ptB = new XYZ(80, 10, 90);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar2.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(80, 10, 90);
            ptB = new XYZ(10, 50, 90);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar2.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 50, 90);
            ptB = new XYZ(10, 10, 90);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            ref_ar2.Append(modelcurve.GeometryCurve.Reference);

            // Add profiles
            var profiles = new ReferenceArrayArray();
            profiles.Append(ref_ar);
            profiles.Append(ref_ar2);

            // Create path for swept blend form
            var path = new ReferenceArray();
            ptA = new XYZ(10, 10, 0);
            ptB = new XYZ(10, 10, 90);
            modelcurve = FormUtils.MakeLine(commandData.Application, ptA, ptB);
            path.Append(modelcurve.GeometryCurve.Reference);

            doc.FamilyCreate.NewSweptBlendForm(true, path, profiles);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class show how to create loft form by Revit API.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeLoftForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var transaction = new Transaction(doc, "MakeLoftForm");
            transaction.Start();

            // Create profiles array
            var ref_ar_ar = new ReferenceArrayArray();

            // Create first profile
            var ref_ar = new ReferenceArray();

            var y = 100;
            var x = 50;
            var ptA = new XYZ(-x, y, 0);
            var ptB = new XYZ(x, y, 0);
            var ptC = new XYZ(0, y + 10, 10);
            var modelcurve = FormUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);
            ref_ar_ar.Append(ref_ar);

            // Create second profile
            ref_ar = new ReferenceArray();

            y = 40;
            ptA = new XYZ(-x, y, 5);
            ptB = new XYZ(x, y, 5);
            ptC = new XYZ(0, y, 25);
            modelcurve = FormUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);
            ref_ar_ar.Append(ref_ar);

            // Create third profile
            ref_ar = new ReferenceArray();

            y = -20;
            ptA = new XYZ(-x, y, 0);
            ptB = new XYZ(x, y, 0);
            ptC = new XYZ(0, y, 15);
            modelcurve = FormUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);
            ref_ar_ar.Append(ref_ar);

            // Create fourth profile
            ref_ar = new ReferenceArray();

            y = -60;
            ptA = new XYZ(-x, y, 0);
            ptB = new XYZ(x, y, 0);
            ptC = new XYZ(0, y + 10, 20);
            modelcurve = FormUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);
            ref_ar_ar.Append(ref_ar);
            ref_ar = new ReferenceArray();
            ref_ar_ar.Append(ref_ar);

            doc.FamilyCreate.NewLoftForm(true, ref_ar_ar);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     This class is utility class for form creation.
    /// </summary>
    public class FormUtils
    {
        /// <summary>
        ///     Create arc element by three points
        /// </summary>
        /// <param name="app">revit application</param>
        /// <param name="ptA">point a</param>
        /// <param name="ptB">point b</param>
        /// <param name="ptC">point c</param>
        /// <returns></returns>
        public static ModelCurve MakeArc(UIApplication app, XYZ ptA, XYZ ptB, XYZ ptC)
        {
            var doc = app.ActiveUIDocument.Document;
            var arc = Arc.Create(ptA, ptB, ptC);
            // Create three lines and a plane by the points
            var line1 = Line.CreateBound(ptA, ptB);
            var line2 = Line.CreateBound(ptB, ptC);
            var line3 = Line.CreateBound(ptC, ptA);
            var ca = new CurveLoop();
            ca.Append(line1);
            ca.Append(line2);
            ca.Append(line3);

            var plane = ca.GetPlane(); // app.Application.Create.NewPlane(ca);
            var skplane = SketchPlane.Create(doc, plane);
            // Create arc here
            var modelcurve = doc.FamilyCreate.NewModelCurve(arc, skplane);
            return modelcurve;
        }

        /// <summary>
        ///     Create line element
        /// </summary>
        /// <param name="app">revit application</param>
        /// <param name="ptA">start point</param>
        /// <param name="ptB">end point</param>
        /// <returns></returns>
        public static ModelCurve MakeLine(UIApplication app, XYZ ptA, XYZ ptB)
        {
            var doc = app.ActiveUIDocument.Document;
            // Create plane by the points
            var line = Line.CreateBound(ptA, ptB);
            var norm = ptA.CrossProduct(ptB);
            if (norm.GetLength() == 0) norm = XYZ.BasisZ;
            var plane = Plane.CreateByNormalAndOrigin(norm, ptB);
            var skplane = SketchPlane.Create(doc, plane);
            // Create line here
            var modelcurve = doc.FamilyCreate.NewModelCurve(line, skplane);
            return modelcurve;
        }

        /// <summary>
        ///     Create line element
        /// </summary>
        /// <param name="app">revit application</param>
        /// <param name="ptA">start point</param>
        /// <param name="ptB">end point</param>
        /// <returns></returns>
        public static ModelCurve MakeLine(UIApplication app, XYZ ptA, XYZ ptB, XYZ norm)
        {
            var doc = app.ActiveUIDocument.Document;
            // Create plane by the points
            var line = Line.CreateBound(ptA, ptB);
            var plane = Plane.CreateByNormalAndOrigin(norm, ptB);
            var skplane = SketchPlane.Create(doc, plane);
            // Create line here
            var modelcurve = doc.FamilyCreate.NewModelCurve(line, skplane);
            return modelcurve;
        }
    }
}
