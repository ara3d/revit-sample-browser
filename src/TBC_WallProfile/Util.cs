using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_WallProfile sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Offset the generated boundary polygon loop
        ///     model lines outwards to separate them from
        ///     the wall edge, measured in feet.
        /// </summary>
        private const double _wallProfileOffset = 1.0;

        /// <summary>
        ///     Determine the elevation boundary profile
        ///     polygons of the exterior vertical planar
        ///     face of the given wall solid.
        /// </summary>
        internal static bool GetWallProfile(
            List<List<XYZ>> polygons,
            Solid solid,
            XYZ v,
            XYZ w)
        {
            double d, dmax = 0;
            PlanarFace outermost = null;
            var faces = solid.Faces;
            foreach (Face f in faces)
            {
                var pf = f as PlanarFace;
                if (null != pf
                    && IsVertical(pf)
                    && IsZero(v.DotProduct(pf.FaceNormal)))
                {
                    d = pf.Origin.DotProduct(w);
                    if (null == outermost
                        || dmax < d)
                    {
                        outermost = pf;
                        dmax = d;
                    }
                }
            }

            if (null != outermost)
            {
                var voffset = _wallProfileOffset * w;
                XYZ p, q = XYZ.Zero;
                bool first;
                int i, n;
                var loops = outermost.EdgeLoops;
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
                            var a = points[i];
                            a += voffset;
                            vertices.Add(a);
                        }
                    }

                    q += voffset;
                    Debug.Assert(q.IsAlmostEqualTo(vertices[0]),
                        "expected last end point to equal"
                        + " first start point");
                    polygons.Add(vertices);
                }
            }

            return null != outermost;
        }

        /// <summary>
        ///     Return all wall profile boundary loop polygons
        ///     for the given walls, offset out from the outer
        ///     face of the wall by a certain amount.
        /// </summary>
        internal static List<List<XYZ>> GetWallProfilePolygons(
            List<Element> walls,
            Options opt)
        {
            XYZ p, q, v, w;
            var polygons = new List<List<XYZ>>();

            foreach (Wall wall in walls)
            {
                var desc = ElementDescription(wall);

                if (wall.Location is not LocationCurve curve)
                    throw new Exception($"{desc}: No wall curve found.");
                p = curve.Curve.GetEndPoint(0);
                q = curve.Curve.GetEndPoint(1);
                v = q - p;
                v = v.Normalize();
                w = XYZ.BasisZ.CrossProduct(v).Normalize();
                if (wall.Flipped) w = -w;

                var geo = wall.get_Geometry(opt);

                foreach (var obj in geo)
                {
                    var solid = obj as Solid;
                    if (solid != null) GetWallProfile(polygons, solid, v, w);
                }
            }

            return polygons;
        }

        internal static void SetModelCurvesColor(
            ModelCurveArray modelCurves,
            View view,
            Color color)
        {
            foreach (var curve in modelCurves
                .Cast<ModelCurve>())
            {
                var overrides = view.GetElementOverrides(
                    curve.Id);

                overrides.SetProjectionLineColor(color);

                view.SetElementOverrides(curve.Id, overrides);
            }
        }
    }
}
