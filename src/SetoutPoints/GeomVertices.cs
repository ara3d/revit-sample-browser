// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SetoutPoints by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/SetoutPoints

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.SetoutPoints.CS
{
    /// <summary>
    /// Geometry helpers for extracting solid corners from structural elements.
    /// </summary>
    internal static class GeomVertices
    {
        private static string RealString(double a) => a.ToString("0.##");

        public static string PointString(XYZ p) =>
            $"({RealString(p.X)},{RealString(p.Y)},{RealString(p.Z)})";

        private sealed class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            private const double SixteenthInchInFeet = 1.0 / (16.0 * 12.0);

            public bool Equals(XYZ p, XYZ q) =>
                p.IsAlmostEqualTo(q, SixteenthInchInFeet);

            public int GetHashCode(XYZ p) => PointString(p).GetHashCode();
        }

        private static void GetCorners(Dictionary<XYZ, int> corners, Solid solid)
        {
            foreach (Face f in solid.Faces)
            {
                foreach (EdgeArray ea in f.EdgeLoops)
                {
                    foreach (Edge e in ea)
                    {
                        var p = e.AsCurveFollowingFace(f).GetEndPoint(0);
                        if (!corners.ContainsKey(p))
                            corners[p] = 0;
                        ++corners[p];
                    }
                }
            }
        }

        public static Dictionary<XYZ, int> GetCorners(List<Solid> solids)
        {
            var corners = new Dictionary<XYZ, int>(new XyzEqualityComparer());
            foreach (var solid in solids)
                GetCorners(corners, solid);
            return corners;
        }

        public static List<Solid> GetSolids(Element e, Options opt, out Transform t)
        {
            var geo = e.get_Geometry(opt);
            var solids = new List<Solid>();
            GeometryInstance inst = null;
            t = Transform.Identity;

            foreach (var obj in geo)
            {
                if (obj is Solid solid && 0 < solid.Faces.Size)
                    solids.Add(solid);
                inst = obj as GeometryInstance;
            }

            if (solids.Count == 0 && inst != null)
            {
                geo = inst.GetSymbolGeometry();
                t = inst.Transform;

                foreach (var obj in geo)
                {
                    if (obj is Solid solid && 0 < solid.Faces.Size)
                        solids.Add(solid);
                }
            }

            return solids;
        }
    }
}
