// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using RebarGeomHelper = Ara3D.RevitSampleBrowser.Common.Structural.RebarGeometry;
namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Geom
{
    using GeoInstance = GeometryInstance;

    public class GeometrySupport
    {
        private readonly Line m_drivingLine;
        private readonly XYZ m_drivingVector;
        private List<Line> m_edges = [];
        private readonly Transform m_transform;

        public GeometrySupport(FamilyInstance element)
        {
            var geoElement = element.get_Geometry(new Options());
            using (var objects = geoElement.GetEnumerator())
            {
                if (null == geoElement || !objects.MoveNext())
                    throw new Exception("Can't get the geometry of selected element.");

                var swProfile = element.GetSweptProfile();
                if (swProfile == null || swProfile.GetDrivingCurve() is not Line line)
                    throw new Exception("The selected element driving curve is not a line.");

                m_drivingLine = line;
                m_drivingVector = XyzMath.SubXyz(line.GetEndPoint(1), line.GetEndPoint(0));
                DrivingLength = m_drivingVector.GetLength();

                objects.Reset();
                while (objects.MoveNext())
                {
                    if (objects.Current is not GeoInstance instance)
                        continue;

                    foreach (var o in instance.SymbolGeometry)
                    {
                        if (o is not Solid solid || solid.Faces.Size == 0 || solid.Edges.Size == 0)
                            continue;

                        m_transform = instance.Transform;
                        if (!GetSweptProfile(solid))
                            throw new Exception("Can't get the swept profile curves.");
                        break;
                    }
                }
            }

            if (null == m_edges)
                throw new Exception("Can't get the geometry edge information.");
            if (4 != ProfilePoints.Count)
                throw new Exception("The sample only works for rectangle beam or column.");
        }

        public List<XYZ> ProfilePoints { get; set; } = [];

        public double DrivingLength { get; }

        private XYZ Transform(XYZ point)
        {
            return XyzMath.TransformPoint(point, m_transform);
        }

        private List<XYZ> GetRelatedVectors(XYZ point)
        {
            List<XYZ> vectors = new();
            foreach (var edgeLine in m_edges)
            {
                if (XyzMath.IsEqual(point, edgeLine.GetEndPoint(0)))
                    vectors.Add(XyzMath.SubXyz(edgeLine.GetEndPoint(1), edgeLine.GetEndPoint(0)));
                if (XyzMath.IsEqual(point, edgeLine.GetEndPoint(1)))
                    vectors.Add(XyzMath.SubXyz(edgeLine.GetEndPoint(0), edgeLine.GetEndPoint(1)));
            }

            return 2 != vectors.Count ? throw new Exception("A point on swept profile should have only two directions.") : vectors;
        }

        public List<XYZ> OffsetPoints(double offset)
        {
            return ProfilePoints.ConvertAll(point =>
            {
                var directions = GetRelatedVectors(point);
                var movedPoint = XyzMath.OffsetPoint(point, directions[0], offset);
                return XyzMath.OffsetPoint(movedPoint, directions[1], offset);
            });
        }

        private bool GetSweptProfile(Solid solid)
        {
            var sweptFace = GetSweptProfileFace(solid);
            if (null == sweptFace || 1 != sweptFace.EdgeLoops.Size)
                return false;

            foreach (var point in sweptFace.Triangulate().Vertices)
                ProfilePoints.Add(Transform(point));

            m_edges = ChangeEdgeToLine(sweptFace.EdgeLoops.get_Item(0));
            return m_edges != null;
        }

        private Face GetSweptProfileFace(Solid solid)
        {
            XYZ refPoint = new();
            foreach (Edge edge in solid.Edges)
            {
                var points = edge.Tessellate() as List<XYZ>;
                if (2 != points?.Count)
                    throw new Exception("Each edge should be a line.");

                var first = Transform(points[0]);
                var second = Transform(points[1]);
                var edgeVector = XyzMath.SubXyz(second, first);
                if (XyzMath.IsSameDirection(edgeVector, m_drivingVector))
                {
                    refPoint = first;
                    break;
                }

                if (XyzMath.IsOppositeDirection(edgeVector, m_drivingVector))
                {
                    refPoint = second;
                    break;
                }
            }

            Face sweptFace = null;
            foreach (Face face in solid.Faces)
            {
                if (sweptFace != null)
                    break;
                if (!RebarGeomHelper.IsVertical(face, m_drivingLine, m_transform, null))
                    continue;

                foreach (var point in face.Triangulate().Vertices)
                {
                    if (!XyzMath.IsEqual(refPoint, Transform(point)))
                        continue;
                    sweptFace = face;
                    break;
                }
            }

            return sweptFace;
        }

        private List<Line> ChangeEdgeToLine(EdgeArray edges)
        {
            List<Line> edgeLines = new();
            foreach (Edge edge in edges)
            {
                var points = edge.Tessellate() as List<XYZ>;
                edgeLines.Add(Line.CreateBound(Transform(points[0]), Transform(points[1])));
            }

            return edgeLines;
        }
    }
}
