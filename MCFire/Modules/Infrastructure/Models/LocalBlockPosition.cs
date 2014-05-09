using System;
using MCFire.Modules.Infrastructure.Extensions;
using Substrate.Core;

namespace MCFire.Modules.Infrastructure.Models
{
    public struct LocalBlockPosition
    {
        public LocalBlockPosition(ChunkPosition chunk, BlockPosition position, ChunkSize chunkSize)
            : this(position.X - chunk.ChunkX * chunkSize.X, position.Y, position.Z - chunk.ChunkZ * chunkSize.Y)
        {
        }

        public LocalBlockPosition(IChunk chunk, BlockPosition position)
            : this()
        {
            var chunkPos = chunk.Position();
            var size = chunk.Size();
            X = position.X - chunkPos.ChunkX * size.X;
            Y = position.Y;
            Z = position.Z - chunkPos.ChunkZ * size.Z;
        }

        public LocalBlockPosition(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(LocalBlockPosition left, LocalBlockPosition right)
        {
            return left.X == right.X && left.Z == right.Z && left.Y == right.Y;
        }

        public static bool operator !=(LocalBlockPosition left, LocalBlockPosition right)
        {
            return left.X != right.X || left.Z != right.Z || left.Y != right.Y;
        }

        public bool Equals(LocalBlockPosition other)
        {
            return X == other.X && Z == other.Z && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LocalBlockPosition && Equals((LocalBlockPosition)obj);
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

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Z { get; private set; }
    }
}
