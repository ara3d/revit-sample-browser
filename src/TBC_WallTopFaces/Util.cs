using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_WallTopFaces sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Super-simple test whether a face is planar
        ///     and its normal vector points upwards.
        /// </summary>
        internal static bool IsTopPlanarFace(Face f)
        {
            return f is PlanarFace face
                   && PointsUpwards(face.FaceNormal);
        }

        /// <summary>
        ///     Simple test whether a given face normal vector
        ///     points upwards in the middle of the face.
        /// </summary>
        internal static bool IsTopFace(Face f)
        {
            var b = f.GetBoundingBox();
            var p = b.Min;
            var q = b.Max;
            var midpoint = p + 0.5 * (q - p);
            var normal = f.ComputeNormal(midpoint);
            return PointsUpwards(normal);
        }
    }
}
