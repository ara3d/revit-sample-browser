// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Diagnostics;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Geometrical Comparison

        public const double _eps = 1.0e-9;

        public static double Eps => _eps;

        public static double MinLineLength => _eps;

        public static double TolPointOnPlane => _eps;

        public static bool IsZero(
            double a,
            double tolerance = _eps)
        {
            return tolerance > Math.Abs(a);
        }

        public static bool IsEqual(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsZero(b - a, tolerance);
        }

        public static bool IsLessOrEqual(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsEqual(a, b, tolerance)
                   || a < b;
        }

        public static int Compare(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsEqual(a, b, tolerance)
                ? 0
                : a < b
                    ? -1
                    : 1;
        }

        public static int Compare(
            XYZ p,
            XYZ q,
            double tolerance = _eps)
        {
            var d = Compare(p.X, q.X, tolerance);

            if (0 == d)
            {
                d = Compare(p.Y, q.Y, tolerance);

                if (0 == d) d = Compare(p.Z, q.Z, tolerance);
            }

            return d;
        }

        public static int Compare(Line a, Line b)
        {
            var pa = a.GetEndPoint(0);
            var qa = a.GetEndPoint(1);
            var pb = b.GetEndPoint(0);
            var qb = b.GetEndPoint(1);
            var va = qa - pa;
            var vb = qb - pb;

            // Compare angle in the XY plane

            var ang_a = Math.Atan2(va.Y, va.X);
            var ang_b = Math.Atan2(vb.Y, vb.X);

            var d = Compare(ang_a, ang_b);

            if (0 == d)
            {
                // Compare distance of unbounded line to origin

                var da = ((qa.X * pa.Y) - (qa.Y * pa.Y))
                         / va.GetLength();

                var db = ((qb.X * pb.Y) - (qb.Y * pb.Y))
                         / vb.GetLength();

                d = Compare(da, db);

                if (0 == d)
                {
                    // Compare distance of start point to origin

                    d = Compare(pa.GetLength(), pb.GetLength());

                    if (0 == d)
                        // Compare distance of end point to origin

                        d = Compare(qa.GetLength(), qb.GetLength());
                }
            }
            return d;
        }

        public static int Compare(Plane a, Plane b)
        {
            var d = Compare(a.Normal, b.Normal);

            if (0 == d)
            {
                d = Compare(a.SignedDistanceTo(XYZ.Zero),
                    b.SignedDistanceTo(XYZ.Zero));

                if (0 == d)
                    d = Compare(a.XVec.AngleOnPlaneTo(
                        b.XVec, b.Normal), 0);
            }
            return d;
        }

        public static bool IsEqual(
            XYZ p,
            XYZ q,
            double tolerance = _eps)
        {
            return 0 == Compare(p, q, tolerance);
        }

        public static bool BoundingBoxXyzContains(
            BoundingBoxXYZ bb,
            XYZ p)
        {
            return 0 < Compare(bb.Min, p)
                   && 0 < Compare(p, bb.Max);
        }

        private static bool IsPerpendicular(XYZ v, XYZ w)
        {
            var a = v.GetLength();
            var b = v.GetLength();
            var c = Math.Abs(v.DotProduct(w));
            return _eps < a
                   && _eps < b
                   && _eps > c;
            // c * c < _eps * a * b
        }

        public static bool IsParallel(XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }

        public static bool AreCollinear(XYZ p, XYZ q, XYZ r)
        {
            var v = q - p;
            var w = r - p;
            return IsParallel(v, w);
        }

        public static bool IsCollinear(Line a, Line b)
        {
            var v = a.Direction;
            var w = b.Origin - a.Origin;
            return IsParallel(v, b.Direction)
                   && IsParallel(v, w);
        }

        public static bool IsHorizontal(XYZ v)
        {
            return IsZero(v.Z);
        }

        public static bool IsVertical(XYZ v)
        {
            return IsZero(v.X) && IsZero(v.Y);
        }

        public static bool IsVertical(XYZ v, double tolerance)
        {
            return IsZero(v.X, tolerance)
                   && IsZero(v.Y, tolerance);
        }

        public static bool IsHorizontal(Edge e)
        {
            var p = e.Evaluate(0);
            var q = e.Evaluate(1);
            return IsHorizontal(q - p);
        }

        public static bool IsHorizontal(PlanarFace f)
        {
            return IsVertical(f.FaceNormal);
        }

        public static bool IsVertical(PlanarFace f)
        {
            return IsHorizontal(f.FaceNormal);
        }

        public static bool IsVertical(CylindricalFace f)
        {
            return IsVertical(f.Axis);
        }

        private const double _minimumSlope = 0.3;

        public static bool PointsUpwards(XYZ v)
        {
            var horizontalLength = (v.X * v.X) + (v.Y * v.Y);
            var verticalLength = v.Z * v.Z;

            return 0 < v.Z
                   && _minimumSlope
                   < verticalLength / horizontalLength;

            //return _eps < v.Normalize().Z;
            //return _eps < v.Normalize().Z && IsVertical( v.Normalize(), tolerance );
        }

        public static double Max(double[] a)
        {
            Debug.Assert(1 == a.Rank, "expected one-dimensional array");
            Debug.Assert(0 == a.GetLowerBound(0), "expected zero-based array");
            Debug.Assert(0 < a.GetUpperBound(0), "expected non-empty array");
            var max = a[0];
            for (var i = 1; i <= a.GetUpperBound(0); ++i)
                if (max < a[i])
                    max = a[i];
            return max;
        }

        #endregion
    }
}
