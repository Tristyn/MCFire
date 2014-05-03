namespace MCFire.Modules.Infrastructure.Models
{
    public struct BlockPosition
    {
        readonly int _x;
        readonly int _y;
        readonly int _z;

        public BlockPosition(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public BlockPosition(ChunkPosition chunk, int localX, int localY, int localZ)
        {
            _x = chunk.ChunkX * 16 + localX;
            _y = localY;
            _z = chunk.ChunkZ * 16 + localZ;
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
            return _x == other._x && _z == other._z && _y == other._y;
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
                int hashCode = _y;
                hashCode = (hashCode * 397) ^ _x;
                hashCode = (hashCode * 397) ^ _z;
                return hashCode;
            }
        }

        public int Y
        {
            get { return _y; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Z
        {
            get { return _z; }
        }
        public ChunkPosition ChunkPosition
        {
            get { return new ChunkPosition(_x, _z); }
        }
    }
}