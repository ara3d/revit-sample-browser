using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static bool IsTopPlanarFace(Face f)
        {
            return f is PlanarFace face
                   && PointsUpwards(face.FaceNormal);
        }

        internal static bool IsTopFace(Face f)
        {
            var b = f.GetBoundingBox();
            var p = b.Min;
            var q = b.Max;
            var midpoint = p + (0.5 * (q - p));
            var normal = f.ComputeNormal(midpoint);
            return PointsUpwards(normal);
        }
    }
}
