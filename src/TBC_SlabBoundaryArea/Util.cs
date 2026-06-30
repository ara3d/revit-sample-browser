#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SlabBoundaryArea sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Use the formula
        ///     area = sign * 0.5 * sum( xi * ( yi+1 - yi-1 ) )
        ///     to determine the winding direction (clockwise
        ///     or counter) and area of a 2D polygon.
        ///     Cf. also GetPolygonPlane.
        /// </summary>
        public static double GetSignedPolygonArea(List<UV> p)
        {
            var n = p.Count;
            var sum = p[0].U * (p[1].V - p[n - 1].V);
            for (var i = 1; i < n - 1; ++i) sum += p[i].U * (p[i + 1].V - p[i - 1].V);
            sum += p[n - 1].U * (p[0].V - p[n - 2].V);
            return 0.5 * sum;
        }

        public static Result GetPlanarFaceOuterLoops(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var IntApp = commandData.Application;
            var IntUIDoc = IntApp.ActiveUIDocument;
            if (IntUIDoc == null)
                return Result.Failed;
            var IntDoc = IntUIDoc.Document;

            Reference R = null;
            try
            {
                R = IntUIDoc.Selection.PickObject(ObjectType.Face);
            }
            catch
            {
            }

            if (R == null)
                return Result.Cancelled;

            var F_El = IntDoc.GetElement(R.ElementId);
            if (F_El == null)
                return Result.Failed;

            var F = F_El.GetGeometryObjectFromReference(R)
                as PlanarFace;

            if (F == null)
                return Result.Failed;

            var CLoop
                = new List<Tuple<PlanarFace, CurveLoop, int>>();

            var Ix = 0;
            foreach (var item in F.GetEdgesAsCurveLoops())
            {
                var CLL = new List<CurveLoop>();
                CLL.Add(item);
                var S = GeometryCreationUtilities
                    .CreateExtrusionGeometry(CLL, F.FaceNormal, 1);

                foreach (Face Fx in S.Faces)
                {
                    var PFx = Fx as PlanarFace;
                    if (PFx == null)
                        continue;
                    if (PFx.FaceNormal.IsAlmostEqualTo(
                        F.FaceNormal))
                    {
                        Ix += 1;
                        CLoop.Add(new Tuple<PlanarFace,
                            CurveLoop, int>(PFx, item, Ix));
                    }
                }
            }

            var OuterLoops = new List<CurveLoop>();
            var InnerLoops = new List<CurveLoop>();
            foreach (var item in CLoop)
            {
                var J = CLoop.ToList().FindAll(z
                    => FirstPointIsInsideFace(item.Item2, z.Item1) && z.Item3 != item.Item3).Count;

                if (J == 0)
                    OuterLoops.Add(item.Item2);
                else
                    InnerLoops.Add(item.Item2);
            }

            using var Tx = new Transaction(IntDoc,
                "Outer loops");
            if (Tx.Start() == TransactionStatus.Started)
            {
                var SKP = SketchPlane.Create(IntDoc,
                    Plane.CreateByThreePoints(F.Origin,
                        F.Origin + F.XVector, F.Origin + F.YVector));

                foreach (var Crv in OuterLoops)
                foreach (var C in Crv)
                    IntDoc.Create.NewModelCurve(C, SKP);
                Tx.Commit();
            }

            return Result.Succeeded;
        }

        public static bool FirstPointIsInsideFace(
            CurveLoop CL,
            PlanarFace PFace)
        {
            var Trans = PFace.ComputeDerivatives(
                new UV(0, 0));
            if (CL.Count() == 0)
                return false;
            var Pt = Trans.Inverse.OfPoint(
                CL.ToList()[0].GetEndPoint(0));
            IntersectionResult Res = null;
            var outval = PFace.IsInside(
                new UV(Pt.X, Pt.Y), out Res);
            return outval;
        }

        public static double MinU(Curve C, Face F)
        {
            return C.Tessellate()
                .Select(p => F.Project(p))
                .Min(ir => ir.UVPoint.U);
        }

        public static double MinX(Curve C, Transform Tinv)
        {
            return C.Tessellate()
                .Select(p => Tinv.OfPoint(p))
                .Min(p => p.X);
        }

        public static EdgeArray OuterLoop(Face F)
        {
            EdgeArray eaMin = null;
            var loops = F.EdgeLoops;
            var uMin = double.MaxValue;
            foreach (EdgeArray a in loops)
            {
                var uMin2 = double.MaxValue;
                foreach (Edge e in a)
                {
                    var min = MinU(e.AsCurve(), F);
                    if (min < uMin2) uMin2 = min;
                }

                if (uMin2 < uMin)
                {
                    uMin = uMin2;
                    eaMin = a;
                }
            }

            return eaMin;
        }

        public static EdgeArray PlanarFaceOuterLoop(Face F)
        {
            var face = F as PlanarFace;
            if (face == null) return null;
            var T = Transform.Identity;
            T.BasisZ = face.FaceNormal;
            T.BasisX = face.XVector;
            T.BasisY = face.YVector;
            T.Origin = face.Origin;
            var Tinv = T.Inverse;

            EdgeArray eaMin = null;
            var loops = F.EdgeLoops;
            var uMin = double.MaxValue;
            foreach (EdgeArray a in loops)
            {
                var uMin2 = double.MaxValue;
                foreach (Edge e in a)
                {
                    var min = MinX(e.AsCurve(), Tinv);
                    if (min < uMin2) uMin2 = min;
                }

                if (uMin2 < uMin)
                {
                    uMin = uMin2;
                    eaMin = a;
                }
            }

            return eaMin;
        }

        /// <summary>
        ///     Eliminate the Z coordinate.
        /// </summary>
        public static UV Flatten(XYZ point)
        {
            return new UV(point.X, point.Y);
        }

        /// <summary>
        ///     Eliminate the Z coordinate.
        /// </summary>
        public static List<UV> Flatten(List<XYZ> polygon)
        {
            var z = polygon[0].Z;
            var a = new List<UV>(polygon.Count);
            foreach (var p in polygon)
            {
                Debug.Assert(IsEqual(p.Z, z),
                    "expected horizontal polygon");
                a.Add(Flatten(p));
            }

            return a;
        }

        /// <summary>
        ///     Eliminate the Z coordinate.
        /// </summary>
        public static List<List<UV>> Flatten(List<List<XYZ>> polygons)
        {
            var z = polygons[0][0].Z;
            var a = new List<List<UV>>(polygons.Count);
            foreach (var polygon in polygons)
            {
                Debug.Assert(IsEqual(polygon[0].Z, z),
                    "expected horizontal polygons");
                a.Add(Flatten(polygon));
            }

            return a;
        }
    }
}
