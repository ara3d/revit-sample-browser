using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NestedInstanceGeo sample.</summary>
    internal static partial class Util
    {
        public static void GetVertices(List<XYZ> vertices, Solid s)
        {
            Debug.Assert(0 < s.Edges.Size,
                "expected a non-empty solid");

            var a
                = new Dictionary<XYZ, int>(
                    new NestedInstanceGeoXyzEqualityComparer());

            foreach (Face f in s.Faces)
            {
                var m = f.Triangulate();
                foreach (var p in m.Vertices)
                    if (!a.ContainsKey(p))
                        a.Add(p, 1);
                    else
                        ++a[p];
            }

            var keys = new List<XYZ>(a.Keys);

            Debug.Assert(8 == keys.Count,
                "expected eight vertices for a rectangular column");

            keys.Sort((p, q) => Compare(p, q));

            foreach (var p in keys)
            {
                Debug.Assert(3 == a[p],
                    "expected every vertex of solid to appear in exactly three faces");

                vertices.Add(p);
            }
        }

        /// <summary>
        ///     Define equality between XYZ objects, ensuring
        ///     that almost equal points compare equal.
        /// </summary>
        private class NestedInstanceGeoXyzEqualityComparer : IEqualityComparer<XYZ>
        {
            public bool Equals(XYZ p, XYZ q)
            {
                return p.IsAlmostEqualTo(q);
            }

            public int GetHashCode(XYZ p)
            {
                return PointString(p).GetHashCode();
            }
        }
    }
}
