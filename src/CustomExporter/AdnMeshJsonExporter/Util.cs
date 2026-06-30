// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    /// <summary>
    ///     Utility methods.
    /// </summary>
    internal static class Util
    {
        /// <summary>
        ///     Return a string for a real number formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        ///     Return a string for an XYZ point or vector with coordinates
        ///     formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
                RealString(p.X),
                RealString(p.Y),
                RealString(p.Z));
        }

        /// <summary>
        ///     Return the signed volume of the paralleliped spanned by the vectors a, b and c.
        /// </summary>
        public static double SignedParallelipedVolume(
            XYZ a,
            XYZ b,
            XYZ c)
        {
            return a.CrossProduct(b).DotProduct(c);
        }

        /// <summary>
        ///     Return true if the three vectors a, b and c form a right handed coordinate system.
        /// </summary>
        public static bool IsRightHanded(XYZ a, XYZ b, XYZ c)
        {
            return 0 < SignedParallelipedVolume(a, b, c);
        }
    }
}
