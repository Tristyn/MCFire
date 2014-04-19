using SharpDX;
using Substrate;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class ChunkRefExtensions
    {
        public static Point ChunkPosition(this ChunkRef chunk)
        {
            return new Point(chunk.X, chunk.Z);
        }
    }
}
