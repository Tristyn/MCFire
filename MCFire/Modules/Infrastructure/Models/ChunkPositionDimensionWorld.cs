using System;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;

namespace MCFire.Modules.Infrastructure.Models
{
    public struct ChunkPositionDimensionWorld
    {
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ChunkX;
                hashCode = (hashCode*397) ^ ChunkZ;
                hashCode = (hashCode*397) ^ Dimension;
                hashCode = (hashCode*397) ^ (World != null ? World.GetHashCode() : 0);
                return hashCode;
            }
        }

        public ChunkPositionDimensionWorld(ChunkPositionDimension position, [NotNull] MCFireWorld world)
            : this(position.ChunkX, position.ChunkZ, position.Dimension, world) { }
        public ChunkPositionDimensionWorld(ChunkPosition position, int dimension, [NotNull] MCFireWorld world)
            : this(position.ChunkX, position.ChunkZ, dimension, world) { }
        public ChunkPositionDimensionWorld(int chunkX, int chunkZ, int dimension, [NotNull] MCFireWorld world)
            : this()
        {
            if (world == null) throw new ArgumentNullException("world");

            ChunkX = chunkX;
            ChunkZ = chunkZ;
            Dimension = dimension;
            World = world;
        }

        public static implicit operator ChunkPositionDimension(ChunkPositionDimensionWorld value)
        {
            return new ChunkPositionDimension(value.ChunkX, value.ChunkZ, value.Dimension);
        }

        public static implicit operator ChunkPosition(ChunkPositionDimensionWorld value)
        {
            return new ChunkPosition(value.ChunkX, value.ChunkZ);
        }

        public static bool operator ==(ChunkPositionDimensionWorld left, ChunkPositionDimensionWorld right)
        {
            return left.ChunkX == right.ChunkX && left.ChunkZ == right.ChunkZ && left.Dimension == right.Dimension && left.World == right.World;
        }

        public static bool operator !=(ChunkPositionDimensionWorld left, ChunkPositionDimensionWorld right)
        {
            return left.ChunkX != right.ChunkX || left.ChunkZ != right.ChunkZ || left.Dimension != right.Dimension || left.World != right.World;
        }

        public bool Equals(ChunkPositionDimensionWorld other)
        {
            return ChunkX == other.ChunkX && ChunkZ == other.ChunkZ && Dimension == other.Dimension && Equals(World, other.World);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkPositionDimensionWorld && Equals((ChunkPositionDimensionWorld) obj);
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, Dimension: {2}, Title: {3}", ChunkX, ChunkZ, Dimension, World.Title);
        }

        public int ChunkX { get; private set; }
        public int ChunkZ { get; private set; }
        public int Dimension { get; private set; }
        public MCFireWorld World { get; private set; }
    }
}
