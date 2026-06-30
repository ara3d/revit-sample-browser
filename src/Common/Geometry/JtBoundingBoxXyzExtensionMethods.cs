// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BuildingCoder
{
    public static class JtBoundingBoxXyzExtensionMethods
    {
        public static void Clear(
            this BoundingBoxXYZ bb)
        {
            var infinity = double.MaxValue;
            bb.Min = new XYZ(infinity, infinity, infinity);
            bb.Max = -bb.Min;
        }

        public static void ExpandToContain(
            this BoundingBoxXYZ bb,
            XYZ p)
        {
            bb.Min = new XYZ(Math.Min(bb.Min.X, p.X),
                Math.Min(bb.Min.Y, p.Y),
                Math.Min(bb.Min.Z, p.Z));

            bb.Max = new XYZ(Math.Max(bb.Max.X, p.X),
                Math.Max(bb.Max.Y, p.Y),
                Math.Max(bb.Max.Z, p.Z));
        }

        public static void ExpandToContain(
            this BoundingBoxXYZ bb,
            IEnumerable<XYZ> pts)
        {
            bb.ExpandToContain(new XYZ(
                pts.Min<XYZ, double>(p => p.X),
                pts.Min<XYZ, double>(p => p.Y),
                pts.Min<XYZ, double>(p => p.Z)));

            bb.ExpandToContain(new XYZ(
                pts.Max<XYZ, double>(p => p.X),
                pts.Max<XYZ, double>(p => p.Y),
                pts.Max<XYZ, double>(p => p.Z)));
        }

        public static void ExpandToContain(
            this BoundingBoxXYZ bb,
            BoundingBoxXYZ other)
        {
            bb.ExpandToContain(other.Min);
            bb.ExpandToContain(other.Max);
        }
    }
}
