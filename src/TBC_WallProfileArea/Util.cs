using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_WallProfileArea sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return the plane properties of a given polygon,
        ///     i.e. the plane normal, area, and its distance
        ///     from the origin.
        /// </summary>
        internal static bool GetPolygonPlane(
            List<XYZ> polygon,
            out XYZ normal,
            out double dist,
            out double area)
        {
            normal = XYZ.Zero;
            dist = area = 0.0;
            var n = null == polygon ? 0 : polygon.Count;
            var rc = 2 < n;
            switch (n)
            {
                case 3:
                {
                    var a = polygon[0];
                    var b = polygon[1];
                    var c = polygon[2];
                    var v = b - a;
                    normal = v.CrossProduct(c - a);
                    dist = normal.DotProduct(a);
                    break;
                }
                case 4:
                {
                    var a = polygon[0];
                    var b = polygon[1];
                    var c = polygon[2];
                    var d = polygon[3];

                    normal = (a - c).CrossProduct(b - d);

                    dist = 0.25 *
                           (normal.X * (a.X + b.X + c.X + d.X)
                            + normal.Y * (a.Y + b.Y + c.Y + d.Y)
                            + normal.Z * (a.Z + b.Z + c.Z + d.Z));
                    break;
                }
                case > 4:
                {
                    XYZ a;
                    var b = polygon[n - 2];
                    var c = polygon[n - 1];
                    var s = XYZ.Zero;

                    for (var i = 0; i < n; ++i)
                    {
                        a = b;
                        b = c;
                        c = polygon[i];

                        normal = new XYZ(
                            normal.X + b.Y * (c.Z - a.Z),
                            normal.Y + b.Z * (c.X - a.X),
                            normal.Z + b.X * (c.Y - a.Y));

                        s += c;
                    }

                    dist = s.DotProduct(normal) / n;
                    break;
                }
            }

            if (rc)
            {
                var length = normal.GetLength();
                rc = !IsZero(length);
                Debug.Assert(rc);

                if (rc)
                {
                    normal /= length;
                    dist /= length;
                    area = 0.5 * length;
                }
            }

            return rc;
        }

        internal static double[] GetPolygonAreas(List<List<XYZ>> polygons)
        {
            int i = 0, n = polygons.Count;
            var areas = new double[n];
            double dist, area;
            XYZ normal;
            foreach (var polygon in polygons)
                if (GetPolygonPlane(polygon, out normal, out dist, out area))
                    areas[i++] = area;
            return areas;
        }

        internal static Transform GetTransformToZ(XYZ v)
        {
            Transform t;

            var a = XYZ.BasisZ.AngleTo(v);

            if (IsZero(a))
            {
                t = Transform.Identity;
            }
            else
            {
                var axis = IsEqual(a, Math.PI)
                    ? XYZ.BasisX
                    : v.CrossProduct(XYZ.BasisZ);

                t = Transform.CreateRotation(axis, a);
            }

            return t;
        }

        internal static List<XYZ> ApplyTransform(
            List<XYZ> polygon,
            Transform t)
        {
            var n = polygon.Count;

            var polygonTransformed
                = new List<XYZ>(n);

            foreach (var p in polygon) polygonTransformed.Add(t.OfPoint(p));
            return polygonTransformed;
        }
    }
}
