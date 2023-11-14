// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     The class which give the base geometry operation, it is a static class.
    /// </summary>
    internal static class GeomUtil
    {
        // Private members
        private const double Precision = 0.00001; //precision when judge whether two doubles are equal

        /// <summary>
        ///     Judge whether the two double data are equal
        /// </summary>
        /// <param name="d1">The first double data</param>
        /// <param name="d2">The second double data</param>
        /// <returns>true if two double data is equal, otherwise false</returns>
        public static bool IsEqual(double d1, double d2)
        {
            //get the absolute value;
            var diff = Math.Abs(d1 - d2);
            return diff < Precision;
        }

        /// <summary>
        ///     Judge whether the two Autodesk.Revit.DB.XYZ point are equal
        /// </summary>
        /// <param name="first">The first Autodesk.Revit.DB.XYZ point</param>
        /// <param name="second">The second Autodesk.Revit.DB.XYZ point</param>
        /// <returns>true if two Autodesk.Revit.DB.XYZ point is equal, otherwise false</returns>
        public static bool IsEqual(XYZ first, XYZ second)
        {
            var flag = true;
            flag = flag && IsEqual(first.X, second.X);
            flag = flag && IsEqual(first.Y, second.Y);
            flag = flag && IsEqual(first.Z, second.Z);
            return flag;
        }

        /// <param name="face"></param>
        /// <param name="line"></param>
        /// <returns>return true when line is perpendicular to the face</returns>
        /// <summary>
        ///     Judge whether the line is perpendicular to the face
        /// </summary>
        /// <param name="face">the face reference</param>
        /// <param name="line">the line reference</param>
        /// <param name="faceTrans">the transform for the face</param>
        /// <param name="lineTrans">the transform for the line</param>
        /// <returns>true if line is perpendicular to the face, otherwise false</returns>
        public static bool IsVertical(Face face, Line line,
            Transform faceTrans, Transform lineTrans)
        {
            //get points which the face contains
            var points = face.Triangulate().Vertices as List<XYZ>;
            if (3 > points.Count) // face's point number should be above 2
                return false;

            // get three points from the face points
            var first = points[0];
            var second = points[1];
            var third = points[2];

            // get start and end point of line
            var lineStart = line.GetEndPoint(0);
            var lineEnd = line.GetEndPoint(1);

            // transForm the three points if necessary
            if (null != faceTrans)
            {
                first = TransformPoint(first, faceTrans);
                second = TransformPoint(second, faceTrans);
                third = TransformPoint(third, faceTrans);
            }

            // transform the start and end points if necessary
            if (null != lineTrans)
            {
                lineStart = TransformPoint(lineStart, lineTrans);
                lineEnd = TransformPoint(lineEnd, lineTrans);
            }

            // form two vectors from the face and a vector stand for the line
            // Use SubXYZ() method to get the vectors
            var vector1 = SubXYZ(first, second); // first vector of face
            var vector2 = SubXYZ(first, third); // second vector of face
            var vector3 = SubXYZ(lineStart, lineEnd); // line vector

            // get two dot products of the face vectors and line vector
            var result1 = DotMatrix(vector1, vector3);
            var result2 = DotMatrix(vector2, vector3);

            // if two dot products are all zero, the line is perpendicular to the face
            return IsEqual(result1, 0) && IsEqual(result2, 0);
        }

        /// <summary>
        ///     judge whether the two vectors have the same direction
        /// </summary>
        /// <param name="firstVec">the first vector</param>
        /// <param name="secondVec">the second vector</param>
        /// <returns>true if the two vector is in same direction, otherwise false</returns>
        public static bool IsSameDirection(XYZ firstVec, XYZ secondVec)
        {
            // get the unit vector for two vectors
            var first = UnitVector(firstVec);
            var second = UnitVector(secondVec);

            // if the dot product of two unit vectors is equal to 1, return true
            var dot = DotMatrix(first, second);
            return IsEqual(dot, 1);
        }

        /// <summary>
        ///     Judge whether the two vectors have the opposite direction
        /// </summary>
        /// <param name="firstVec">the first vector</param>
        /// <param name="secondVec">the second vector</param>
        /// <returns>true if the two vector is in opposite direction, otherwise false</returns>
        public static bool IsOppositeDirection(XYZ firstVec, XYZ secondVec)
        {
            // get the unit vector for two vectors
            var first = UnitVector(firstVec);
            var second = UnitVector(secondVec);

            // if the dot product of two unit vectors is equal to -1, return true
            var dot = DotMatrix(first, second);
            return IsEqual(dot, -1);
        }

        /// <summary>
        ///     multiplication cross of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1">The first XYZ</param>
        /// <param name="p2">The second XYZ</param>
        /// <returns>the normal vector of the face which first and secend vector lie on</returns>
        public static XYZ CrossMatrix(XYZ p1, XYZ p2)
        {
            //get the coordinate of the XYZ
            var u1 = p1.X;
            var u2 = p1.Y;
            var u3 = p1.Z;

            var v1 = p2.X;
            var v2 = p2.Y;
            var v3 = p2.Z;

            var x = v3 * u2 - v2 * u3;
            var y = v1 * u3 - v3 * u1;
            var z = v2 * u1 - v1 * u2;

            return new XYZ(x, y, z);
        }


        /// <summary>
        ///     Set the vector into unit length
        /// </summary>
        /// <param name="vector">the input vector</param>
        /// <returns>the vector in unit length</returns>
        public static XYZ UnitVector(XYZ vector)
        {
            // calculate the distance from grid origin to the XYZ
            var length = GetLength(vector);

            // changed the vector into the unit length
            var x = vector.X / length;
            var y = vector.Y / length;
            var z = vector.Z / length;
            return new XYZ(x, y, z);
        }

        /// <summary>
        ///     calculate the distance from grid origin to the XYZ(vector length)
        /// </summary>
        /// <param name="vector">the input vector</param>
        /// <returns>the length of the vector</returns>
        public static double GetLength(XYZ vector)
        {
            var x = vector.X;
            var y = vector.Y;
            var z = vector.Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        ///     Subtraction of two points(or vectors), get a new vector
        /// </summary>
        /// <param name="p1">the first point(vector)</param>
        /// <param name="p2">the second point(vector)</param>
        /// <returns>return a new vector from point p2 to p1</returns>
        public static XYZ SubXYZ(XYZ p1, XYZ p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            var z = p1.Z - p2.Z;

            return new XYZ(x, y, z);
        }

        /// <summary>
        ///     Add of two points(or vectors), get a new point(vector)
        /// </summary>
        /// <param name="p1">the first point(vector)</param>
        /// <param name="p2">the first point(vector)</param>
        /// <returns>a new vector(point)</returns>
        public static XYZ AddXYZ(XYZ p1, XYZ p2)
        {
            var x = p1.X + p2.X;
            var y = p1.Y + p2.Y;
            var z = p1.Z + p2.Z;

            return new XYZ(x, y, z);
        }

        /// <summary>
        ///     Multiply a vector with a number
        /// </summary>
        /// <param name="vector">a vector</param>
        /// <param name="rate">the rate number</param>
        /// <returns></returns>
        public static XYZ MultiplyVector(XYZ vector, double rate)
        {
            var x = vector.X * rate;
            var y = vector.Y * rate;
            var z = vector.Z * rate;

            return new XYZ(x, y, z);
        }

        /// <summary>
        ///     Transform old coordinate system in the new coordinate system
        /// </summary>
        /// <param name="point">the Autodesk.Revit.DB.XYZ which need to be transformed</param>
        /// <param name="transform">the value of the coordinate system to be transformed</param>
        /// <returns>the new Autodesk.Revit.DB.XYZ which has been transformed</returns>
        public static XYZ TransformPoint(XYZ point, Transform transform)
        {
            //get the coordinate value in X, Y, Z axis
            var x = point.X;
            var y = point.Y;
            var z = point.Z;

            //transform basis of the old coordinate system in the new coordinate system
            var b0 = transform.get_Basis(0);
            var b1 = transform.get_Basis(1);
            var b2 = transform.get_Basis(2);
            var origin = transform.Origin;

            //transform the origin of the old coordinate system in the new coordinate system
            var xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
            var yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
            var zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;

            return new XYZ(xTemp, yTemp, zTemp);
        }

        /// <summary>
        ///     Move a point a give offset along a given direction
        /// </summary>
        /// <param name="point">the point need to move</param>
        /// <param name="direction">the direction the point move to</param>
        /// <param name="offset">indicate how long to move</param>
        /// <returns>the moved point</returns>
        public static XYZ OffsetPoint(XYZ point, XYZ direction, double offset)
        {
            var directUnit = UnitVector(direction);
            var offsetVect = MultiplyVector(directUnit, offset);
            return AddXYZ(point, offsetVect);
        }

        /// <summary>
        ///     get the orient of hook according to curve direction, reinforcement normal and hook direction
        /// </summary>
        /// <param name="curveVec">the curve direction</param>
        /// <param name="normal">rebar normal direction</param>
        /// <param name="hookVec">the hook direction</param>
        /// <returns>the orient of the hook</returns>
        public static RebarHookOrientation GetHookOrient(XYZ curveVec, XYZ normal, XYZ hookVec)
        {
            var tempVec = normal;

            for (var i = 0; i < 4; i++)
            {
                tempVec = CrossMatrix(tempVec, curveVec);
                if (IsSameDirection(tempVec, hookVec))
                {
                    if (i == 0)
                        return RebarHookOrientation.Right;
                    if (i == 2) return RebarHookOrientation.Left;
                }
            }

            throw new Exception("Can't find the hook orient according to hook direction.");
        }

        /// <summary>
        ///     Judge the vector is in right or left direction
        /// </summary>
        /// <param name="normal">The unit vector need to be judged its direction</param>
        /// <returns>if in right dircetion return true, otherwise return false</returns>
        public static bool IsInRightDir(XYZ normal)
        {
            var eps = 1.0e-8;
            if (Math.Abs(normal.X) <= eps)
            {
                if (normal.Y > 0) return false;
                return true;
            }

            if (normal.X > 0) return true;
            if (normal.X < 0) return false;
            return true;
        }

        /// <summary>
        ///     dot product of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1">The first XYZ</param>
        /// <param name="p2">The second XYZ</param>
        /// <returns>the cosine value of the angle between vector p1 an p2</returns>
        private static double DotMatrix(XYZ p1, XYZ p2)
        {
            //get the coordinate of the Autodesk.Revit.DB.XYZ 
            var v1 = p1.X;
            var v2 = p1.Y;
            var v3 = p1.Z;

            var u1 = p2.X;
            var u2 = p2.Y;
            var u3 = p2.Z;

            return v1 * u1 + v2 * u2 + v3 * u3;
        }
    }
}
