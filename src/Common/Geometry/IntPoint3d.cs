// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

﻿using System;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal class IntPoint3d : IComparable<IntPoint3d>
    {
        public IntPoint3d(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public IntPoint3d(UV p)
        {
            X = Util.FootToMmInt(p.U);
            Y = Util.FootToMmInt(p.V);
            Z = 0;
        }

        public IntPoint3d(XYZ p)
        {
            X = Util.FootToMmInt(p.X);
            Y = Util.FootToMmInt(p.Y);
            Z = Util.FootToMmInt(p.Z);
        }

        public IntPoint3d(double x, double y, double z)
        {
            X = Util.FootToMmInt(x);
            Y = Util.FootToMmInt(y);
            Z = Util.FootToMmInt(z);
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int CompareTo(IntPoint3d a)
        {
            var d = X - a.X;

            if (0 == d)
            {
                d = Y - a.Y;

                if (0 == d) d = Z - a.Z;
            }

            return d;
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public string ToString(
            bool onlySpaceSeparator)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1} {2}"
                : "({0},{1},{2})";

            return string.Format(format_string, X, Y, Z);
        }

        public static IntPoint3d operator +(
            IntPoint3d a,
            IntPoint3d b)
        {
            return new IntPoint3d(
                a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    }
}