#region Namespaces

using System;
using System.Diagnostics;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewSweptBlend sample.</summary>
    internal static partial class Util
    {
        internal static Sweep CreateSweepWithMultipleLoops(
            Document doc)
        {
            var path = new CurveArray();

            path.Append(Line.CreateBound(XYZ.Zero,
                new XYZ(0, 5, 0)));

            var p1 = new XYZ(0, 0, 0);
            var p2 = new XYZ(10, 0, 0);
            var p3 = new XYZ(10, 15, 0);
            var p4 = new XYZ(0, 15, 0);
            var a1 = new XYZ(1, 5, 0);
            var a2 = new XYZ(3, 5, 0);
            var a3 = new XYZ(3, 10, 0);
            var a4 = new XYZ(1, 10, 0);
            var b1 = new XYZ(5, 5, 0);
            var b2 = new XYZ(7, 5, 0);
            var b3 = new XYZ(7, 10, 0);
            var b4 = new XYZ(5, 10, 0);

            var arrcurve = new CurveArrArray();
            var curve = new CurveArray();
            curve.Append(Line.CreateBound(p1, p2));
            curve.Append(Line.CreateBound(p2, p3));
            curve.Append(Line.CreateBound(p3, p4));
            curve.Append(Line.CreateBound(p4, p1));
            arrcurve.Append(curve);
            curve = new CurveArray();
            curve.Append(Line.CreateBound(a1, a4));
            curve.Append(Line.CreateBound(a4, a3));
            curve.Append(Line.CreateBound(a3, a2));
            curve.Append(Line.CreateBound(a2, a1));
            arrcurve.Append(curve);
            curve = new CurveArray();
            curve.Append(Line.CreateBound(b1, b4));
            curve.Append(Line.CreateBound(b4, b3));
            curve.Append(Line.CreateBound(b3, b2));
            curve.Append(Line.CreateBound(b2, b1));
            arrcurve.Append(curve);

            var app = doc.Application;

            SweepProfile profile = app.Create
                .NewCurveLoopsProfile(arrcurve);

            var plane = Plane.CreateByNormalAndOrigin(
                XYZ.BasisZ, XYZ.Zero);

            var sketchPlane = SketchPlane.Create(
                doc, plane);

            var sweep = doc.FamilyCreate.NewSweep(true,
                path, sketchPlane, profile, 0,
                ProfilePlaneLocation.Start);

            return sweep;
        }
        internal static SketchPlane CreateSketchPlane(
            Document doc,
            XYZ normal,
            XYZ origin)
        {
            var geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin);

            if (null == geometryPlane) throw new Exception("Geometry plane creation failed.");

            var plane = SketchPlane.Create(
                doc, geometryPlane);

            if (null == plane) throw new Exception("Sketch plane creation failed.");
            return plane;
        }
        internal static void CreateNewSweptBlend(Document doc)
        {
            Debug.Assert(doc.IsFamilyDocument,
                "this method will only work in a family document");

            var app = doc.Application;

            var creapp
                = app.Create;

            var curvess0
                = creapp.NewCurveArrArray();

            var curves0 = new CurveArray();

            var p00 = creapp.NewXYZ(0, 7.5, 0);
            var p01 = creapp.NewXYZ(0, 15, 0);

            var p02 = creapp.NewXYZ(-1, 10, 0);

            curves0.Append(Line.CreateBound(p00, p01));
            curves0.Append(Line.CreateBound(p01, p02));
            curves0.Append(Line.CreateBound(p02, p00));
            curvess0.Append(curves0);

            var curvess1 = creapp.NewCurveArrArray();
            var curves1 = new CurveArray();

            var p10 = creapp.NewXYZ(7.5, 0, 0);
            var p11 = creapp.NewXYZ(15, 0, 0);

            var p12 = creapp.NewXYZ(10, -1, 0);

            curves1.Append(Line.CreateBound(p10, p11));
            curves1.Append(Line.CreateBound(p11, p12));
            curves1.Append(Line.CreateBound(p12, p10));
            curvess1.Append(curves1);

            SweepProfile sweepProfile0
                = creapp.NewCurveLoopsProfile(curvess0);

            SweepProfile sweepProfile1
                = creapp.NewCurveLoopsProfile(curvess1);

            var pnt10 = new XYZ(5, 0, 0);
            var pnt11 = new XYZ(0, 20, 0);
            Curve curve = Line.CreateBound(pnt10, pnt11);

            var normal = XYZ.BasisZ;

            var splane = CreateSketchPlane(
                doc, normal, XYZ.Zero);

            try
            {
                doc.FamilyCreate.NewSweptBlend(
                    true, curve, splane, sweepProfile0, sweepProfile1);
            }
            catch (Exception ex)
            {
                ErrorMsg($"NewSweptBlend exception: {ex.Message}");
            }
        }
        internal static void CreateNewSweptBlendArc(Document doc)
        {
            Debug.Assert(doc.IsFamilyDocument,
                "this method will only work in a family document");

            var app = doc.Application;

            var creapp
                = app.Create;

            var credoc
                = doc.FamilyCreate;

            var px = XYZ.BasisX;
            var py = XYZ.BasisY;
            var arc1 = Arc.Create(-px, px, -py);
            var arc2 = Arc.Create(px, -px, py);
            var arr1 = new CurveArray();
            arr1.Append(arc1);
            arr1.Append(arc2);
            var arrarr1 = new CurveArrArray();
            arrarr1.Append(arr1);

            SweepProfile bottomProfile
                = creapp.NewCurveLoopsProfile(arrarr1);

            px += px;
            py += py;
            var arc3 = Arc.Create(-px, px, -py);
            var arc4 = Arc.Create(px, -px, py);
            var arr2 = new CurveArray();
            arr2.Append(arc3);
            arr2.Append(arc4);
            var arrarr2 = new CurveArrArray();
            arrarr2.Append(arr2);

            SweepProfile topProfile
                = creapp.NewCurveLoopsProfile(arrarr2);

            var p0 = XYZ.Zero;
            var p5 = 5 * XYZ.BasisY;
            var pmid = new XYZ(2.5, 2.5, 0);
            var testArc = Arc.Create(p0, p5, pmid);

            var geometryPlane = Plane.CreateByNormalAndOrigin(
                XYZ.BasisZ, XYZ.Zero);

            var sketchPlane = SketchPlane.Create(
                doc, geometryPlane);

            credoc.NewSweptBlend(
                true, testArc, sketchPlane, bottomProfile,
                topProfile);
        }
    }
}
