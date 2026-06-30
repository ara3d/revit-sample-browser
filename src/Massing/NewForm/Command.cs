// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
namespace Ara3D.RevitSampleBrowser.Massing.NewForm.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class MakeExtrusionForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "MakeExtrusionForm");
            transaction.Start();

            // Create one profile
            ReferenceArray refAr = new();

            XYZ ptA = new(10, 10, 0);
            XYZ ptB = new(90, 10, 0);
            var modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(90, 10, 0);
            ptB = new XYZ(10, 90, 0);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 90, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            // The extrusion form direction
            XYZ direction = new(0, 0, 50);

            doc.FamilyCreate.NewExtrusionForm(true, refAr, direction);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeCapForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "MakeCapForm");
            transaction.Start();

            // Create one profile
            ReferenceArray refAr = new();

            XYZ ptA = new(10, 10, 0);
            XYZ ptB = new(100, 10, 0);
            Line.CreateBound(ptA, ptB);
            var modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(100, 10, 0);
            ptB = new XYZ(50, 50, 0);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(50, 50, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            doc.FamilyCreate.NewFormByCap(true, refAr);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeRevolveForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "MakeRevolveForm");
            transaction.Start();

            // Create one profile
            ReferenceArray refAr = new();
            var norm = XYZ.BasisZ;

            XYZ ptA = new(0, 0, 10);
            XYZ ptB = new(100, 0, 10);
            var modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB, norm);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(100, 0, 10);
            ptB = new XYZ(100, 100, 10);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB, norm);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(100, 100, 10);
            ptB = new XYZ(0, 0, 10);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB, norm);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            // Create axis for revolve form
            ptA = new XYZ(-5, 0, 10);
            ptB = new XYZ(-5, 10, 10);
            var axis = XyzMath.MakeLine(commandData.Application, ptA, ptB, norm);
            axis.ChangeToReferenceLine();

            doc.FamilyCreate.NewRevolveForms(true, refAr, axis.GeometryCurve.Reference, 0, Math.PI / 4);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeSweptBlendForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "MakeSweptBlendForm");
            transaction.Start();

            // Create first profile
            ReferenceArray refAr = new();
            XYZ ptA = new(10, 10, 0);
            XYZ ptB = new(50, 10, 0);
            var modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(50, 10, 0);
            ptB = new XYZ(10, 50, 0);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 50, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr.Append(modelcurve.GeometryCurve.Reference);

            // Create second profile
            ReferenceArray refAr2 = new();
            ptA = new XYZ(10, 10, 90);
            ptB = new XYZ(80, 10, 90);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr2.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(80, 10, 90);
            ptB = new XYZ(10, 50, 90);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr2.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 50, 90);
            ptB = new XYZ(10, 10, 90);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            refAr2.Append(modelcurve.GeometryCurve.Reference);

            // Add profiles
            ReferenceArrayArray profiles = new();
            profiles.Append(refAr);
            profiles.Append(refAr2);

            // Create path for swept blend form
            ReferenceArray path = new();
            ptA = new XYZ(10, 10, 0);
            ptB = new XYZ(10, 10, 90);
            modelcurve = XyzMath.MakeLine(commandData.Application, ptA, ptB);
            path.Append(modelcurve.GeometryCurve.Reference);

            doc.FamilyCreate.NewSweptBlendForm(true, path, profiles);

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MakeLoftForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "MakeLoftForm");
            transaction.Start();

            // Create profiles array
            ReferenceArrayArray refArAr = new();

            // Create first profile
            ReferenceArray refAr = new();

            var y = 100;
            var x = 50;
            XYZ ptA = new(-x, y, 0);
            XYZ ptB = new(x, y, 0);
            XYZ ptC = new(0, y + 10, 10);
            var modelcurve = SampleBrowserUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            refAr.Append(modelcurve.GeometryCurve.Reference);
            refArAr.Append(refAr);

            // Create second profile
            refAr = new ReferenceArray();

            y = 40;
            ptA = new XYZ(-x, y, 5);
            ptB = new XYZ(x, y, 5);
            ptC = new XYZ(0, y, 25);
            modelcurve = SampleBrowserUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            refAr.Append(modelcurve.GeometryCurve.Reference);
            refArAr.Append(refAr);

            // Create third profile
            refAr = new ReferenceArray();

            y = -20;
            ptA = new XYZ(-x, y, 0);
            ptB = new XYZ(x, y, 0);
            ptC = new XYZ(0, y, 15);
            modelcurve = SampleBrowserUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            refAr.Append(modelcurve.GeometryCurve.Reference);
            refArAr.Append(refAr);

            // Create fourth profile
            refAr = new ReferenceArray();

            y = -60;
            ptA = new XYZ(-x, y, 0);
            ptB = new XYZ(x, y, 0);
            ptC = new XYZ(0, y + 10, 20);
            modelcurve = SampleBrowserUtils.MakeArc(commandData.Application, ptA, ptB, ptC);
            refAr.Append(modelcurve.GeometryCurve.Reference);
            refArAr.Append(refAr);
            refAr = new ReferenceArray();
            refArAr.Append(refAr);

            doc.FamilyCreate.NewLoftForm(true, refArAr);

            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
