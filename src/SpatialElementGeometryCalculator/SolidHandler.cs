// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

using Autodesk.Revit.DB;
using BuildingCoder;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    internal class SolidHandler
    {
        public double GetWallAsOpeningArea(
            Element elemOpening,
            Solid solidRoom)
        {
            var doc = elemOpening.Document;
            var wallAsOpening = elemOpening as Wall;

            var options = doc.Application.Create.NewGeometryOptions();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;

            List<Wall> walls = new()
            { wallAsOpening };

            var polygons = GetWallProfilePolygons(walls, options);
            var solidProfile = XYZAsCurveloop(polygons.First());

            var solidOpening = GeometryCreationUtilities
                .CreateExtrusionGeometry(solidProfile,
                    wallAsOpening.Orientation, 1);

            var intersectSolid = BooleanOperationsUtils
                .ExecuteBooleanOperation(solidOpening,
                    solidRoom, BooleanOperationsType.Intersect);

            if (intersectSolid.Faces.Size.Equals(0))
            {
                solidOpening = GeometryCreationUtilities
                    .CreateExtrusionGeometry(solidProfile,
                        wallAsOpening.Orientation.Negate(), 1);

                intersectSolid = BooleanOperationsUtils
                    .ExecuteBooleanOperation(solidOpening,
                        solidRoom, BooleanOperationsType.Intersect);
            }

            if (DebugHandler.EnableSolidUtilityVolumes)
            {
                using Transaction t = new(doc);
                t.Start("Test1");
                ShapeCreator.CreateDirectShape(doc,
                    intersectSolid, "namesolid");
                t.Commit();
            }

            var openingArea = GetLargestFaceArea(intersectSolid);

            LogCreator.LogEntry(";_______OPENINGAREA;"
                + elemOpening.Id.ToString() + ";"
                + elemOpening.Category.Name + ";"
                + elemOpening.Name + ";"
                + (openingArea * 0.09290304).ToString());

            return openingArea;
        }

        public IList<CurveLoop> XYZAsCurveloop(List<XYZ> polyPoints)
        {
            CurveLoop curveLoop = new();

            for (var i = 0; i < polyPoints.Count - 1; i++)
            {
                curveLoop.Append(Line.CreateBound(
                    polyPoints[i], polyPoints[i + 1]));
            }

            curveLoop.Append(Line.CreateBound(
                polyPoints[polyPoints.Count - 1], polyPoints[0]));

            IList<CurveLoop> curveLoops = [curveLoop];

            return curveLoops;
        }

        public static List<List<XYZ>> GetWallProfilePolygons(
            List<Wall> walls,
            Options opt)
        {
            List<List<XYZ>> polygons = new();

            foreach (var wall in walls)
            {
                if (wall.Location is not LocationCurve curve)
                {
                    return null;
                }

                var p = curve.Curve.GetEndPoint(0);
                var q = curve.Curve.GetEndPoint(1);
                var v = (q - p).Normalize();
                var w = XYZ.BasisZ.CrossProduct(v).Normalize();

                if (wall.Flipped) { w = -w; }

                var geo = wall.get_Geometry(opt);

                foreach (var obj in geo)
                {
                    if (obj is Solid solid)
                    {
                        GetProfile(polygons, solid, v, w);
                    }
                }
            }

            return polygons;
        }

        const double _offset = 0;

        static bool GetProfile(
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
                if (f is PlanarFace pf
                    && Util.IsVertical(pf)
                    && Util.IsZero(v.DotProduct(pf.FaceNormal)))
                {
                    d = pf.Origin.DotProduct(w);
                    if (null == outermost || dmax < d)
                    {
                        outermost = pf;
                        dmax = d;
                    }
                }
            }

            if (null != outermost)
            {
                var voffset = _offset * w;
                var loops = outermost.EdgeLoops;

                foreach (EdgeArray loop in loops)
                {
                    List<XYZ> vertices = new();
                    var first = true;
                    var q = XYZ.Zero;

                    foreach (Edge e in loop)
                    {
                        var points = e.Tessellate();
                        var p = points[0];

                        if (!first)
                        {
                            if (!p.IsAlmostEqualTo(q))
                            {
                                LogCreator.LogEntry("Expected "
                                    + "subsequent start point to equal "
                                    + "previous end point");
                            }
                        }

                        var n = points.Count;
                        q = points[n - 1];

                        for (var i = 0; i < n - 1; ++i)
                        {
                            var a = points[i];
                            a += voffset;
                            vertices.Add(a);
                        }
                    }

                    q += voffset;

                    if (!q.IsAlmostEqualTo(vertices[0]))
                    {
                        LogCreator.LogEntry("Expected last end "
                            + "point to equal first start point");
                    }

                    polygons.Add(vertices);
                }
            }

            return null != outermost;
        }

        public double GetLargestFaceArea(Solid intersectSolid)
        {
            var faceArray = intersectSolid.Faces;
            var maxFaceArea = 0.0;

            foreach (Face face in faceArray)
            {
                var a = face.Area;

                if (a > maxFaceArea)
                {
                    maxFaceArea = a;
                }
            }

            return maxFaceArea;
        }

        public Solid CreateSolidFromBoundingBox(
            Transform lcs,
            BoundingBoxXYZ boundingBoxXYZ,
            SolidOptions solidOptions)
        {
            if (boundingBoxXYZ == null
                || !boundingBoxXYZ.Enabled)
            {
                return null;
            }

            try
            {
                var bboxTransform = (lcs == null)
                    ? boundingBoxXYZ.Transform
                    : lcs.Multiply(boundingBoxXYZ.Transform);

                var profilePts = new XYZ[4];
                profilePts[0] = bboxTransform.OfPoint(boundingBoxXYZ.Min);
                profilePts[1] = bboxTransform.OfPoint(new XYZ(
                    boundingBoxXYZ.Max.X, boundingBoxXYZ.Min.Y, boundingBoxXYZ.Min.Z));
                profilePts[2] = bboxTransform.OfPoint(new XYZ(
                    boundingBoxXYZ.Max.X, boundingBoxXYZ.Max.Y, boundingBoxXYZ.Min.Z));
                profilePts[3] = bboxTransform.OfPoint(new XYZ(
                    boundingBoxXYZ.Min.X, boundingBoxXYZ.Max.Y, boundingBoxXYZ.Min.Z));

                var origExtrusionVector = new XYZ(
                    boundingBoxXYZ.Min.X, boundingBoxXYZ.Min.Y,
                    boundingBoxXYZ.Max.Z) - boundingBoxXYZ.Min;

                var extrusionVector = bboxTransform.OfVector(origExtrusionVector);
                var extrusionDistance = extrusionVector.GetLength();
                var extrusionDirection = extrusionVector.Normalize();

                CurveLoop baseLoop = new();

                for (var i = 0; i < 4; i++)
                {
                    baseLoop.Append(Line.CreateBound(
                        profilePts[i], profilePts[(i + 1) % 4]));
                }

                IList<CurveLoop> baseLoops = [baseLoop];

                return solidOptions == null
                    ? GeometryCreationUtilities
                        .CreateExtrusionGeometry(baseLoops,
                            extrusionDirection, extrusionDistance)
                    : GeometryCreationUtilities
                    .CreateExtrusionGeometry(baseLoops,
                        extrusionDirection, extrusionDistance,
                        solidOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}
