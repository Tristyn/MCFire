using SharpDX;
using SharpDX.Serialization;
using System;

namespace MCFire.Modules.Infrastructure.Models
{
    [Serializable]
    public struct Point : IEquatable<Point>, IDataSerializable
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Point(Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        public static implicit operator Vector2(Point value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point operator *(Point left, Point right)
        {
            return new Point(left.X * right.X, left.Y * right.Y);
        }

        public static Point operator /(Point left, Point right)
        {
            return new Point(left.X / right.X, left.Y / right.Y);
        }

        public static Point operator +(Point left, int right)
        {
            return new Point(left.X + right, left.Y + right);
        }

        public static Point operator -(Point left, int right)
        {
            return new Point(left.X - right, left.Y - right);
        }

        public static Point operator *(Point left, int right)
        {
            return new Point(left.X * right, left.Y * right);
        }

        public static Point operator /(Point left, int right)
        {
            return new Point(left.X / right, left.Y / right);
        }

        public Point ToChunkSpace()
        {
            return new Point((int)Math.Floor((double)X / 16), (int)Math.Floor((double)Y / 16));
        }

        public bool Equals(Point other)
        {
            if (other.X == X)
                return other.Y == Y;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj.GetType() != typeof(Point))
                return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return X * 397 ^ Y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        void IDataSerializable.Serialize(BinarySerializer serializer)
        {
            if (serializer.Mode == SerializerMode.Write)
            {
                serializer.Writer.Write(X);
                serializer.Writer.Write(Y);
            }
            else
            {
                X = serializer.Reader.ReadInt32();
                Y = serializer.Reader.ReadInt32();
            }
        }
    }
}
