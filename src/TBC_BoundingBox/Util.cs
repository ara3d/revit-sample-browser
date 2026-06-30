using System;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_BoundingBox sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return a rotated bounding box around
        ///     the origin in the XY plane.
        /// </summary>
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

        /// <summary>
        ///     XYZ wrapper class implementing IComparable.
        /// </summary>
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
