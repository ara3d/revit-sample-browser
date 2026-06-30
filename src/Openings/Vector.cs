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
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentOutOfRangeException(),
            };
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Vector(Vector rhs)
        {
            X = rhs.X;
            Y = rhs.Y;
            Z = rhs.Z;
        }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector GetNormal()
        {
            Vector direct = new();
            var len = GetLength();
            direct.X = X / len;
            direct.Y = Y / len;
            direct.Z = Z / len;
            return direct;
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            Vector result = new(lhs);
            result.X += rhs.X;
            result.Y += rhs.Y;
            result.Z += rhs.Z;
            return result;
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            Vector result = new(lhs);
            result.X -= rhs.X;
            result.Y -= rhs.Y;
            result.Z -= rhs.Z;
            return result;
        }

        public static Vector operator -(Vector lhs)
        {
            Vector result = new(lhs)
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

            var x = (v2 * u3) - (v3 * u2);
            var y = (v3 * u1) - (v1 * u3);
            var z = (v1 * u2) - (v2 * u1);

            return new Vector(x, y, z);
        }

        public static double operator *(Vector lhs, Vector rhs)
        {
            return (lhs.X * rhs.X) + (lhs.Y * rhs.Y) + (lhs.Z * rhs.Z);
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
            return new Vector(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public double GetLength()
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        private static bool IsEqual(Vector lhs, Vector rhs)
        {
            return lhs.X == rhs.X && lhs.X == rhs.X && lhs.X == rhs.X;
        }
    }
}
