// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.SlabShapeEditing.CS
{
    /// <summary>
    ///     Vector4 is a homogeneous coordinate class used to store vector
    ///     and contain method to handle the vector
    /// </summary>
    public class Vector4
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public Vector4(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     constructor, transfer Autodesk.Revit.DB.XYZ to vector
        /// </summary>
        /// <param name="v">Autodesk.Revit.DB.XYZ structure which needs to be transferred</param>
        public Vector4(XYZ v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        ///     X property to get/set x value of Vector4
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     Y property to get/set y value of Vector4
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        ///     Z property to get/set z value of Vector4
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        ///     W property to get/set fourth value of Vector4
        /// </summary>
        public double W { get; set; } = 1.0f;

        /// <summary>
        ///     adds two vectors
        /// </summary>
        /// <param name="va">first vector</param>
        /// <param name="vb">second vector</param>
        public static Vector4 operator +(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z);
        }

        /// <summary>
        ///     subtracts two vectors
        /// </summary>
        /// <param name="va">first vector</param>
        /// <param name="vb">second vector</param>
        /// <returns>subtraction of two vectors</returns>
        public static Vector4 operator -(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z);
        }

        /// <summary>
        ///     multiplies a vector by a double type value
        /// </summary>
        /// <param name="v">vector</param>
        /// <param name="factor">multiplier of double type</param>
        /// <returns> the result vector </returns>
        public static Vector4 operator *(Vector4 v, double factor)
        {
            return new Vector4(v.X * factor, v.Y * factor, v.Z * factor);
        }

        /// <summary>
        ///     divides vector by a double type value
        /// </summary>
        /// <param name="v">vector</param>
        /// <param name="factor">double type value</param>
        /// <returns> vector divided by a double type value </returns>
        public static Vector4 operator /(Vector4 v, double factor)
        {
            return new Vector4(v.X / factor, v.Y / factor, v.Z / factor);
        }

        /// <summary>
        ///     dot multiply vector
        /// </summary>
        /// <param name="v"> the result vector </param>
        public double DotProduct(Vector4 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        /// <summary>
        ///     get normal vector of plane contains two vectors
        /// </summary>
        /// <param name="v">second vector</param>
        /// <returns> normal vector of two vectors</returns>
        public Vector4 CrossProduct(Vector4 v)
        {
            return new Vector4(Y * v.Z - Z * v.Y, Z * v.X
                                                  - X * v.Z, X * v.Y - Y * v.X);
        }

        /// <summary>
        ///     dot multiply two vectors
        /// </summary>
        /// <param name="va">first vector</param>
        /// <param name="vb">second vector</param>
        public static double DotProduct(Vector4 va, Vector4 vb)
        {
            return va.X * vb.X + va.Y * vb.Y + va.Z * vb.Z;
        }

        /// <summary>
        ///     get normal vector of two vectors
        /// </summary>
        /// <param name="va">first vector</param>
        /// <param name="vb">second vector</param>
        /// <returns> normal vector of two vectors </returns>
        public static Vector4 CrossProduct(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X
                                                          - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X);
        }

        /// <summary>
        ///     get unit vector
        /// </summary>
        public void Normalize()
        {
            var length = Length();
            if (length == 0) length = 1;
            X /= length;
            Y /= length;
            Z /= length;
        }

        /// <summary>
        ///     calculate the length of vector
        /// </summary>
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
    }

    /// <summary>
    ///     Matrix used to transform between ucs coordinate and world coordinate.
    /// </summary>
    public class Matrix4
    {
        /// <summary>
        ///     Matrix Type Enum use to define function of matrix
        /// </summary>
        public enum MatrixType
        {
            Rotation, // matrix use to rotate
            Translation, // matrix used to Translation 
            Scale, // matrix used to Scale 
            RotationAndTranslation, // matrix used to Rotation and Translation 
            Normal // normal matrix
        }

        /// <summary>
        ///     default constructor
        /// </summary>
        public Matrix4()
        {
            Type = MatrixType.Normal;
            Identity();
        }

        /// <summary>
        ///     ctor,rotation matrix,origin at (0,0,0)
        /// </summary>
        /// <param name="xAxis">identity of x axis</param>
        /// <param name="yAxis">identity of y axis</param>
        /// <param name="zAxis">identity of z axis</param>
        public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis)
        {
            Type = MatrixType.Rotation;
            Identity();
            Matrix[0, 0] = xAxis.X;
            Matrix[0, 1] = xAxis.Y;
            Matrix[0, 2] = xAxis.Z;
            Matrix[1, 0] = yAxis.X;
            Matrix[1, 1] = yAxis.Y;
            Matrix[1, 2] = yAxis.Z;
            Matrix[2, 0] = zAxis.X;
            Matrix[2, 1] = zAxis.Y;
            Matrix[2, 2] = zAxis.Z;
        }

        /// <summary>
        ///     ctor,translation matrix.
        /// </summary>
        /// <param name="origin">origin of ucs in world coordinate</param>
        public Matrix4(Vector4 origin)
        {
            Type = MatrixType.Translation;
            Identity();
            Matrix[3, 0] = origin.X;
            Matrix[3, 1] = origin.Y;
            Matrix[3, 2] = origin.Z;
        }

        /// <summary>
        ///     rotation and translation matrix constructor
        /// </summary>
        /// <param name="xAxis">x Axis</param>
        /// <param name="yAxis">y Axis</param>
        /// <param name="zAxis">z Axis</param>
        /// <param name="origin">origin</param>
        public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 origin)
        {
            Type = MatrixType.RotationAndTranslation;
            Identity();
            Matrix[0, 0] = xAxis.X;
            Matrix[0, 1] = xAxis.Y;
            Matrix[0, 2] = xAxis.Z;
            Matrix[1, 0] = yAxis.X;
            Matrix[1, 1] = yAxis.Y;
            Matrix[1, 2] = yAxis.Z;
            Matrix[2, 0] = zAxis.X;
            Matrix[2, 1] = zAxis.Y;
            Matrix[2, 2] = zAxis.Z;
            Matrix[3, 0] = origin.X;
            Matrix[3, 1] = origin.Y;
            Matrix[3, 2] = origin.Z;
        }

        /// <summary>
        ///     scale matrix constructor
        /// </summary>
        /// <param name="scale">scale factor</param>
        public Matrix4(double scale)
        {
            Type = MatrixType.Scale;
            Identity();
            Matrix[0, 0] = scale;
            Matrix[1, 1] = scale;
            Matrix[2, 2] = scale;
        }

        /// <summary>
        ///     X property to get/set Type of matrix
        /// </summary>
        public MatrixType Type { get; set; }

        /// <summary>
        ///     X property to get/set Array which store data for matrix
        /// </summary>
        public double[,] Matrix { get; set; } = new double[4, 4];

        /// <summary>
        ///     indexer of matrix
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="column">column number</param>
        /// <returns></returns>
        public double this[int row, int column]
        {
            get => Matrix[row, column];
            set => Matrix[row, column] = value;
        }

        /// <summary>
        ///     get a matrix used to rotate object specific angle on X direction
        /// </summary>
        /// <param name="angle">rotate angle</param>
        /// <returns>matrix which rotate object specific angle on X direction</returns>
        public static Matrix4 RotateX(double angle)
        {
            var rotateX = new Matrix4
            {
                Type = MatrixType.Rotation
            };
            rotateX.Identity();
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);
            rotateX.Matrix[1, 1] = cos;
            rotateX.Matrix[1, 2] = sin;

            rotateX.Matrix[2, 1] = -sin;
            rotateX.Matrix[2, 2] = cos;
            return rotateX;
        }

        /// <summary>
        ///     get a matrix used to rotate object specific angle on Y direction
        /// </summary>
        /// <param name="angle">rotate angle</param>
        /// <returns>matrix which rotate object specific angle on Y direction</returns>
        public static Matrix4 RotateY(double angle)
        {
            var rotateX = new Matrix4
            {
                Type = MatrixType.Rotation
            };
            rotateX.Identity();
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);
            rotateX.Matrix[0, 0] = cos;
            rotateX.Matrix[0, 2] = -sin;

            rotateX.Matrix[2, 0] = sin;
            rotateX.Matrix[2, 2] = cos;
            return rotateX;
        }

        /// <summary>
        ///     get a matrix used to rotate object specific angle on Z direction
        /// </summary>
        /// <param name="angle">rotate angle</param>
        /// <returns>matrix which rotate object specific angle on Z direction</returns>
        public static Matrix4 RotateZ(double angle)
        {
            var rotateX = new Matrix4
            {
                Type = MatrixType.Rotation
            };
            rotateX.Identity();
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);
            rotateX.Matrix[1, 1] = cos;
            rotateX.Matrix[1, 2] = sin;

            rotateX.Matrix[2, 1] = -sin;
            rotateX.Matrix[2, 2] = cos;
            return rotateX;
        }

        /// <summary>
        ///     Identity matrix
        /// </summary>
        public void Identity()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                Matrix[i, j] = 0.0f;
            Matrix[0, 0] = 1.0f;
            Matrix[1, 1] = 1.0f;
            Matrix[2, 2] = 1.0f;
            Matrix[3, 3] = 1.0f;
        }

        /// <summary>
        ///     multiply matrix left and right
        /// </summary>
        /// <param name="left">left matrix</param>
        /// <param name="right">right matrix</param>
        /// <returns></returns>
        public static Matrix4 Multiply(Matrix4 left, Matrix4 right)
        {
            var result = new Matrix4();
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                result[i, j] = left[i, 0] * right[0, j] + left[i, 1] * right[1, j]
                                                        + left[i, 2] * right[2, j] + left[i, 3] * right[3, j];
            return result;
        }

        /// <summary>
        ///     transform point using this matrix
        /// </summary>
        /// <param name="point">point to be transformed</param>
        /// <returns>transform result</returns>
        public Vector4 Transform(Vector4 point)
        {
            return new Vector4(point.X * this[0, 0] + point.Y * this[1, 0]
                                                    + point.Z * this[2, 0] + point.W * this[3, 0],
                point.X * this[0, 1] + point.Y * this[1, 1]
                                     + point.Z * this[2, 1] + point.W * this[3, 1],
                point.X * this[0, 2] + point.Y * this[1, 2]
                                     + point.Z * this[2, 2] + point.W * this[3, 2]);
        }

        /// <summary>
        ///     if m_matrix is a rotation matrix,this method can get the rotation inverse matrix.
        /// </summary>
        /// <returns>inverse of rotation matrix</returns>
        public Matrix4 RotationInverse()
        {
            return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]),
                new Vector4(this[0, 1], this[1, 1], this[2, 1]),
                new Vector4(this[0, 2], this[1, 2], this[2, 2]));
        }

        /// <summary>
        ///     if this m_matrix is a translation matrix,
        ///     this method can get the translation inverse matrix.
        /// </summary>
        /// <returns>inverse of translation matrix</returns>
        public Matrix4 TranslationInverse()
        {
            return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]));
        }

        /// <summary>
        ///     get inverse matrix
        /// </summary>
        /// <returns>inverse matrix</returns>
        public Matrix4 Inverse()
        {
            switch (Type)
            {
                case MatrixType.Rotation: return RotationInverse();

                case MatrixType.Translation: return TranslationInverse();

                case MatrixType.RotationAndTranslation:
                    return Multiply(TranslationInverse(), RotationInverse());

                case MatrixType.Scale: return ScaleInverse();

                case MatrixType.Normal: return new Matrix4();

                default: return null;
            }
        }

        /// <summary>
        ///     if  m_matrix is a scale matrix,this method can get the scale inverse matrix.
        /// </summary>
        /// <returns>inverse of scale matrix</returns>
        public Matrix4 ScaleInverse()
        {
            return new Matrix4(1 / Matrix[0, 0]);
        }
    }
}
