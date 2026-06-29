// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    /// <summary>
    ///     A object to help locating with geometry data.
    /// </summary>
    public class GeoHelper
    {
        /// <summary>
        ///     store the const precision
        /// </summary>
        private const double Precision = 0.0001;

        /// <summary>
        ///     The method is used to get the wall face along the specified parameters
        /// </summary>
        /// <param name="wall">the wall</param>
        /// <param name="view">the options view</param>
        /// <param name="extOrInt">if true indicate that get exterior wall face, else false get the interior wall face</param>
        /// <returns>the face</returns>
        public static Face GetWallFace(Wall wall, View view, bool extOrInt)
        {
            FaceArray faces = null;
            Face face = null;
            var options = new Options
            {
                ComputeReferences = true,
                View = view
            };
            if (wall != null)
            {
                //GeometryObjectArray geoArr = wall.get_Geometry(options).Objects;
                var objects = wall.get_Geometry(options).GetEnumerator();
                //foreach (GeometryObject geoObj in geoArr)
                while (objects.MoveNext())
                {
                    var geoObj = objects.Current;

                    if (geoObj is Solid obj)
                    {
                        faces = obj.Faces;
                    }
                }
            }

            if (extOrInt)
                face = GetExteriorFace(faces);
            else
                face = GetInteriorFace(faces);
            return face;
        }

        /// <summary>
        ///     The method is used to get extrusion's face along to the specified parameters
        /// </summary>
        /// <param name="extrusion">the extrusion</param>
        /// <param name="view">options view</param>
        /// <param name="extOrInt">If true indicate getting exterior extrusion face, else getting interior extrusion face</param>
        /// <returns>the face</returns>
        public static Face GetExtrusionFace(Extrusion extrusion, View view, bool extOrInt)
        {
            Face face = null;
            FaceArray faces = null;
            if (extrusion.IsSolid)
            {
                var options = new Options
                {
                    ComputeReferences = true,
                    View = view
                };
                //GeometryObjectArray geoArr = extrusion.get_Geometry(options).Objects;
                var objects = extrusion.get_Geometry(options).GetEnumerator();
                //foreach (GeometryObject geoObj in geoArr)
                while (objects.MoveNext())
                {
                    var geoObj = objects.Current;

                    if (geoObj is Solid obj)
                    {
                        faces = obj.Faces;
                    }
                }

                if (extOrInt)
                    face = GetExteriorFace(faces);
                else
                    face = GetInteriorFace(faces);
            }

            return face;
        }

        /// <summary>
        ///     The assistant method is used for getting wall face and getting extrusion face
        /// </summary>
        /// <param name="faces">faces array</param>
        /// <returns>the face</returns>
        private static Face GetExteriorFace(FaceArray faces)
        {
            double elevation = 0;
            Face face = null;
            foreach (Face f in faces)
            {
                double tempElevation = 0;
                var mesh = f.Triangulate();
                foreach (var xyz in mesh.Vertices)
                {
                    tempElevation += xyz.Y;
                }

                tempElevation /= mesh.Vertices.Count;
                if (elevation < tempElevation || null == face)
                {
                    face = f;
                    elevation = tempElevation;
                }
            }

            return face;
        }

        /// <summary>
        ///     The assistant method is used for getting wall face and getting extrusion face
        /// </summary>
        /// <param name="faces">faces array</param>
        /// <returns>the face</returns>
        private static Face GetInteriorFace(FaceArray faces)
        {
            double elevation = 0;
            Face face = null;
            foreach (Face f in faces)
            {
                double tempElevation = 0;
                var mesh = f.Triangulate();
                foreach (var xyz in mesh.Vertices)
                {
                    tempElevation += xyz.Y;
                }

                tempElevation /= mesh.Vertices.Count;
                if (elevation > tempElevation || null == face)
                {
                    face = f;
                    elevation = tempElevation;
                }
            }

            return face;
        }

        /// <summary>
        ///     Find out the three points which made of a plane.
        /// </summary>
        /// <param name="mesh">A mesh contains many points.</param>
        /// <param name="startPoint">Create a new instance of ReferencePlane.</param>
        /// <param name="endPoint">The free end apply to reference plane.</param>
        /// <param name="thirdPnt">A third point needed to define the reference plane.</param>
        public static void Distribute(Mesh mesh, ref XYZ startPoint, ref XYZ endPoint, ref XYZ thirdPnt)
        {
            var count = mesh.Vertices.Count;
            startPoint = mesh.Vertices[0];
            endPoint = mesh.Vertices[count / 3];
            thirdPnt = mesh.Vertices[count / 3 * 2];
        }

        /// <summary>
        ///     Determines whether a edge is vertical.
        /// </summary>
        /// <param name="edge">The edge to be determined.</param>
        /// <returns>Return true if this edge is vertical, or else return false.</returns>
        public static bool IsVerticalEdge(Edge edge)
        {
            var polyline = edge.Tessellate() as List<XYZ>;
            var verticalVct = new XYZ(0, 0, 1);
            var pointBuffer = polyline[0];

            for (var i = 1; i < polyline.Count; i++)
            {
                var temp = polyline[i];
                var vector = GetVector(pointBuffer, temp);
                if (Equal(vector, verticalVct))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Get the vector between two points.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The vector between two points.</returns>
        public static XYZ GetVector(XYZ startPoint, XYZ endPoint)
        {
            return new XYZ(endPoint.X - startPoint.X,
                endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);
        }

        /// <summary>
        ///     Determines whether two vector are equal in x and y axis.
        /// </summary>
        /// <param name="vectorA">The vector A.</param>
        /// <param name="vectorB">The vector B.</param>
        /// <returns>Return true if two vector are equals, or else return false.</returns>
        public static bool Equal(XYZ vectorA, XYZ vectorB)
        {
            var isNotEqual = Precision < Math.Abs(vectorA.X - vectorB.X) ||
                             Precision < Math.Abs(vectorA.Y - vectorB.Y);
            return isNotEqual ? false : true;
        }
    }
}
