using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_DetailCurves sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return a point projected onto a plane defined by its normal.
        /// </summary>
        internal static XYZ ProjectPointOntoPlane(
            XYZ point,
            XYZ planeNormal)
        {
            var a = planeNormal.X;
            var b = planeNormal.Y;
            var c = planeNormal.Z;

            var dx = (b * b + c * c) * point.X - a * b * point.Y - a * c * point.Z;
            var dy = -(b * a) * point.X + (a * a + c * c) * point.Y - b * c * point.Z;
            var dz = -(c * a) * point.X - c * b * point.Y + (a * a + b * b) * point.Z;
            return new XYZ(dx, dy, dz);
        }
    }
}
