// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.GenericModelCreation.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     This class show how to create Generic Model Family by Revit API.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        // FamilyItemFactory used to create family
        private FamilyItemFactory m_creationFamily;

        // Count error numbers
        private int m_errCount;

        // Error information
        private string m_errorInfo = "";

        // the document to create generic model family
        private Document m_familyDocument;

        // Application of Revit
        private Application m_revit;

        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                m_revit = commandData.Application.Application;
                m_familyDocument = commandData.Application.ActiveUIDocument.Document;
                // create new family document if active document is not a family document
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
                // create generic model family in the document
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

        /// <summary>
        ///     Examples for form creation in generic model families.
        ///     Create extrusion, blend, revolution, sweep, swept blend
        /// </summary>
        public void CreateGenericModel()
        {
            // use transaction if the family document is not active document
            var transaction = new Transaction(m_familyDocument, "CreateGenericModel");
            transaction.Start();
            CreateExtrusion();
            CreateBlend();
            CreateRevolution();
            CreateSweep();
            CreateSweptBlend();
            transaction.Commit();
        }

        /// <summary>
        ///     Create one rectangular extrusion
        /// </summary>
        private void CreateExtrusion()
        {
            try
            {
                var curveArrArray = new CurveArrArray();
                var curveArray1 = new CurveArray();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                // create one rectangular extrusion
                var p0 = XYZ.Zero;
                var p1 = new XYZ(10, 0, 0);
                var p2 = new XYZ(10, 10, 0);
                var p3 = new XYZ(0, 10, 0);
                var line1 = Line.CreateBound(p0, p1);
                var line2 = Line.CreateBound(p1, p2);
                var line3 = Line.CreateBound(p2, p3);
                var line4 = Line.CreateBound(p3, p0);
                curveArray1.Append(line1);
                curveArray1.Append(line2);
                curveArray1.Append(line3);
                curveArray1.Append(line4);

                curveArrArray.Append(curveArray1);
                // here create rectangular extrusion
                var rectExtrusion = m_creationFamily.NewExtrusion(true, curveArrArray, sketchPlane, 10);
                // move to proper place
                var transPoint1 = new XYZ(-16, 0, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, rectExtrusion.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateExtrusion: {e}\r\n";
            }
        }

        /// <summary>
        ///     Create one blend
        /// </summary>
        private void CreateBlend()
        {
            try
            {
                var topProfile = new CurveArray();
                var baseProfile = new CurveArray();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                // create one blend
                var p00 = XYZ.Zero;
                var p01 = new XYZ(10, 0, 0);
                var p02 = new XYZ(10, 10, 0);
                var p03 = new XYZ(0, 10, 0);
                var line01 = Line.CreateBound(p00, p01);
                var line02 = Line.CreateBound(p01, p02);
                var line03 = Line.CreateBound(p02, p03);
                var line04 = Line.CreateBound(p03, p00);

                baseProfile.Append(line01);
                baseProfile.Append(line02);
                baseProfile.Append(line03);
                baseProfile.Append(line04);

                var p10 = new XYZ(5, 2, 10);
                var p11 = new XYZ(8, 5, 10);
                var p12 = new XYZ(5, 8, 10);
                var p13 = new XYZ(2, 5, 10);
                var line11 = Line.CreateBound(p10, p11);
                var line12 = Line.CreateBound(p11, p12);
                var line13 = Line.CreateBound(p12, p13);
                var line14 = Line.CreateBound(p13, p10);

                topProfile.Append(line11);
                topProfile.Append(line12);
                topProfile.Append(line13);
                topProfile.Append(line14);
                // here create one blend
                var blend = m_creationFamily.NewBlend(true, topProfile, baseProfile, sketchPlane);
                // move to proper place
                var transPoint1 = new XYZ(0, 11, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, blend.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateBlend: {e}\r\n";
            }
        }

        /// <summary>
        ///     Create one rectangular profile revolution
        /// </summary>
        private void CreateRevolution()
        {
            try
            {
                var curveArrArray = new CurveArrArray();
                var curveArray = new CurveArray();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                // create one rectangular profile revolution
                var p0 = XYZ.Zero;
                var p1 = new XYZ(10, 0, 0);
                var p2 = new XYZ(10, 10, 0);
                var p3 = new XYZ(0, 10, 0);
                var line1 = Line.CreateBound(p0, p1);
                var line2 = Line.CreateBound(p1, p2);
                var line3 = Line.CreateBound(p2, p3);
                var line4 = Line.CreateBound(p3, p0);

                var pp = new XYZ(1, -1, 0);
                var axis1 = Line.CreateBound(XYZ.Zero, pp);
                curveArray.Append(line1);
                curveArray.Append(line2);
                curveArray.Append(line3);
                curveArray.Append(line4);

                curveArrArray.Append(curveArray);
                // here create rectangular profile revolution
                var revolution1 = m_creationFamily.NewRevolution(true, curveArrArray, sketchPlane, axis1, -Math.PI, 0);
                // move to proper place
                var transPoint1 = new XYZ(0, 32, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, revolution1.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateRevolution: {e}\r\n";
            }
        }

        /// <summary>
        ///     Create one sweep
        /// </summary>
        private void CreateSweep()
        {
            try
            {
                var arrarr = new CurveArrArray();
                var arr = new CurveArray();

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);

                var pnt1 = new XYZ(0, 0, 0);
                var pnt2 = new XYZ(2, 0, 0);
                var pnt3 = new XYZ(1, 1, 0);
                arr.Append(Arc.Create(pnt2, 1.0d, 0.0d, 180.0d, XYZ.BasisX, XYZ.BasisY));
                arr.Append(Arc.Create(pnt1, pnt3, pnt2));
                arrarr.Append(arr);
                SweepProfile profile = m_revit.Create.NewCurveLoopsProfile(arrarr);

                var pnt4 = new XYZ(10, 0, 0);
                var pnt5 = new XYZ(0, 10, 0);
                Curve curve = Line.CreateBound(pnt4, pnt5);

                var curves = new CurveArray();
                curves.Append(curve);
                // here create one sweep with two arcs formed the profile
                var sweep1 =
                    m_creationFamily.NewSweep(true, curves, sketchPlane, profile, 0, ProfilePlaneLocation.Start);
                // move to proper place
                var transPoint1 = new XYZ(11, 0, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, sweep1.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateSweep: {e}\r\n";
            }
        }

        /// <summary>
        ///     Create one SweptBlend
        /// </summary>
        private void CreateSweptBlend()
        {
            try
            {
                var pnt1 = new XYZ(0, 0, 0);
                var pnt2 = new XYZ(1, 0, 0);
                var pnt3 = new XYZ(1, 1, 0);
                var pnt4 = new XYZ(0, 1, 0);
                new XYZ(0, 0, 1);

                var arrarr1 = new CurveArrArray();
                var arr1 = new CurveArray();
                arr1.Append(Line.CreateBound(pnt1, pnt2));
                arr1.Append(Line.CreateBound(pnt2, pnt3));
                arr1.Append(Line.CreateBound(pnt3, pnt4));
                arr1.Append(Line.CreateBound(pnt4, pnt1));
                arrarr1.Append(arr1);

                var pnt6 = new XYZ(0.5, 0, 0);
                var pnt7 = new XYZ(1, 0.5, 0);
                var pnt8 = new XYZ(0.5, 1, 0);
                var pnt9 = new XYZ(0, 0.5, 0);
                var arrarr2 = new CurveArrArray();
                var arr2 = new CurveArray();
                arr2.Append(Line.CreateBound(pnt6, pnt7));
                arr2.Append(Line.CreateBound(pnt7, pnt8));
                arr2.Append(Line.CreateBound(pnt8, pnt9));
                arr2.Append(Line.CreateBound(pnt9, pnt6));
                arrarr2.Append(arr2);

                SweepProfile bottomProfile = m_revit.Create.NewCurveLoopsProfile(arrarr1);
                SweepProfile topProfile = m_revit.Create.NewCurveLoopsProfile(arrarr2);

                var pnt10 = new XYZ(5, 0, 0);
                var pnt11 = new XYZ(0, 20, 0);
                Curve curve = Line.CreateBound(pnt10, pnt11);

                var normal = XYZ.BasisZ;
                var sketchPlane = CreateSketchPlane(normal, XYZ.Zero);
                // here create one swept blend
                var newSweptBlend1 =
                    m_creationFamily.NewSweptBlend(true, curve, sketchPlane, bottomProfile, topProfile);
                // move to proper place
                var transPoint1 = new XYZ(11, 32, 0);
                ElementTransformUtils.MoveElement(m_familyDocument, newSweptBlend1.Id, transPoint1);
            }
            catch (Exception e)
            {
                m_errCount++;
                m_errorInfo += $"Unexpected exceptions occur in CreateSweptBlend: {e}\r\n";
            }
        }

        /// <summary>
        ///     Get element by its id
        /// </summary>
        private T GetElement<T>(long eid) where T : Element
        {
            var elementId = new ElementId(eid);
            return m_familyDocument.GetElement(elementId) as T;
        }

        /// <summary>
        ///     Create sketch plane for generic model profile
        /// </summary>
        /// <param name="normal">plane normal</param>
        /// <param name="origin">origin point</param>
        /// <returns></returns>
        public SketchPlane CreateSketchPlane(XYZ normal, XYZ origin)
        {
            // First create a Geometry.Plane which need in NewSketchPlane() method
            var geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin);
            if (null == geometryPlane) // assert the creation is successful
                throw new Exception("Create the geometry plane failed.");
            // Then create a sketch plane using the Geometry.Plane
            var plane = SketchPlane.Create(m_familyDocument, geometryPlane);
            // throw exception if creation failed
            if (null == plane) throw new Exception("Create the sketch plane failed.");
            return plane;
        }
    }
}
