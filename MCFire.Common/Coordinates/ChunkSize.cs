using System;
using Substrate.Core;

namespace MCFire.Common.Coordinates
{
    /// <summary>
    /// The width, length and height of a chunk.
    /// </summary>
    public struct ChunkSize
    {
        public ChunkSize(IChunk chunk)
            : this(chunk.Blocks.XDim, chunk.Blocks.YDim, chunk.Blocks.ZDim)
        {
        }
        public ChunkSize(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(ChunkSize left, ChunkSize right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(ChunkSize left, ChunkSize right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }

        public bool Equals(ChunkSize other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkSize && Equals((ChunkSize)obj);
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

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", X, Y, Z);
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
    }
}
