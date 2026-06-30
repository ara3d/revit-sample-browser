using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static BoundingBoxXYZ RotateBoundingBox(
            BoundingBoxXYZ b,
            Transform t)
        {
            var height = b.Max.Z - b.Min.Z;

            var corners = GetBottomCorners(b);

            var cornersTransformed
                = corners.Select(
                        p => new XyzComparable(t.OfPoint(p)))
                    .ToArray();

            b.Min = cornersTransformed.Min();
            b.Max = cornersTransformed.Max();
            b.Max += height * XYZ.BasisZ;

            return b;
        }
        private class XyzComparable : XYZ, IComparable<XYZ>
        {
            public XyzComparable(XYZ a)
                : base(a.X, a.Y, a.Z)
            {
            }

            int IComparable<XYZ>.CompareTo(XYZ a)
            {
                return Compare(this, a);
            }
        }
    }
}
