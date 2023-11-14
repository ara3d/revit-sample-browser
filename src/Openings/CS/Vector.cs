// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;

namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    ///     Point class use to store point coordinate value
    ///     and get the value via (x, y ,z)property
    /// </summary>
    public struct Vector
    {
        /// <summary>
        ///     x coordinate of vector
        /// </summary>
        private double m_x;

        /// <summary>
        ///     y coordinate of vector
        /// </summary>
        private double m_y;

        /// <summary>
        ///     z coordinate of vector
        /// </summary>
        private double m_z;

        /// <summary>
        ///     Property to get X coordinate
        /// </summary>
        public double X
        {
            get => m_x;
            set => m_x = value;
        }

        /// <summary>
        ///     Property to get Y coordinate
        /// </summary>
        public double Y
        {
            get => m_y;
            set => m_y = value;
        }

        /// <summary>
        ///     Property to get Z coordinate
        /// </summary>
        public double Z
        {
            get => m_z;
            set => m_z = value;
        }

        /// <summary>
        ///     Property to get x, y, z coordinate bu index 1, 2, 3
        /// </summary>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return m_x;
                    case 1:
                        return m_y;
                    case 2:
                        return m_z;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        m_x = value;
                        break;
                    case 1:
                        m_y = value;
                        break;
                    case 2:
                        m_z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     copy constructor
        /// </summary>
        public Vector(Vector rhs)
        {
            m_x = rhs.X;
            m_y = rhs.Y;
            m_z = rhs.Z;
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="x">x coordinate of point</param>
        /// <param name="y">y coordinate of point</param>
        /// <param name="z">z coordinate of point</param>
        public Vector(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        /// <summary>
        ///     get Normal by vector
        /// </summary>
        public Vector GetNormal()
        {
            var direct = new Vector();
            var len = GetLength();
            direct.X = m_x / len;
            direct.Y = m_y / len;
            direct.Z = m_z / len;
            return direct;
        }

        /// <summary>
        ///     add two vector
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns>add two vector</returns>
        public static Vector operator +(Vector lhs, Vector rhs)
        {
            var result = new Vector(lhs);
            result.X += rhs.X;
            result.Y += rhs.Y;
            result.Z += rhs.Z;
            return result;
        }

        /// <summary>
        ///     subtraction of two vector
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns>subtraction of two vector</returns>
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            var result = new Vector(lhs);
            result.X -= rhs.X;
            result.Y -= rhs.Y;
            result.Z -= rhs.Z;
            return result;
        }

        /// <summary>
        ///     negative of vector
        /// </summary>
        /// <param name="lhs">vector</param>
        /// <returns>negative of vector</returns>
        public static Vector operator -(Vector lhs)
        {
            var result = new Vector(lhs)
            {
                X = -lhs.X,
                Y = -lhs.Y,
                Z = -lhs.Z
            };
            return result;
        }

        /// <summary>
        ///     get normal vector of two vector
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns> normal vector of two vector</returns>
        public static Vector operator &(Vector lhs, Vector rhs)
        {
            var v1 = lhs.X;
            var v2 = lhs.Y;
            var v3 = lhs.Z;

            var u1 = rhs.X;
            var u2 = rhs.Y;
            var u3 = rhs.Z;

            var x = v2 * u3 - v3 * u2;
            var y = v3 * u1 - v1 * u3;
            var z = v1 * u2 - v2 * u1;

            return new Vector(x, y, z);
        }

        /// <summary>
        ///     get cross vector of two vector
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns> cross vector of two vector</returns>
        public static double operator *(Vector lhs, Vector rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
        }

        /// <summary>
        ///     get vector multiply by an double value
        /// </summary>
        /// <param name="lhs">vector</param>
        /// <param name="rhs">double value</param>
        /// <returns> vector multiply by an double value</returns>
        public static Vector operator *(Vector lhs, double rhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        /// <summary>
        ///     estimate whether two are unequal
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns> whether two are unequal</returns>
        public static bool operator !=(Vector lhs, Vector rhs)
        {
            return !IsEqual(lhs, rhs);
        }

        /// <summary>
        ///     estimate whether two are equal
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns> whether two are equal</returns>
        public static bool operator ==(Vector lhs, Vector rhs)
        {
            return IsEqual(lhs, rhs);
        }

        /// <summary>
        ///     get the length of vector
        /// </summary>
        /// <param name="lhs">vector</param>
        /// <returns>length of vector</returns>
        public static double operator ~(Vector lhs)
        {
            return lhs.GetLength();
        }

        /// <summary>
        ///     get vector divided by an double value
        /// </summary>
        /// <param name="lhs">vector</param>
        /// <param name="rhs">double value</param>
        /// <returns> vector divided by an double value</returns>
        public static Vector operator /(Vector lhs, double rhs)
        {
            return new Vector(lhs.m_x / rhs, lhs.m_y / rhs, lhs.m_z / rhs);
        }

        /// <summary>
        ///     get angle of two vector
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns> angle of two vector</returns>
        public static double GetAngleOf2Vectors(Vector lhs, Vector rhs, bool acuteAngleDesired)
        {
            var angle = Math.Acos(lhs.GetNormal() * rhs.GetNormal());
            if (acuteAngleDesired && angle > Math.PI / 2) angle = Math.PI - angle;
            return angle;
        }

        /// <summary>
        ///     estimate whether two are equal
        /// </summary>
        /// <param name="obj">object which compare with</param>
        /// <returns> whether two are equal</returns>
        public override bool Equals(object obj)
        {
            try
            {
                var rhs = (Vector)obj;
                return IsEqual(this, rhs);
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        ///     Get HashCode
        /// </summary>
        public override int GetHashCode()
        {
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }

        /// <summary>
        ///     Get Length of vector
        /// </summary>
        public double GetLength()
        {
            return Math.Sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
        }

        /// <summary>
        ///     estimate whether two vector are equal
        /// </summary>
        /// <param name="lhs">first vector</param>
        /// <param name="rhs">second vector</param>
        /// <returns> whether two are equal</returns>
        private static bool IsEqual(Vector lhs, Vector rhs)
        {
            return lhs.X == rhs.X && lhs.X == rhs.X && lhs.X == rhs.X;
        }
    }
}
