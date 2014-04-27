using MCFire.Modules.Infrastructure.Models;
using Substrate;
using Substrate.Core;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class NbtWorldExtensions
    {
        /// <summary>
        /// Returns the block at the specified position.
        /// Returns air (0) if the coord is in the void, above the sky limit, if the chunk doesn't exist or hasn't been created.
        /// </summary>
        public static AlphaBlock GetBlock(this IChunkManager world, int x, int y, int z)
        {
            if (y < 0) return new AlphaBlock(BlockType.AIR);
            var chunkPos = new Point(x, z).ToChunkSpace();
            var chunk = world.GetChunkRef(chunkPos.X, chunkPos.Y);
            if (chunk == null) return new AlphaBlock(BlockType.AIR);
            var blocks = chunk.Blocks;
            return y < blocks.YDim ? blocks.GetBlock(x.Mod(16), y, z.Mod(16)) : new AlphaBlock(BlockType.AIR);
        }
    }
}
