// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    internal static class Util
    {
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
                RealString(p.X),
                RealString(p.Y),
                RealString(p.Z));
        }

        public static double SignedParallelipedVolume(
            XYZ a,
            XYZ b,
            XYZ c)
        {
            return a.CrossProduct(b).DotProduct(c);
        }

        public static bool IsRightHanded(XYZ a, XYZ b, XYZ c)
        {
            return 0 < SignedParallelipedVolume(a, b, c);
        }
    }
}
