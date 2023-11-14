//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.Reinforcement.CS
{
    using GeoInstance = GeometryInstance;

    /// <summary>
    ///     The base class which support beamGeometrySupport and ColumnGeometrySupport etc.
    ///     it store some common geometry information, and give some helper fuctions
    /// </summary>
    public class GeometrySupport
    {
        /// <summary>
        ///     the extend or sweep path of the beam or column
        /// </summary>
        protected Line m_drivingLine;

        /// <summary>
        ///     the director vector of beam or column
        /// </summary>
        protected XYZ m_drivingVector;

        /// <summary>
        ///     a list to store the edges
        /// </summary>
        protected List<Line> m_edges = new List<Line>();

        /// <summary>
        ///     a list to store the point
        /// </summary>
        protected List<XYZ> m_points = new List<XYZ>();

        /// <summary>
        ///     store the solid of beam or column
        /// </summary>
        protected Solid m_solid;

        /// <summary>
        ///     the transform value of the solid
        /// </summary>
        protected Transform m_transform;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="element">the host object, must be family instance</param>
        /// <param name="geoOptions">the geometry option</param>
        public GeometrySupport(FamilyInstance element, Options geoOptions)
        {
            // get the geometry element of the selected element
            var geoElement = element.get_Geometry(new Options());
            var Objects = geoElement.GetEnumerator();
            if (null == geoElement || !Objects.MoveNext())
                throw new Exception("Can't get the geometry of selected element.");

            var swProfile = element.GetSweptProfile();
            if (swProfile == null || !(swProfile.GetDrivingCurve() is Line))
                throw new Exception("The selected element driving curve is not a line.");

            // get the driving path and vector of the beam or column
            var line = swProfile.GetDrivingCurve() as Line;
            if (null != line)
            {
                m_drivingLine = line; // driving path
                m_drivingVector = GeomUtil.SubXYZ(line.GetEndPoint(1), line.GetEndPoint(0));
            }

            //get the geometry object
            Objects.Reset();
            //foreach (GeometryObject geoObject in geoElement.Objects)
            while (Objects.MoveNext())
            {
                var geoObject = Objects.Current;

                //get the geometry instance which contain the geometry information
                var instance = geoObject as GeoInstance;
                if (null != instance)
                {
                    //foreach (GeometryObject o in instance.SymbolGeometry.Objects)
                    var Objects1 = instance.SymbolGeometry.GetEnumerator();
                    while (Objects1.MoveNext())
                    {
                        var o = Objects1.Current;

                        // get the solid of beam of column
                        var solid = o as Solid;

                        // do some checks.
                        if (null == solid) continue;
                        if (0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;

                        m_solid = solid;
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
            if (4 != m_points.Count) throw new Exception("The sample only work for rectangular beams or columns.");
        }


        /// <summary>
        ///     transform the point to new coordinates
        /// </summary>
        /// <param name="point">the point need to transform</param>
        /// <returns>the changed point</returns>
        protected XYZ Transform(XYZ point)
        {
            // only invoke the TransformPoint() method.
            return GeomUtil.TransformPoint(point, m_transform);
        }


        /// <summary>
        ///     Get the length of driving line
        /// </summary>
        /// <returns>the length of the driving line</returns>
        protected double GetDrivingLineLength()
        {
            return GeomUtil.GetLength(m_drivingVector);
        }

        /// <summary>
        ///     Get two vectors, which indicate some edge direction which contain given point,
        ///     set the given point as the start point, the other end point of the edge as end
        /// </summary>
        /// <param name="point">a point of the swept profile</param>
        /// <returns>two vectors indicate edge direction</returns>
        protected List<XYZ> GetRelatedVectors(XYZ point)
        {
            // Initialize the return vector list.
            var vectors = new List<XYZ>();

            // Get all the edge which contain this point.
            // And get the vector from this point to another point
            foreach (var line in m_edges)
            {
                if (GeomUtil.IsEqual(point, line.GetEndPoint(0)))
                {
                    var vector = GeomUtil.SubXYZ(line.GetEndPoint(1), line.GetEndPoint(0));
                    vectors.Add(vector);
                }

                if (GeomUtil.IsEqual(point, line.GetEndPoint(1)))
                {
                    var vector = GeomUtil.SubXYZ(line.GetEndPoint(0), line.GetEndPoint(1));
                    vectors.Add(vector);
                }
            }

            // only two vector(direction) should be found
            if (2 != vectors.Count) throw new Exception("a point on swept profile should have only two direction.");

            return vectors;
        }


        /// <summary>
        ///     Offset the points of the swept profile to make the points inside swept profile
        /// </summary>
        /// <param name="offset">indicate how long to offset on two directions</param>
        /// <returns>the offset points</returns>
        protected List<XYZ> OffsetPoints(double offset)
        {
            // Initialize the offset point list.
            var points = new List<XYZ>();

            // Get all points of the swept profile, and offset it in two related direction
            foreach (var point in m_points)
            {
                // Get two related directions
                var directions = GetRelatedVectors(point);
                var firstDir = directions[0];
                var secondDir = directions[1];

                // offset the point in two direction
                var movedPoint = GeomUtil.OffsetPoint(point, firstDir, offset);
                movedPoint = GeomUtil.OffsetPoint(movedPoint, secondDir, offset);

                // add the offset point into the array
                points.Add(movedPoint);
            }

            return points;
        }


        /// <summary>
        ///     Find the inforamtion of the swept profile(face),
        ///     and store the points and edges of the profile(face)
        /// </summary>
        /// <param name="solid">the solid reference</param>
        /// <returns>true if the swept profile can be gotten, otherwise false</returns>
        private bool GetSweptProfile(Solid solid)
        {
            // get the swept face
            var sweptFace = GetSweptProfileFace(solid);
            // do some checks
            if (null == sweptFace || 1 != sweptFace.EdgeLoops.Size) return false;

            // get the points of the swept face
            foreach (var point in sweptFace.Triangulate().Vertices) m_points.Add(Transform(point));

            // get the edges of the swept face
            m_edges = ChangeEdgeToLine(sweptFace.EdgeLoops.get_Item(0));

            // do some checks
            return null != m_edges;
        }

        /// <summary>
        ///     Get the swept profile(face) of the host object(family instance)
        /// </summary>
        /// <param name="solid">the solid reference</param>
        /// <returns>the swept profile</returns>
        private Face GetSweptProfileFace(Solid solid)
        {
            // Get a point on the swept profile from all points in solid
            var refPoint = new XYZ(); // the point on swept profile
            foreach (Edge edge in solid.Edges)
            {
                var points = edge.Tessellate() as List<XYZ>; //get end points of the edge
                if (2 != points.Count) // make sure all edges are lines
                    throw new Exception("All edge should be line.");

                // get two points of the edge. All points in solid should be transform first
                var first = Transform(points[0]); // start point of edge
                var second = Transform(points[1]); // end point of edge

                // some edges should be parallelled with the driving line,
                // and the start point of that edge should be the wanted point
                var edgeVector = GeomUtil.SubXYZ(second, first);
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
                // use the gotted point to get the swept face
                foreach (var point in face.Triangulate().Vertices)
                {
                    var pnt = Transform(point); // all points in solid should be transform
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
        /// <param name="edges">the swept profile edges</param>
        /// <returns>the line list which stores the swept profile edges</returns>
        private List<Line> ChangeEdgeToLine(EdgeArray edges)
        {
            // create the line list instance.
            var edgeLines = new List<Line>();

            // get each edge from swept profile,
            // and changed the geometry information in line list
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