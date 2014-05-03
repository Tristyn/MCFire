﻿using System;

namespace MCFire.Modules.Infrastructure.Models
{
    public struct ChunkPosition : IEquatable<ChunkPosition>
    {
        readonly int _chunkX;
        readonly int _chunkZ;

        public ChunkPosition(int chunkX, int chunkZ)
        {
            _chunkX = chunkX;
            _chunkZ = chunkZ;
        }

        public static implicit operator ChunkPosition(ChunkPositionDimension value)
        {
            return new ChunkPosition(value.ChunkX,value.ChunkZ);
        }

        public static bool operator ==(ChunkPosition left, ChunkPosition right)
        {
            return left.ChunkX == right.ChunkX && left.ChunkZ == right.ChunkZ;
        }

        public static bool operator !=(ChunkPosition left, ChunkPosition right)
        {
            return left.ChunkX != right.ChunkX || left.ChunkZ != right.ChunkZ;
        }


        #region ChunkPosition Operators

        public static ChunkPosition operator +(ChunkPosition left, ChunkPosition right)
        {
            return new ChunkPosition(left.ChunkX + right.ChunkX, left.ChunkZ + right.ChunkZ);
        }

        public static ChunkPosition operator -(ChunkPosition left, ChunkPosition right)
        {
            return new ChunkPosition(left.ChunkX - right.ChunkX, left.ChunkZ - right.ChunkZ);
        }

        #endregion

        #region Int Operators

        public static ChunkPosition operator +(ChunkPosition left, int right)
        {
            return new ChunkPosition(left.ChunkX + right, left.ChunkZ + right);
        }

        public static ChunkPosition operator -(ChunkPosition left, int right)
        {
            return new ChunkPosition(left.ChunkX - right, left.ChunkZ - right);
        }

        #endregion

        public bool Equals(ChunkPosition other)
        {
            return _chunkX == other._chunkX && _chunkZ == other._chunkZ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChunkPosition && Equals((ChunkPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_chunkX * 397) ^ _chunkZ;
            }
        }

        public int ChunkX
        {
            get { return _chunkX; }
        }

        public int ChunkZ
        {
            get { return _chunkZ; }
        }
    }
}