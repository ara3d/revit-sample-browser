// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    /// <summary>
    ///     Point class use to store point coordinate value
    ///     and get the value via (x, y ,z)property
    /// </summary>
    public struct Vector
    {
        private double m_x;

        private double m_y;

        private double m_z;

        public double X
        {
            get => m_x;
            set => m_x = value;
        }

        public double Y
        {
            get => m_y;
            set => m_y = value;
        }

        public double Z
        {
            get => m_z;
            set => m_z = value;
        }

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

        public Vector(Vector rhs)
        {
            m_x = rhs.X;
            m_y = rhs.Y;
            m_z = rhs.Z;
        }

        public Vector(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        public Vector GetNormal()
        {
            var direct = new Vector();
            var len = GetLength();
            direct.X = m_x / len;
            direct.Y = m_y / len;
            direct.Z = m_z / len;
            return direct;
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            var result = new Vector(lhs);
            result.X += rhs.X;
            result.Y += rhs.Y;
            result.Z += rhs.Z;
            return result;
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            var result = new Vector(lhs);
            result.X -= rhs.X;
            result.Y -= rhs.Y;
            result.Z -= rhs.Z;
            return result;
        }

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

        public static double operator *(Vector lhs, Vector rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
        }

        public static Vector operator *(Vector lhs, double rhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public static bool operator !=(Vector lhs, Vector rhs)
        {
            return !IsEqual(lhs, rhs);
        }

        public static bool operator ==(Vector lhs, Vector rhs)
        {
            return IsEqual(lhs, rhs);
        }

        public static double operator ~(Vector lhs)
        {
            return lhs.GetLength();
        }

        public static Vector operator /(Vector lhs, double rhs)
        {
            return new Vector(lhs.m_x / rhs, lhs.m_y / rhs, lhs.m_z / rhs);
        }

        public static double GetAngleOf2Vectors(Vector lhs, Vector rhs, bool acuteAngleDesired)
        {
            var angle = Math.Acos(lhs.GetNormal() * rhs.GetNormal());
            if (acuteAngleDesired && angle > Math.PI / 2) angle = Math.PI - angle;
            return angle;
        }

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

        public override int GetHashCode()
        {
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }

        public double GetLength()
        {
            return Math.Sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
        }

        private static bool IsEqual(Vector lhs, Vector rhs)
        {
            return lhs.X == rhs.X && lhs.X == rhs.X && lhs.X == rhs.X;
        }
    }
}
