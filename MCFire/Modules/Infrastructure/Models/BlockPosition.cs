using System;
using MCFire.Modules.Infrastructure.Extensions;
using SharpDX;

namespace MCFire.Modules.Infrastructure.Models
{
    /// <summary>
    /// The exact coordinate for a block in world-space.
    /// </summary>
    public struct BlockPosition
    {
        public BlockPosition(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public BlockPosition(ChunkPosition chunkPosition, LocalBlockPosition local, ChunkSize size)
            : this()
        {
            X = local.X + chunkPosition.ChunkX * size.X;
            Y = local.Y;
            Z = local.Z + chunkPosition.ChunkZ * size.Y;
        }

        public static implicit operator BlockPosition(Vector3 value)
        {
            return new BlockPosition((int)value.X, (int)value.Y, (int)value.Z);
        }

        public static implicit operator Vector3(BlockPosition value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        public static implicit operator BlockPosition(Point3 value)
        {
            return new BlockPosition(value.X, value.Y, value.Z);
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
            return obj is BlockPosition && Equals((BlockPosition)obj);
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

        public int Y { get; private set; }

        public int X { get; private set; }

        public int Z { get; private set; }
    }
}