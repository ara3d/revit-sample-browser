#region Namespaces

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewCrossFitting sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return the normalised direction of the given pipe.
        /// </summary>
        internal static XYZ GetPipeDirection(Pipe pipe)
        {
            var c = pipe.GetCurve();
            var dir = c.GetEndPoint(1) - c.GetEndPoint(1);
            dir = dir.Normalize();
            return dir;
        }

        /// <summary>
        ///     Are the two given pipes parallel?
        /// </summary>
        internal static bool IsPipeParallel(Pipe p1, Pipe p2)
        {
            var c1 = p1.GetCurve() as Line;
            var c2 = p2.GetCurve() as Line;
            return Math.Sin(c1.Direction.AngleTo(
                c2.Direction)) < 0.01;
        }
    }
}
