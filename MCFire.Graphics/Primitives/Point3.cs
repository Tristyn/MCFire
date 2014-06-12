using System;
using SharpDX;

namespace MCFire.Graphics.Primitives
{
    /// <summary>
    /// 3D point.
    /// </summary>
    [Serializable]
    public struct Point3 : IEquatable<Point3>
    {
        /// <summary>
        /// Left coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Top coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// Forward coordinate
        /// </summary>
        public readonly int Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static explicit operator Point3(Vector3 value)
        {
            return new Point3((int)value.X, (int)value.Y, (int)value.Z);
        }

        public static implicit operator Vector3(Point3 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        public static bool operator ==(Point3 left, Point3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point3 left, Point3 right)
        {
            return !left.Equals(right);
        }

        public static Point3 operator +(Point3 left, Point3 right)
        {
            return new Point3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Point3 operator *(Point3 left, Point3 right)
        {
            return new Point3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static Point3 operator -(Point3 left, Point3 right)
        {
            return new Point3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Point3 operator /(Point3 value, Point3 scale)
        {
            return new Point3(value.X / scale.X, value.Y / scale.Y, value.Z / scale.Z);
        }

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point3 && Equals((Point3)obj);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", X, Y, Z);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }
    }
}