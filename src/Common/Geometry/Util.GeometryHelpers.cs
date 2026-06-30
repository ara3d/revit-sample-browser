// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated geometry helpers

        public static IEnumerable<Solid> GetSolidsFromGeometry(
            IEnumerable<GeometryObject> geometryElement)
        {
            foreach (var geometry in geometryElement)
            {
                var solid = geometry as Solid;
                if (solid != null)
                    yield return solid;

                var instance = geometry as GeometryInstance;
                if (instance != null)
                    foreach (var instanceSolid in GetSolidsFromGeometry(
                        instance.GetInstanceGeometry()))
                        yield return instanceSolid;

                var element = geometry as GeometryElement;
                if (element != null)
                    foreach (var elementSolid in GetSolidsFromGeometry(element))
                        yield return elementSolid;
            }
        }

        public static IEnumerable<Solid> GetSolidsFromElement(
            Element element)
        {
            var geometry = element
                .get_Geometry(new Options
                {
                    ComputeReferences = true,
                    IncludeNonVisibleObjects = true
                });

            if (geometry == null)
                return Enumerable.Empty<Solid>();

            return GetSolidsFromGeometry(geometry)
                .Where(x => x.Volume > 0);
        }

        public static List<Solid> GetElemSolids(GeometryElement geomElem)
        {
            if (geomElem == null)
                return new List<Solid>();

            return GetSolidsFromGeometry(geomElem)
                .Where(s => s.Faces.Size > 0)
                .ToList();
        }

        /// <summary>
        ///     Use the formula
        ///     area = sign * 0.5 * sum( xi * ( yi+1 - yi-1 ) )
        ///     to determine the winding direction (clockwise
        ///     or counter) and area of a 2D polygon.
        ///     Cf. also GetPolygonPlane.
        /// </summary>
        public static double GetSignedPolygonArea(List<UV> p)
        {
            var n = p.Count;
            var sum = p[0].U * (p[1].V - p[n - 1].V);
            for (var i = 1; i < n - 1; ++i) sum += p[i].U * (p[i + 1].V - p[i - 1].V);
            sum += p[n - 1].U * (p[0].V - p[n - 2].V);
            return 0.5 * sum;
        }

        /// <summary>
        ///     Eliminate the Z coordinate.
        /// </summary>
        public static UV Flatten(XYZ point)
        {
            return new UV(point.X, point.Y);
        }

        /// <summary>
        ///     Eliminate the Z coordinate.
        /// </summary>
        public static List<UV> Flatten(List<XYZ> polygon)
        {
            var z = polygon[0].Z;
            var a = new List<UV>(polygon.Count);
            foreach (var p in polygon)
            {
                Debug.Assert(IsEqual(p.Z, z),
                    "expected horizontal polygon");
                a.Add(Flatten(p));
            }

            return a;
        }

        /// <summary>
        ///     Eliminate the Z coordinate.
        /// </summary>
        public static List<List<UV>> Flatten(List<List<XYZ>> polygons)
        {
            var z = polygons[0][0].Z;
            var a = new List<List<UV>>(polygons.Count);
            foreach (var polygon in polygons)
            {
                Debug.Assert(IsEqual(polygon[0].Z, z),
                    "expected horizontal polygons");
                a.Add(Flatten(polygon));
            }

            return a;
        }

        /// <summary>
        ///     Return the plane properties of a given polygon,
        ///     i.e. the plane normal, area, and its distance
        ///     from the origin.
        /// </summary>
        public static bool GetPolygonPlane(
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

        public static double[] GetPolygonAreas(List<List<XYZ>> polygons)
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

        public static Transform GetTransformToZ(XYZ v)
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

        public static List<XYZ> ApplyTransform(
            List<XYZ> polygon,
            Transform t)
        {
            var n = polygon.Count;

            var polygonTransformed
                = new List<XYZ>(n);

            foreach (var p in polygon) polygonTransformed.Add(t.OfPoint(p));
            return polygonTransformed;
        }

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
        private static bool GetWallProfile(
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
        public static List<List<XYZ>> GetWallProfilePolygons(
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

        #endregion
    }
}
