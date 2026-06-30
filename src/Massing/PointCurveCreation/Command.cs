// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;

namespace Ara3D.RevitSampleBrowser.Massing.PointCurveCreation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PointsParabola : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("B6FBC0C1-F3AE-4ffa-AB46-B4CF94304827"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "PointsParabola");
            transaction.Start();
            double yctr = 0;
            var power = 1.2;
            while (power < 1.5)
            {
                double xctr = 0;
                double zctr = 0;
                while (zctr < 100)
                {
                    zctr = Math.Pow(xctr, power);
                    XYZ xyz = new(xctr, yctr, zctr);
                    doc.FamilyCreate.NewReferencePoint(xyz);
                    if (xctr > 0)
                    {
                        xyz = new XYZ(-xctr, yctr, zctr);
                        doc.FamilyCreate.NewReferencePoint(xyz);
                    }

                    xctr++;
                }

                power += 0.1;
                yctr += 50;
                zctr = 0;
            }

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PointsOnCurve : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("22D07F77-A3F7-490c-B0D8-0EC10D8DE7C7"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "PointsOnCurve");
            transaction.Start();
            XYZ start = new(0, 0, 0);
            XYZ end = new(50, 50, 0);
            var line = Line.CreateBound(start, end);
            var geometryPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, start);
            var skplane = SketchPlane.Create(doc, geometryPlane);
            var modelcurve = doc.FamilyCreate.NewModelCurve(line, skplane);

            for (var i = 0.1; i <= 1; i += 0.1)
            {
                PointLocationOnCurve locationOnCurve = new(PointOnCurveMeasurementType.NormalizedCurveParameter, i,
                    PointOnCurveMeasureFrom.Beginning);
                var poe = app.Create.NewPointOnEdge(modelcurve.GeometryCurve.Reference, locationOnCurve);
                doc.FamilyCreate.NewReferencePoint(poe);
            }

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class PointsFromTextFile : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("C6D0F4DB-81C3-4927-9D68-8936D6EE67DD"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "PointsParabola");
            transaction.Start();
            var filename = "sphere.csv";
            var filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (File.Exists($"{filepath}\\{filename}"))
            {
                StreamReader readFile = new($"{filepath}\\{filename}");
                string line;
                while ((line = readFile.ReadLine()) != null)
                {
                    var data = line.Split(',');
                    XYZ xyz = new(Convert.ToDouble(data[0]), Convert.ToDouble(data[1]), Convert.ToDouble(data[2]));
                    doc.FamilyCreate.NewReferencePoint(xyz);
                }
            }

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SineCurve : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("F18A831C-AE42-43cc-91FD-6B5D461A1AC7"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "PointsParabola");
            transaction.Start();
            var pntCtr = 0;
            double xctr = 0;
            XYZ xyz = new();
            ReferencePointArray rparray = new();
            while (pntCtr < 500)
            {
                xyz = new XYZ(xctr, 0, Math.Cos(xctr) * 10);
                var rp = doc.FamilyCreate.NewReferencePoint(xyz);
                rparray.Append(rp);
                xctr += 0.1;
                pntCtr++;
            }

            doc.FamilyCreate.NewCurveByPoints(rparray);
            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CatenaryCurve : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("817C2A99-BF00-4029-86F3-2D10550F1410"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "CatenaryCurve");
            transaction.Start();
            for (double scalingFactor = 1; scalingFactor <= 2; scalingFactor += 0.5)
            {
                ReferencePointArray rpArray = new();
                for (double x = -5; x <= 5; x += 0.5)
                {
                    var y = scalingFactor * Math.Cosh(x / scalingFactor);
                    if (y < 50)
                    {
                        var rp = doc.FamilyCreate.NewReferencePoint(new XYZ(x, y, 0));
                        rpArray.Append(rp);
                    }
                }

                doc.FamilyCreate.NewCurveByPoints(rpArray);
            }

            transaction.Commit();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CyclicSurface : IExternalCommand
    {
        private static readonly AddInId _appId = new(new Guid("3F926F3E-D93A-41cd-9ABF-A31594A827B3"));

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            Transaction transaction = new(doc, "CyclicSurface");
            transaction.Start();
            XYZ xyz = new();
            ReferenceArrayArray refArAr = new();
            var x = 0;
            while (x < 800)
            {
                ReferencePointArray rpAr = new();
                var y = 0;
                while (y < 800)
                {
                    var z = 50 * (Math.Cos(Math.PI / 180 * x) + Math.Cos(Math.PI / 180 * y));
                    xyz = new XYZ(x, y, z);
                    var rp = doc.FamilyCreate.NewReferencePoint(xyz);
                    rpAr.Append(rp);
                    y += 40;
                }

                var curve = doc.FamilyCreate.NewCurveByPoints(rpAr);
                ReferenceArray refAr = new();
                refAr.Append(curve.GeometryCurve.Reference);
                refArAr.Append(refAr);
                x += 40;
            }

            doc.FamilyCreate.NewLoftForm(true, refArAr);
            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
