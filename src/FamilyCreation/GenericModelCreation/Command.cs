// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.GenericModelCreation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private FamilyItemFactory m_creationFamily;
        private int m_errCount;
        private string m_errorInfo = "";
        private Document m_familyDocument;
        private Application m_revit;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                m_revit = commandData.Application.Application;
                m_familyDocument = commandData.Application.ActiveUIDocument.Document;
                if (!m_familyDocument.IsFamilyDocument)
                {
                    m_familyDocument = m_revit.NewFamilyDocument("Generic Model.rft");
                    if (null == m_familyDocument)
                    {
                        message = "Cannot open family document";
                        return Result.Failed;
                    }
                }

                m_creationFamily = m_familyDocument.FamilyCreate;
                CreateGenericModel();
                if (0 == m_errCount)
                {
                    return Result.Succeeded;
                }

                message = m_errorInfo;
                return Result.Failed;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return Result.Failed;
            }
        }

        public void CreateGenericModel()
        {
            Transaction transaction = new(m_familyDocument, "CreateGenericModel");
            transaction.Start();
            CreateExtrusion();
            CreateBlend();
            CreateRevolution();
            CreateSweep();
            CreateSweptBlend();
            transaction.Commit();
        }

        private void CreateExtrusion()
        {
            try
            {
                CurveArrArray curveArrArray = new();
                CurveArray curveArray1 = new();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                var p0 = XYZ.Zero;
                XYZ p1 = new(10, 0, 0);
                XYZ p2 = new(10, 10, 0);
                XYZ p3 = new(0, 10, 0);
                var line1 = Line.CreateBound(p0, p1);
                var line2 = Line.CreateBound(p1, p2);
                var line3 = Line.CreateBound(p2, p3);
                var line4 = Line.CreateBound(p3, p0);
                curveArray1.Append(line1);
                curveArray1.Append(line2);
                curveArray1.Append(line3);
                curveArray1.Append(line4);

                curveArrArray.Append(curveArray1);
                var rectExtrusion = m_creationFamily.NewExtrusion(true, curveArrArray, sketchPlane, 10);
                XYZ transPoint1 = new(-16, 0, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, rectExtrusion.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateExtrusion: {e}\r\n";
            }
        }

        private void CreateBlend()
        {
            try
            {
                CurveArray topProfile = new();
                CurveArray baseProfile = new();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                var p00 = XYZ.Zero;
                XYZ p01 = new(10, 0, 0);
                XYZ p02 = new(10, 10, 0);
                XYZ p03 = new(0, 10, 0);
                var line01 = Line.CreateBound(p00, p01);
                var line02 = Line.CreateBound(p01, p02);
                var line03 = Line.CreateBound(p02, p03);
                var line04 = Line.CreateBound(p03, p00);

                baseProfile.Append(line01);
                baseProfile.Append(line02);
                baseProfile.Append(line03);
                baseProfile.Append(line04);

                XYZ p10 = new(5, 2, 10);
                XYZ p11 = new(8, 5, 10);
                XYZ p12 = new(5, 8, 10);
                XYZ p13 = new(2, 5, 10);
                var line11 = Line.CreateBound(p10, p11);
                var line12 = Line.CreateBound(p11, p12);
                var line13 = Line.CreateBound(p12, p13);
                var line14 = Line.CreateBound(p13, p10);

                topProfile.Append(line11);
                topProfile.Append(line12);
                topProfile.Append(line13);
                topProfile.Append(line14);
                var blend = m_creationFamily.NewBlend(true, topProfile, baseProfile, sketchPlane);
                XYZ transPoint1 = new(0, 11, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, blend.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateBlend: {e}\r\n";
            }
        }

        private void CreateRevolution()
        {
            try
            {
                CurveArrArray curveArrArray = new();
                CurveArray curveArray = new();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                var p0 = XYZ.Zero;
                XYZ p1 = new(10, 0, 0);
                XYZ p2 = new(10, 10, 0);
                XYZ p3 = new(0, 10, 0);
                var line1 = Line.CreateBound(p0, p1);
                var line2 = Line.CreateBound(p1, p2);
                var line3 = Line.CreateBound(p2, p3);
                var line4 = Line.CreateBound(p3, p0);

                XYZ pp = new(1, -1, 0);
                var axis1 = Line.CreateBound(XYZ.Zero, pp);
                curveArray.Append(line1);
                curveArray.Append(line2);
                curveArray.Append(line3);
                curveArray.Append(line4);

                curveArrArray.Append(curveArray);
                var revolution1 = m_creationFamily.NewRevolution(true, curveArrArray, sketchPlane, axis1, -Math.PI, 0);
                XYZ transPoint1 = new(0, 32, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, revolution1.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateRevolution: {e}\r\n";
            }
        }

        private void CreateSweep()
        {
            try
            {
                CurveArrArray arrarr = new();
                CurveArray arr = new();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                XYZ pnt1 = new(0, 0, 0);
                XYZ pnt2 = new(2, 0, 0);
                XYZ pnt3 = new(1, 1, 0);
                arr.Append(Arc.Create(pnt2, 1.0d, 0.0d, 180.0d, XYZ.BasisX, XYZ.BasisY));
                arr.Append(Arc.Create(pnt1, pnt3, pnt2));
                arrarr.Append(arr);
                SweepProfile profile = m_revit.Create.NewCurveLoopsProfile(arrarr);

                XYZ pnt4 = new(10, 0, 0);
                XYZ pnt5 = new(0, 10, 0);
                Curve curve = Line.CreateBound(pnt4, pnt5);

                CurveArray curves = new();
                curves.Append(curve);
                var sweep1 =
                    m_creationFamily.NewSweep(true, curves, sketchPlane, profile, 0, ProfilePlaneLocation.Start);
                XYZ transPoint1 = new(11, 0, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, sweep1.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateSweep: {e}\r\n";
            }
        }

        private void CreateSweptBlend()
        {
            try
            {
                XYZ pnt1 = new(0, 0, 0);
                XYZ pnt2 = new(1, 0, 0);
                XYZ pnt3 = new(1, 1, 0);
                XYZ pnt4 = new(0, 1, 0);
                new XYZ(0, 0, 1);

                CurveArrArray arrarr1 = new();
                CurveArray arr1 = new();
                arr1.Append(Line.CreateBound(pnt1, pnt2));
                arr1.Append(Line.CreateBound(pnt2, pnt3));
                arr1.Append(Line.CreateBound(pnt3, pnt4));
                arr1.Append(Line.CreateBound(pnt4, pnt1));
                arrarr1.Append(arr1);

                XYZ pnt6 = new(0.5, 0, 0);
                XYZ pnt7 = new(1, 0.5, 0);
                XYZ pnt8 = new(0.5, 1, 0);
                XYZ pnt9 = new(0, 0.5, 0);
                CurveArrArray arrarr2 = new();
                CurveArray arr2 = new();
                arr2.Append(Line.CreateBound(pnt6, pnt7));
                arr2.Append(Line.CreateBound(pnt7, pnt8));
                arr2.Append(Line.CreateBound(pnt8, pnt9));
                arr2.Append(Line.CreateBound(pnt9, pnt6));
                arrarr2.Append(arr2);

                SweepProfile bottomProfile = m_revit.Create.NewCurveLoopsProfile(arrarr1);
                SweepProfile topProfile = m_revit.Create.NewCurveLoopsProfile(arrarr2);

                XYZ pnt10 = new(5, 0, 0);
                XYZ pnt11 = new(0, 20, 0);
                Curve curve = Line.CreateBound(pnt10, pnt11);

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);
                var newSweptBlend1 =
                    m_creationFamily.NewSweptBlend(true, curve, sketchPlane, bottomProfile, topProfile);
                XYZ transPoint1 = new(11, 32, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, newSweptBlend1.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateSweptBlend: {e}\r\n";
            }
        }

        private T GetElement<T>(long eid) where T : Element
        {
            ElementId elementId = new(eid);
            return m_familyDocument.GetElement(elementId) as T;
        }

        public SketchPlane CreateSketchPlane(XYZ normal, XYZ origin)
        {
            // SketchPlane.Create requires a Geometry.Plane, not just normal/origin vectors.
            var geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin);
            if (null == geometryPlane)
                throw new Exception("Create the geometry plane failed.");
            var plane = SketchPlane.Create(m_familyDocument, geometryPlane);
            return plane ?? throw new Exception("Create the sketch plane failed.");
        }
    }
}
