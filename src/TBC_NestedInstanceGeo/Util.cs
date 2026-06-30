using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NestedInstanceGeo sample.</summary>
    internal static partial class Util
    {
        public static void GetVertices(List<XYZ> vertices, Solid s)
        {
            Debug.Assert(0 < s.Edges.Size,
                "expected a non-empty solid");

            Dictionary<XYZ, int> a
                = new(
                    new XyzEqualityComparer());

            foreach (Face f in s.Faces)
            {
                var m = f.Triangulate();
                foreach (var p in m.Vertices)
                    if (!a.ContainsKey(p))
                        a.Add(p, 1);
                    else
                        ++a[p];
            }

            List<XYZ> keys = [.. a.Keys];

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
    }
}
