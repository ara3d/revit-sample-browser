// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.Geom
{
    // using GeoInstance as Autodesk.Revit.DB.Instance
    using GeoInstance = GeometryInstance;

    /// <summary>
    ///     Compute geometry information and store geometry information.
    /// </summary>
    public class GeometrySupport
    {
        /// <summary>
        ///     the extend or sweep path of the beam or column
        /// </summary>
        private readonly Line m_drivingLine;

        /// <summary>
        ///     the director vector of beam or column
        /// </summary>
        private readonly XYZ m_drivingVector;

        /// <summary>
        ///     a list to store the edges
        /// </summary>
        private List<Line> m_edges = new List<Line>();

        /// <summary>
        ///     the transform value of the solid
        /// </summary>
        private readonly Transform m_transform;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="element">The host object, must be family instance</param>
        public GeometrySupport(FamilyInstance element)
        {
            // get the geometry element of the selected element
            var geoElement = element.get_Geometry(new Options());
            using (var objects = geoElement.GetEnumerator())
            {
                //if (null == geoElement || 0 == geoElement.Objects.Size)
                if (null == geoElement || !objects.MoveNext())
                    throw new Exception("Can't get the geometry of selected element.");

                var swProfile = element.GetSweptProfile();
                if (swProfile == null || !(swProfile.GetDrivingCurve() is Line))
                    throw new Exception("The selected element driving curve is not a line.");

                // get the driving path and vector of the beam or column
                var line = swProfile.GetDrivingCurve() as Line;
                if (null != line)
                {
                    m_drivingLine = line; // driving path
                    m_drivingVector = GeomUtil.SubXyz(line.GetEndPoint(1), line.GetEndPoint(0));
                    DrivingLength = m_drivingVector.GetLength();
                }

                //get the geometry object
                //foreach (GeometryObject geoObject in geoElement.Objects)
                objects.Reset();
                while (objects.MoveNext())
                {
                    var geoObject = objects.Current;

                    //get the geometry instance which contains the geometry information
                    var instance = geoObject as GeoInstance;
                    if (null == instance) continue;
                    
                    foreach (GeometryObject o in instance.SymbolGeometry)
                    {
                        // get the solid of beam of column
                        var solid = o as Solid;

                        // do some checks.
                        if (null == solid) continue;
                        if (0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;

                        //get the transform value of instance
                        m_transform = instance.Transform;

                        // Get the swept profile curves information
                        if (!GetSweptProfile(solid)) throw new Exception("Can't get the swept profile curves.");
                        break;
                    }
                }
            }

            // do some checks about profile curves information
            if (null == m_edges) throw new Exception("Can't get the geometry edge information.");
            if (4 != ProfilePoints.Count) throw new Exception("The sample only works for rectangle beam or column.");
        }

        /// <summary>
        ///     Return profile points
        /// </summary>
        public List<XYZ> ProfilePoints { get; set; } = new List<XYZ>();

        /// <summary>
        ///     Return driving length
        /// </summary>
        public double DrivingLength { get; }

        /// <summary>
        ///     Transform the point to new coordinates
        /// </summary>
        /// <param name="point">The point need to transform</param>
        /// <returns>The changed point</returns>
        private XYZ Transform(XYZ point)
        {
            // only invoke the TransformPoint() method.
            return GeomUtil.TransformPoint(point, m_transform);
        }

        /// <summary>
        ///     Get two vectors, which indicate some edge direction which contain given point,
        ///     set the given point as the start point, the other end point of the edge as end
        /// </summary>
        /// <param name="point">A point of the swept profile</param>
        /// <returns>Two vectors indicate edge direction</returns>
        private List<XYZ> GetRelatedVectors(XYZ point)
        {
            // Initialize the return vector list.
            var vectors = new List<XYZ>();

            // Get all the edges which contain this point.
            // And get the vector from this point to another point
            foreach (var line in m_edges)
            {
                if (GeomUtil.IsEqual(point, line.GetEndPoint(0)))
                {
                    var vector = GeomUtil.SubXyz(line.GetEndPoint(1), line.GetEndPoint(0));
                    vectors.Add(vector);
                }

                if (GeomUtil.IsEqual(point, line.GetEndPoint(1)))
                {
                    var vector = GeomUtil.SubXyz(line.GetEndPoint(0), line.GetEndPoint(1));
                    vectors.Add(vector);
                }
            }

            // only two vectors(directions) should be found
            if (2 != vectors.Count) throw new Exception("A point on swept profile should have only two directions.");

            return vectors;
        }

        /// <summary>
        ///     Offset the points of the swept profile to make the points inside swept profile
        /// </summary>
        /// <param name="offset">Indicate how long to offset on two directions</param>
        /// <returns>The offset points</returns>
        public List<XYZ> OffsetPoints(double offset)
        {
            // Initialize the offset point list.
            var points = new List<XYZ>();

            // Get all points of the swept profile, and offset it in two related directions
            foreach (var point in ProfilePoints)
            {
                // Get two related directions
                var directions = GetRelatedVectors(point);
                var firstDir = directions[0];
                var secondDir = directions[1];

                // offset the point in two directions
                var movedPoint = GeomUtil.OffsetPoint(point, firstDir, offset);
                movedPoint = GeomUtil.OffsetPoint(movedPoint, secondDir, offset);

                // add the offset point into the array
                points.Add(movedPoint);
            }

            return points;
        }

        /// <summary>
        ///     Find the information of the swept profile(face),
        ///     and store the points and edges of the profile(face)
        /// </summary>
        /// <param name="solid">The solid reference</param>
        /// <returns>True if the swept profile can be gotten, otherwise false</returns>
        private bool GetSweptProfile(Solid solid)
        {
            // get the swept face
            var sweptFace = GetSweptProfileFace(solid);
            // do some checks
            if (null == sweptFace || 1 != sweptFace.EdgeLoops.Size) return false;

            // get the points of the swept face
            foreach (var point in sweptFace.Triangulate().Vertices)
            {
                ProfilePoints.Add(Transform(point));
            }

            // get the edges of the swept face
            m_edges = ChangeEdgeToLine(sweptFace.EdgeLoops.get_Item(0));

            // do some checks
            return null != m_edges;
        }

        /// <summary>
        ///     Get the swept profile(face) of the host object(family instance)
        /// </summary>
        /// <param name="solid">The solid reference</param>
        /// <returns>The swept profile</returns>
        private Face GetSweptProfileFace(Solid solid)
        {
            // Get a point on the swept profile from all points in solid
            var refPoint = new XYZ(); // the point on swept profile
            foreach (Edge edge in solid.Edges)
            {
                var points = edge.Tessellate() as List<XYZ>; //get end points of the edge
                if (2 != points.Count) // make sure all edges are lines
                    throw new Exception("Each edge should be a line.");

                // get two points of the edge. All points in solid should be transformed first
                var first = Transform(points[0]); // start point of edge
                var second = Transform(points[1]); // end point of edge

                // some edges should be paralleled with the driving line,
                // and the start point of that edge should be the wanted point
                var edgeVector = GeomUtil.SubXyz(second, first);
                if (GeomUtil.IsSameDirection(edgeVector, m_drivingVector))
                {
                    refPoint = first;
                    break;
                }

                if (GeomUtil.IsOppositeDirection(edgeVector, m_drivingVector))
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
                if (!GeomUtil.IsVertical(face, m_drivingLine, m_transform, null)) continue;
                // use the point to get the swept face
                foreach (var point in face.Triangulate().Vertices)
                {
                    var pnt = Transform(point); // all points in solid should be transformed
                    if (GeomUtil.IsEqual(refPoint, pnt))
                    {
                        sweptFace = face;
                        break;
                    }
                }
            }

            return sweptFace;
        }

        /// <summary>
        ///     Change the swept profile edges from EdgeArray type to line list
        /// </summary>
        /// <param name="edges">The swept profile edges</param>
        /// <returns>The line list which stores the swept profile edges</returns>
        private List<Line> ChangeEdgeToLine(EdgeArray edges)
        {
            // create the line list instance.
            var edgeLines = new List<Line>();

            // get each edge from swept profile,
            // and change the geometry information in line list
            foreach (Edge edge in edges)
            {
                //get the two points of each edge
                var points = edge.Tessellate() as List<XYZ>;
                var first = Transform(points[0]);
                var second = Transform(points[1]);

                // create new line and add them into line list
                edgeLines.Add(Line.CreateBound(first, second));
            }

            return edgeLines;
        }
    }
}
