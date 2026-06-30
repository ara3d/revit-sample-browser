#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SlabBoundary sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Offset the generated boundary polygon loop
        ///     model lines downwards to separate them from
        ///     the slab edge.
        /// </summary>
        private const double SlabBoundary_offset = 0.1;

        /// <summary>
        ///     Determine the boundary polygons of the lowest
        ///     horizontal planar face of the given solid.
        /// </summary>
        private static bool GetBoundary(
            List<List<XYZ>> polygons,
            Solid solid)
        {
            PlanarFace lowest = null;
            var faces = solid.Faces;
            foreach (Face f in faces)
            {
                var pf = f as PlanarFace;
                if (null != pf && IsHorizontal(pf))
                    if (null == lowest
                        || pf.Origin.Z < lowest.Origin.Z)
                        lowest = pf;
            }

            if (null != lowest)
            {
                XYZ p, q = XYZ.Zero;
                bool first;
                int i, n;
                var loops = lowest.EdgeLoops;
                foreach (EdgeArray loop in loops)
                {
                    var vertices = new List<XYZ>();
                    first = true;
                    foreach (Edge e in loop)
                    {
                        var points = e.Tessellate();
                        p = points[0];
                        if (!first)
                            Debug.Assert(p.IsAlmostEqualTo(q),
                                "expected subsequent start point"
                                + " to equal previous end point");
                        n = points.Count;
                        q = points[n - 1];
                        for (i = 0; i < n - 1; ++i)
                        {
                            var v = points[i];
                            v -= SlabBoundary_offset * XYZ.BasisZ;
                            vertices.Add(v);
                        }
                    }

                    q -= SlabBoundary_offset * XYZ.BasisZ;
                    Debug.Assert(q.IsAlmostEqualTo(vertices[0]),
                        "expected last end point to equal"
                        + " first start point");
                    polygons.Add(vertices);
                }
            }

            return null != lowest;
        }

        /// <summary>
        ///     Return all floor slab boundary loop polygons
        ///     for the given floors, offset downwards from the
        ///     bottom floor faces by a certain amount.
        /// </summary>
        public static List<List<XYZ>> GetFloorBoundaryPolygons(
            List<Element> floors,
            Options opt)
        {
            var polygons = new List<List<XYZ>>();

            foreach (Floor floor in floors)
            {
                var geo = floor.get_Geometry(opt);

                foreach (var obj in geo)
                {
                    var solid = obj as Solid;
                    if (solid != null) GetBoundary(polygons, solid);
                }
            }

            return polygons;
        }
    }
}
