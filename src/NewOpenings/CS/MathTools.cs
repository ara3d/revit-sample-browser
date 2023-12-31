// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    /// <summary>
    ///     Vector4 class use to store vector
    ///     and contain method to handle the vector
    /// </summary>
    public class Vector4
    {
        /// <summary>
        ///     The coordinate w value
        /// </summary>
        private float m_w = 1.0f;

        /// <summary>
        ///     The coordinate x value
        /// </summary>
        private float m_x;

        /// <summary>
        ///     The coordinate y value
        /// </summary>
        private float m_y;

        /// <summary>
        ///     The coordinate z value
        /// </summary>
        private float m_z;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Vector4(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        public Vector4(Vector4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        ///     Constructor, transform Autodesk.Revit.DB.XYZ to vector
        /// </summary>
        public Vector4(XYZ v)
        {
            X = (float)v.X;
            Y = (float)v.Y;
            Z = (float)v.Z;
        }

        /// <summary>
        ///     The coordinate x value
        /// </summary>
        public float X
        {
            get => m_x;
            set => m_x = value;
        }

        /// <summary>
        ///     The coordinate y value
        /// </summary>
        public float Y
        {
            get => m_y;
            set => m_y = value;
        }

        /// <summary>
        ///     The coordinate z value
        /// </summary>
        public float Z
        {
            get => m_z;
            set => m_z = value;
        }

        /// <summary>
        ///     The coordinate w value
        /// </summary>
        public float W
        {
            get => m_w;
            set => m_w = value;
        }

        /// <summary>
        ///     Add two vector
        /// </summary>
        /// <param name="va">First vector</param>
        /// <param name="vb">Second vector</param>
        public static Vector4 operator +(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z);
        }

        /// <summary>
        ///     Subtraction of two vector
        /// </summary>
        /// <param name="va">First vector</param>
        /// <param name="vb">Second vector</param>
        /// <returns>Subtraction of two vector</returns>
        public static Vector4 operator -(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z);
        }

        /// <summary>
        ///     Get vector multiply by an double value
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="factor">Double value</param>
        /// <returns> Vector multiply by an double value</returns>
        public static Vector4 operator *(Vector4 v, float factor)
        {
            return new Vector4(v.X * factor, v.Y * factor, v.Z * factor);
        }

        /// <summary>
        ///     get vector divided by an double value
        /// </summary>
        /// <param name="v">vector</param>
        /// <param name="factor">double value</param>
        /// <returns> vector divided by an double value</returns>
        public static Vector4 operator /(Vector4 v, float factor)
        {
            return new Vector4(v.X / factor, v.Y / factor, v.Z / factor);
        }

        /// <summary>
        ///     dot multiply vector
        /// </summary>
        /// <param name="v">vector</param>
        public float DotProduct(Vector4 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        /// <summary>
        ///     get normal vector of two vector
        /// </summary>
        /// <param name="v">second vector</param>
        /// <returns> normal vector of two vector</returns>
        public Vector4 CrossProduct(Vector4 v)
        {
            return new Vector4(Y * v.Z - Z * v.Y, Z * v.X
                                                  - X * v.Z, X * v.Y - Y * v.X);
        }

        /// <summary>
        ///     Dot multiply two vector
        /// </summary>
        /// <param name="va">First vector</param>
        /// <param name="vb">Second vector</param>
        public static float DotProduct(Vector4 va, Vector4 vb)
        {
            return va.X * vb.X + va.Y * vb.Y + va.Z * vb.Z;
        }

        /// <summary>
        ///     Get normal vector of two vector
        /// </summary>
        /// <param name="va">First vector</param>
        /// <param name="vb">Second vector</param>
        /// <returns> Normal vector of two vector</returns>
        public static Vector4 CrossProduct(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X
                                                          - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X);
        }

        /// <summary>
        ///     Get unit vector
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
        ///     Calculate the length of vector
        /// </summary>
        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
    }

    /// <summary>
    ///     Matrix used to transform between UCS coordinate and world coordinate.
    /// </summary>
    public class Matrix4
    {
        /// <summary>
        ///     matrix algorithm
        /// </summary>
        public enum MatrixType
        {
            /// <summary>
            ///     rotation matrix
            /// </summary>
            Rotation,

            /// <summary>
            ///     translation matrix
            /// </summary>
            TransLation,

            /// <summary>
            ///     scale matrix
            /// </summary>
            Scale,

            /// <summary>
            ///     rotation and translation mix matrix
            /// </summary>
            RotationAndTransLation,

            /// <summary>
            ///     identity matrix
            /// </summary>
            Normal
        }

        private readonly float[,] m_matrix = new float[4, 4];
        private readonly MatrixType m_type;

        /// <summary>
        ///     Construct a identity matrix
        /// </summary>
        public Matrix4()
        {
            m_type = MatrixType.Normal;
            Identity();
        }

        /// <summary>
        ///     Construct a rotation matrix,origin at (0,0,0)
        /// </summary>
        /// <param name="xAxis">identity of x axis</param>
        /// <param name="yAxis">identity of y axis</param>
        /// <param name="zAxis">identity of z axis</param>
        public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis)
        {
            m_type = MatrixType.Rotation;
            Identity();
            m_matrix[0, 0] = xAxis.X;
            m_matrix[0, 1] = xAxis.Y;
            m_matrix[0, 2] = xAxis.Z;
            m_matrix[1, 0] = yAxis.X;
            m_matrix[1, 1] = yAxis.Y;
            m_matrix[1, 2] = yAxis.Z;
            m_matrix[2, 0] = zAxis.X;
            m_matrix[2, 1] = zAxis.Y;
            m_matrix[2, 2] = zAxis.Z;
        }

        /// <summary>
        ///     ctor,translation matrix.
        /// </summary>
        /// <param name="origin">origin of UCS in world coordinate</param>
        public Matrix4(Vector4 origin)
        {
            m_type = MatrixType.TransLation;
            Identity();
            m_matrix[3, 0] = origin.X;
            m_matrix[3, 1] = origin.Y;
            m_matrix[3, 2] = origin.Z;
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
            m_type = MatrixType.RotationAndTransLation;
            Identity();
            m_matrix[0, 0] = xAxis.X;
            m_matrix[0, 1] = xAxis.Y;
            m_matrix[0, 2] = xAxis.Z;
            m_matrix[1, 0] = yAxis.X;
            m_matrix[1, 1] = yAxis.Y;
            m_matrix[1, 2] = yAxis.Z;
            m_matrix[2, 0] = zAxis.X;
            m_matrix[2, 1] = zAxis.Y;
            m_matrix[2, 2] = zAxis.Z;
            m_matrix[3, 0] = origin.X;
            m_matrix[3, 1] = origin.Y;
            m_matrix[3, 2] = origin.Z;
        }

        /// <summary>
        ///     scale matrix constructor
        /// </summary>
        /// <param name="scale">scale factor</param>
        public Matrix4(float scale)
        {
            m_type = MatrixType.Scale;
            Identity();
            m_matrix[0, 0] = m_matrix[1, 1] = m_matrix[2, 2] = scale;
        }

        /// <summary>
        ///     indexer of matrix
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="column">column number</param>
        /// <returns></returns>
        public float this[int row, int column]
        {
            get => m_matrix[row, column];
            set => m_matrix[row, column] = value;
        }

        /// <summary>
        ///     Identity matrix
        /// </summary>
        public void Identity()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                m_matrix[i, j] = 0.0f;
            m_matrix[0, 0] = 1.0f;
            m_matrix[1, 1] = 1.0f;
            m_matrix[2, 2] = 1.0f;
            m_matrix[3, 3] = 1.0f;
        }

        /// <summary>
        ///     Multiply matrix left and right
        /// </summary>
        /// <param name="left">Left matrix</param>
        /// <param name="right">Right matrix</param>
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
        ///     Transform point use this matrix
        /// </summary>
        /// <param name="point">Point needed to be transformed</param>
        /// <returns>Transform result</returns>
        public Vector4 TransForm(Vector4 point)
        {
            return new Vector4(point.X * this[0, 0] + point.Y * this[1, 0]
                                                    + point.Z * this[2, 0] + point.W * this[3, 0],
                point.X * this[0, 1] + point.Y * this[1, 1]
                                     + point.Z * this[2, 1] + point.W * this[3, 1],
                point.X * this[0, 2] + point.Y * this[1, 2]
                                     + point.Z * this[2, 2] + point.W * this[3, 2]);
        }

        /// <summary>
        ///     If it is a rotation matrix,this method can get the rotation inverse matrix.
        /// </summary>
        /// <returns>rotation inverse matrix</returns>
        public Matrix4 RotationInverse()
        {
            return new Matrix4(new Vector4(this[0, 0], this[1, 0], this[2, 0]),
                new Vector4(this[0, 1], this[1, 1], this[2, 1]),
                new Vector4(this[0, 2], this[1, 2], this[2, 2]));
        }

        /// <summary>
        ///     If it is a translation matrix,
        ///     this method can get the translation inverse matrix.
        /// </summary>
        /// <returns>translation inverse matrix</returns>
        public Matrix4 TransLationInverse()
        {
            return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]));
        }

        /// <summary>
        ///     Get inverse matrix
        /// </summary>
        /// <returns>Inverse matrix</returns>
        public Matrix4 Inverse()
        {
            switch (m_type)
            {
                case MatrixType.Rotation:
                    return RotationInverse();

                case MatrixType.TransLation:
                    return TransLationInverse();

                case MatrixType.RotationAndTransLation:
                    return Multiply(TransLationInverse(), RotationInverse());

                case MatrixType.Scale:
                    return ScaleInverse();

                case MatrixType.Normal:
                    return new Matrix4();

                default: return null;
            }
        }

        /// <summary>
        ///     If it is a scale matrix,this method can get the scale inverse matrix.
        /// </summary>
        /// <returns>Scale inverse matrix</returns>
        public Matrix4 ScaleInverse()
        {
            return new Matrix4(1 / m_matrix[0, 0]);
        }
    }
}
