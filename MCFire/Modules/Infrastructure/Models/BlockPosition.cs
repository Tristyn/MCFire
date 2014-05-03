using System;

namespace MCFire.Modules.Infrastructure.Models
{
    public struct BlockPosition
    {

        public BlockPosition(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public BlockPosition(ChunkPosition chunk, int localX, int localY, int localZ)
            : this()
        {
            X = chunk.ChunkX * 16 + localX;
            Y = localY;
            Z = chunk.ChunkZ * 16 + localZ;
        }

        public static implicit operator ChunkPosition(BlockPosition value)
        {
            return new ChunkPosition(value.X >> 4, value.Z >> 4);
        }

        public static bool operator ==(BlockPosition left, BlockPosition right)
        {
            return left.X == right.X && left.Z == right.Z && left.Y == right.Y;
        }

        public static bool operator !=(BlockPosition left, BlockPosition right)
        {
            return left.X != right.X || left.Z != right.Z || left.Y != right.Y;
        }

        public bool Equals(BlockPosition other)
        {
            return X == other.X && Z == other.Z && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkPositionDimension && Equals((ChunkPositionDimension)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Y;
                hashCode = (hashCode * 397) ^ X;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", X, Y, Z);
        }

        public int Y
        {
            get;
            private set;
        }

        public int X { get; private set; }

        public int Z { get; private set; }
    }
}