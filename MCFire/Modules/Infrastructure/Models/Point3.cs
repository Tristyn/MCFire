using System;
using SharpDX;
using SharpDX.Serialization;

namespace MCFire.Modules.Infrastructure.Models
{
    /// <summary>
    /// 3D point.
    /// </summary>
    [Serializable]
    public struct Point3 : IEquatable<Point3>, IDataSerializable
    {
        /// <summary>
        /// Left coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// Top coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// Forward coordinate
        /// </summary>
        public int Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static explicit operator Point3(Vector3 value)
        {
            return new Point3((int) value.X, (int) value.Y, (int) value.Z);
        }

        public static implicit operator Vector3(Point3 value)
        {
            return new Vector3((float) value.X, (float) value.Y, (float) value.Z);
        }

        public static bool operator ==(Point3 left, Point3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point3 left, Point3 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point3 && Equals((Point3) obj);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", X, Y, Z);
        }

        void IDataSerializable.Serialize(BinarySerializer serializer)
        {
            if (serializer.Mode == SerializerMode.Write)
            {
                serializer.Writer.Write(X);
                serializer.Writer.Write(Y);
                serializer.Writer.Write(Z);
            }
            else
            {
                X = serializer.Reader.ReadInt32();
                Y = serializer.Reader.ReadInt32();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode*397) ^ Y;
                hashCode = (hashCode*397) ^ Z;
                return hashCode;
            }
        }
    }
}
