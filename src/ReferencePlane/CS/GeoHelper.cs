// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.ReferencePlane.CS
{
    /// <summary>
    ///     A object to help locating with geometry data.
    /// </summary>
    public class GeoHelper
    {
        //Defined the precision.
        private const double Precision = 0.0001;

        /// <summary>
        ///     Find the bottom face of a face array.
        /// </summary>
        /// <param name="faces">A face array.</param>
        /// <returns>The bottom face of a face array.</returns>
        public static Face GetBottomFace(FaceArray faces)
        {
            Face face = null;
            double elevation = 0;

            foreach (Face f in faces)
            {
                if (IsVerticalFace(f))
                    // If this is a vertical face, it cannot be a bottom face to a certainty.
                    continue;

                double tempElevation = 0;
                var mesh = f.Triangulate();

                foreach (var xyz in mesh.Vertices) tempElevation = tempElevation + xyz.Z;

                tempElevation = tempElevation / mesh.Vertices.Count;

                if (elevation > tempElevation || null == face)
                {
                    // Update the bottom face to which's elevation is the lowest.
                    face = f;
                    elevation = tempElevation;
                }
            }

            // The bottom face is consider as which's average elevation is the lowest, except vertical
            // face.
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
        ///     Calculate the length between two points.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The length between two points.</returns>
        public static double GetLength(XYZ startPoint, XYZ endPoint)
        {
            return Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) +
                             Math.Pow(endPoint.Y - startPoint.Y, 2) +
                             Math.Pow(endPoint.Z - startPoint.Z, 2));
        }

        /// <summary>
        ///     The distance between two value in a same axis.
        /// </summary>
        /// <param name="start">start value.</param>
        /// <param name="end">end value.</param>
        /// <returns>The distance between two value.</returns>
        public static double GetDistance(double start, double end)
        {
            return Math.Abs(start - end);
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
        ///     Determines whether a face is vertical.
        /// </summary>
        /// <param name="face">The face to be determined.</param>
        /// <returns>Return true if this face is vertical, or else return false.</returns>
        private static bool IsVerticalFace(Face face)
        {
            foreach (EdgeArray ea in face.EdgeLoops)
            foreach (Edge e in ea)
                if (IsVerticalEdge(e))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether a edge is vertical.
        /// </summary>
        /// <param name="edge">The edge to be determined.</param>
        /// <returns>Return true if this edge is vertical, or else return false.</returns>
        private static bool IsVerticalEdge(Edge edge)
        {
            var polyline = edge.Tessellate() as List<XYZ>;
            var verticalVct = new XYZ(0, 0, 1);
            var pointBuffer = polyline[0];

            for (var i = 1; i < polyline.Count; i = i + 1)
            {
                var temp = polyline[i];
                var vector = GetVector(pointBuffer, temp);
                if (Equal(vector, verticalVct))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether two vector are equal in x and y axis.
        /// </summary>
        /// <param name="vectorA">The vector A.</param>
        /// <param name="vectorB">The vector B.</param>
        /// <returns>Return true if two vector are equals, or else return false.</returns>
        private static bool Equal(XYZ vectorA, XYZ vectorB)
        {
            var isNotEqual = Precision < Math.Abs(vectorA.X - vectorB.X) ||
                             Precision < Math.Abs(vectorA.Y - vectorB.Y);

            return isNotEqual ? false : true;
        }
    }
}
