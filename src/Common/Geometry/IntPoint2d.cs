// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;

namespace BuildingCoder
{
    internal class IntPoint2d : IComparable<IntPoint2d>
    {
        public IntPoint2d(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntPoint2d(UV p)
        {
            X = Util.FootToMmInt(p.U);
            Y = Util.FootToMmInt(p.V);
        }

        public IntPoint2d(XYZ p)
        {
            X = Util.FootToMmInt(p.X);
            Y = Util.FootToMmInt(p.Y);
        }

        public IntPoint2d(double x, double y)
        {
            X = Util.FootToMmInt(x);
            Y = Util.FootToMmInt(y);
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int CompareTo(IntPoint2d a)
        {
            var d = X - a.X;

            if (0 == d) d = Y - a.Y;
            return d;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public string ToString(
            bool onlySpaceSeparator)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1}"
                : "({0},{1})";

            return string.Format(format_string, X, Y);
        }

        public static IntPoint2d operator +(
            IntPoint2d a,
            IntPoint2d b)
        {
            return new IntPoint2d(
                a.X + b.X, a.Y + b.Y);
        }
    }
}