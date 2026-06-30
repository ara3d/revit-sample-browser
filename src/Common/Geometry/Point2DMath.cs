// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;

namespace Ara3D.RevitSampleBrowser.Common.Geometry
{
    public static class Point2DMath
    {
        public const float FloatEpsilon = 0.00001f;

        public static bool CompareDouble(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < 0.00001;
        }

        public static float Dot(PointF pnt1, PointF pnt2)
        {
            return (pnt1.X * pnt2.X) + (pnt1.Y * pnt2.Y);
        }

        public static PointF Multiply(float f, PointF pnt)
        {
            return new PointF(f * pnt.X, f * pnt.Y);
        }

        public static PointF Add(PointF f1, PointF f2)
        {
            return new PointF(f1.X + f2.X, f1.Y + f2.Y);
        }

        public static PointF Subtract(PointF f1, PointF f2)
        {
            return new PointF(f1.X - f2.X, f1.Y - f2.Y);
        }

        public static int FindIntersection(float u0, float u1, float v0, float v1, ref float[] w)
        {
            if (u1 < v0 || u0 > v1) return 0;
            if (u1 == v0) { w[0] = u1; return 1; }
            if (u0 == v1) { w[0] = u0; return 1; }
            if (u1 > v0)
            {
                if (u0 < v1)
                {
                    w[0] = u0 < v0 ? v0 : u0;
                    w[1] = u1 > v1 ? v1 : u1;
                    return 2;
                }

                w[0] = u0;
                return 1;
            }

            w[0] = u1;
            return 1;
        }

        public static float GetMin(float f1, float f2)
        {
            return f1 < f2 ? f1 : f2;
        }

        public static float GetMax(float f1, float f2)
        {
            return f1 > f2 ? f1 : f2;
        }
    }
}
