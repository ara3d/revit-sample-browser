// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
namespace Ara3D.RevitSampleBrowser.PathReinforcement.CS
{
    /// <summary>
    ///     Vector4 class is used to store vector
    ///     and contain method to handle the vector
    /// </summary>
    public class Vector4
    {
        public Vector4(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     constructor, transform Autodesk.Revit.DB.XYZ to vector
        /// </summary>
        public Vector4(XYZ v)
        {
            X = (float)v.X;
            Y = (float)v.Y;
            Z = (float)v.Z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; } = 1.0f;

        public static Vector4 operator +(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.X + vb.X, va.Y + vb.Y, va.Z + vb.Z);
        }

        public static Vector4 operator -(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.X - vb.X, va.Y - vb.Y, va.Z - vb.Z);
        }

        public static Vector4 operator *(Vector4 v, float factor)
        {
            return new Vector4(v.X * factor, v.Y * factor, v.Z * factor);
        }

        public static Vector4 operator /(Vector4 v, float factor)
        {
            return new Vector4(v.X / factor, v.Y / factor, v.Z / factor);
        }

        public float DotProduct(Vector4 v)
        {
            return (X * v.X) + (Y * v.Y) + (Z * v.Z);
        }

        public Vector4 CrossProduct(Vector4 v)
        {
            return new Vector4((Y * v.Z) - (Z * v.Y), (Z * v.X)
                                                  - (X * v.Z), (X * v.Y) - (Y * v.X));
        }

        public static float DotProduct(Vector4 va, Vector4 vb)
        {
            return (va.X * vb.X) + (va.Y * vb.Y) + (va.Z * vb.Z);
        }

        public static Vector4 CrossProduct(Vector4 va, Vector4 vb)
        {
            return new Vector4((va.Y * vb.Z) - (va.Z * vb.Y), (va.Z * vb.X)
                                                          - (va.X * vb.Z), (va.X * vb.Y) - (va.Y * vb.X));
        }

        public void Normalize()
        {
            var length = Length();
            if (length == 0) length = 1;
            X /= length;
            Y /= length;
            Z /= length;
        }

        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }
    }

    /// <summary>
    ///     Matrix used to transform between ucs coordinate and world coordinate.
    /// </summary>
    public class Matrix4
    {
        public enum MatrixType
        {
            Rotation,

            Translation,

            Scale,

            RotationAndTranslation,

            Normal
        }

        private readonly float[,] m_matrix = new float[4, 4];
        private readonly MatrixType m_type;

        public Matrix4()
        {
            m_type = MatrixType.Normal;
            Identity();
        }

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

        public Matrix4(Vector4 origin)
        {
            m_type = MatrixType.Translation;
            Identity();
            m_matrix[3, 0] = origin.X;
            m_matrix[3, 1] = origin.Y;
            m_matrix[3, 2] = origin.Z;
        }

        public Matrix4(Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 origin)
        {
            m_type = MatrixType.RotationAndTranslation;
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

        public Matrix4(float scale)
        {
            m_type = MatrixType.Scale;
            Identity();
            m_matrix[0, 0] = m_matrix[1, 1] = m_matrix[2, 2] = scale;
        }

        public float this[int row, int column]
        {
            get => m_matrix[row, column];
            set => m_matrix[row, column] = value;
        }

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

        public static Matrix4 Multiply(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = new();
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    result[i, j] = (left[i, 0] * right[0, j]) + (left[i, 1] * right[1, j])
                                                            + (left[i, 2] * right[2, j]) + (left[i, 3] * right[3, j]);
            return result;
        }

        public Vector4 Transform(Vector4 point)
        {
            return new Vector4((point.X * this[0, 0]) + (point.Y * this[1, 0])
                                                    + (point.Z * this[2, 0]) + (point.W * this[3, 0]),
                (point.X * this[0, 1]) + (point.Y * this[1, 1])
                                     + (point.Z * this[2, 1]) + (point.W * this[3, 1]),
                (point.X * this[0, 2]) + (point.Y * this[1, 2])
                                     + (point.Z * this[2, 2]) + (point.W * this[3, 2]));
        }

        /// <summary>
        ///     if m_matrix is a rotation matrix,this method can get the rotation inverse matrix.
        /// </summary>
        /// <returns>inversed rotation matrix</returns>
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
        /// <returns>inversed translation matrix</returns>
        public Matrix4 TranslationInverse()
        {
            return new Matrix4(new Vector4(-this[3, 0], -this[3, 1], -this[3, 2]));
        }

        public Matrix4 Inverse()
        {
            return m_type switch
            {
                MatrixType.Rotation => RotationInverse(),
                MatrixType.Translation => TranslationInverse(),
                MatrixType.RotationAndTranslation => Multiply(TranslationInverse(), RotationInverse()),
                MatrixType.Scale => ScaleInverse(),
                MatrixType.Normal => new Matrix4(),
                _ => null,
            };
        }

        /// <summary>
        ///     if m_matrix is a scale matrix,this method can get the scale inverse matrix.
        /// </summary>
        /// <returns> inversed scale matrix</returns>
        public Matrix4 ScaleInverse()
        {
            return new Matrix4(1 / m_matrix[0, 0]);
        }
    }
}
