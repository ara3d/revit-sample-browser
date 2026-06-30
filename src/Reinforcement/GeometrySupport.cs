// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using RebarGeomHelper = Ara3D.RevitSampleBrowser.Common.Structural.RebarGeometry;
namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    using GeoInstance = GeometryInstance;

    public class GeometrySupport
    {
        protected readonly Line DrivingLine;

        protected readonly XYZ DrivingVector;

        protected List<Line> Edges = [];

        protected readonly List<XYZ> Points = [];

        protected Solid Solid;

        protected readonly Transform m_transform;

        public GeometrySupport(FamilyInstance element, Options geoOptions)
        {
            var geoElement = element.get_Geometry(new Options());
            var objects = geoElement.GetEnumerator();
            if (null == geoElement || !objects.MoveNext())
                throw new Exception("Can't get the geometry of selected element.");

            var swProfile = element.GetSweptProfile();
            if (swProfile == null || swProfile.GetDrivingCurve() is not Line)
                throw new Exception("The selected element driving curve is not a line.");

            var line = swProfile.GetDrivingCurve() as Line;
            if (null != line)
            {
                DrivingLine = line; // driving path
                DrivingVector = XyzMath.SubXyz(line.GetEndPoint(1), line.GetEndPoint(0));
            }

            objects.Reset();
            while (objects.MoveNext())
            {
                var geoObject = objects.Current;

                var instance = geoObject as GeoInstance;
                if (null != instance)
                {
                    var objects1 = instance.SymbolGeometry.GetEnumerator();
                    while (objects1.MoveNext())
                    {
                        var o = objects1.Current;

                        var solid = o as Solid;

                        // do some checks.
                        if (null == solid) continue;
                        if (0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;

                        Solid = solid;
                        m_transform = instance.Transform;

                        if (!GetSweptProfile(solid)) throw new Exception("Can't get the swept profile curves.");
                        break;
                    }
                }
            }

            // do some checks about profile curves information
            if (null == Edges) throw new Exception("Can't get the geometry edge information.");
            if (4 != Points.Count) throw new Exception("The sample only work for rectangular beams or columns.");
        }

        protected XYZ Transform(XYZ point)
        {
            // only invoke the TransformPoint() method.
            return XyzMath.TransformPoint(point, m_transform);
        }

        protected double GetDrivingLineLength()
        {
            return XyzMath.GetLength(DrivingVector);
        }

        protected List<XYZ> GetRelatedVectors(XYZ point)
        {
            // Initialize the return vector list.
            List<XYZ> vectors = new();

            // And get the vector from this point to another point
            foreach (var line in Edges)
            {
                if (XyzMath.IsEqual(point, line.GetEndPoint(0)))
                {
                    var vector = XyzMath.SubXyz(line.GetEndPoint(1), line.GetEndPoint(0));
                    vectors.Add(vector);
                }

                if (XyzMath.IsEqual(point, line.GetEndPoint(1)))
                {
                    var vector = XyzMath.SubXyz(line.GetEndPoint(0), line.GetEndPoint(1));
                    vectors.Add(vector);
                }
            }

            // only two vector(direction) should be found
            return 2 != vectors.Count ? throw new Exception("a point on swept profile should have only two direction.") : vectors;
        }

        protected List<XYZ> OffsetPoints(double offset)
        {
            // Initialize the offset point list.
            List<XYZ> points = new();

            // Get all points of the swept profile, and offset it in two related direction
            foreach (var point in Points)
            {
                var directions = GetRelatedVectors(point);
                var firstDir = directions[0];
                var secondDir = directions[1];

                // offset the point in two direction
                var movedPoint = XyzMath.OffsetPoint(point, firstDir, offset);
                movedPoint = XyzMath.OffsetPoint(movedPoint, secondDir, offset);

                points.Add(movedPoint);
            }

            return points;
        }

        private bool GetSweptProfile(Solid solid)
        {
            var sweptFace = GetSweptProfileFace(solid);
            // do some checks
            if (null == sweptFace || 1 != sweptFace.EdgeLoops.Size) return false;

            foreach (var point in sweptFace.Triangulate().Vertices)
            {
                Points.Add(Transform(point));
            }

            Edges = ChangeEdgeToLine(sweptFace.EdgeLoops.get_Item(0));

            // do some checks
            return null != Edges;
        }

        private Face GetSweptProfileFace(Solid solid)
        {
            XYZ refPoint = new(); // the point on swept profile
            foreach (Edge edge in solid.Edges)
            {
                var points = edge.Tessellate() as List<XYZ>;
                if (2 != points.Count) // make sure all edges are lines
                    throw new Exception("All edge should be line.");

                var first = Transform(points[0]); // start point of edge
                var second = Transform(points[1]); // end point of edge

                // some edges should be parallelled with the driving line,
                // and the start point of that edge should be the wanted point
                var edgeVector = XyzMath.SubXyz(second, first);
                if (XyzMath.IsSameDirection(edgeVector, DrivingVector))
                {
                    refPoint = first;
                    break;
                }

                if (XyzMath.IsOppositeDirection(edgeVector, DrivingVector))
                {
                    refPoint = second;
                    break;
                }
            }

            // Find swept profile(face)
            Face sweptFace = null; // define the swept face
            foreach (Face face in solid.Faces)
            {
                if (null != sweptFace) break;
                // the swept face should be perpendicular with the driving line
                if (!RebarGeomHelper.IsVertical(face, DrivingLine, m_transform, null)) continue;
                // use the gotted point to get the swept face
                foreach (var point in face.Triangulate().Vertices)
                {
                    var pnt = Transform(point); // all points in solid should be transform
                    if (XyzMath.IsEqual(refPoint, pnt))
                    {
                        sweptFace = face;
                        break;
                    }
                }
            }

            return sweptFace;
        }

        private List<Line> ChangeEdgeToLine(EdgeArray edges)
        {
            List<Line> edgeLines = new();

            // and changed the geometry information in line list
            foreach (Edge edge in edges)
            {
                var points = edge.Tessellate() as List<XYZ>;
                var first = Transform(points[0]);
                var second = Transform(points[1]);

                edgeLines.Add(Line.CreateBound(first, second));
            }

            return edgeLines;
        }
    }
}
