// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Geometrical Comparison

        /// <summary>
        ///     Default tolerance used to add fuzz
        ///     for real number equality detection
        /// </summary>
        public const double _eps = 1.0e-9;

        /// <summary>
        ///     Default tolerance used to add fuzz
        ///     for real number equality detection
        /// </summary>
        public static double Eps => _eps;

        public static double MinLineLength => _eps;

        public static double TolPointOnPlane => _eps;

        /// <summary>
        ///     Predicate to determine whether the given
        ///     real number should be considered equal to
        ///     zero, adding fuzz according to the specified
        ///     tolerance
        /// </summary>
        public static bool IsZero(
            double a,
            double tolerance = _eps)
        {
            return tolerance > Math.Abs(a);
        }

        /// <summary>
        ///     Predicate to determine whether the two given
        ///     real numbers should be considered equal, adding
        ///     fuzz according to the specified tolerance
        /// </summary>
        public static bool IsEqual(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsZero(b - a, tolerance);
        }

        /// <summary>
        ///     Predicate to determine whether a is
        ///     smaller than or equal to b, adding fuzz
        ///     according to the specified tolerance
        /// </summary>
        public static bool IsLessOrEqual(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsEqual(a, b, tolerance)
                   || a < b;
        }

        /// <summary>
        ///     Comparison method for two real numbers
        ///     returning 0 if they are to be considered equal,
        ///     -1 if the first is smaller and +1 otherwise
        /// </summary>
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

        /// <summary>
        ///     Comparison method for two XYZ objects
        ///     returning 0 if they are to be considered equal,
        ///     -1 if the first is smaller and +1 otherwise
        /// </summary>
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

        /// <summary>
        ///     Implement a comparison operator for lines
        ///     in the XY plane useful for sorting into
        ///     groups of parallel lines.
        /// </summary>
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

                var da = (qa.X * pa.Y - qa.Y * pa.Y)
                         / va.GetLength();

                var db = (qb.X * pb.Y - qb.Y * pb.Y)
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

        /// <summary>
        ///     Predicate to test whether two points or
        ///     vectors can be considered equal with the
        ///     given tolerance.
        /// </summary>
        public static bool IsEqual(
            XYZ p,
            XYZ q,
            double tolerance = _eps)
        {
            return 0 == Compare(p, q, tolerance);
        }

        /// <summary>
        ///     Return true if the given bounding box bb
        ///     contains the given point p in its interior.
        /// </summary>
        public static bool BoundingBoxXyzContains(
            BoundingBoxXYZ bb,
            XYZ p)
        {
            return 0 < Compare(bb.Min, p)
                   && 0 < Compare(p, bb.Max);
        }

        /// <summary>
        ///     Return true if the vectors v and w
        ///     are non-zero and perpendicular.
        /// </summary>
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

        /// <summary>
        ///     Return true if the vectors p and Q are parallel, 
        ///     or at least one of them is zero length.
        /// </summary>
        public static bool IsParallel(XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }

        /// <summary>
        ///     Predicate returning true if three given points are collinear
        /// </summary>
        public static bool AreCollinear(XYZ p, XYZ q, XYZ r)
        {
            var v = q - p;
            var w = r - p;
            return IsParallel(v, w);
        }

        /// <summary>
        ///     Predicate returning true if two given lines are collinear
        /// </summary>
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

        /// <summary>
        ///     Minimum slope for a vector to be considered
        ///     to be pointing upwards. Slope is simply the
        ///     relationship between the vertical and
        ///     horizontal components.
        /// </summary>
        private const double _minimumSlope = 0.3;

        /// <summary>
        ///     Return true if the Z coordinate of the
        ///     given vector is positive and the slope
        ///     is larger than the minimum limit.
        /// </summary>
        public static bool PointsUpwards(XYZ v)
        {
            var horizontalLength = v.X * v.X + v.Y * v.Y;
            var verticalLength = v.Z * v.Z;

            return 0 < v.Z
                   && _minimumSlope
                   < verticalLength / horizontalLength;

            //return _eps < v.Normalize().Z;
            //return _eps < v.Normalize().Z && IsVertical( v.Normalize(), tolerance );
        }

        /// <summary>
        ///     Return the maximum value from an array of real numbers.
        /// </summary>
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
