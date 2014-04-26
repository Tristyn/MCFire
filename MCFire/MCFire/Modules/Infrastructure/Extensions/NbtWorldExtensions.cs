using MCFire.Modules.Infrastructure.Models;
using Substrate;
using Substrate.Core;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public  static class NbtWorldExtensions
    {
        /// <summary>
        /// Returns the block at the specified position.
        /// Returns air (0) if the coord is in the void, above the sky limit, if the chunk doesn't exist or hasn't been created.
        /// </summary>
        public static AlphaBlock GetBlock(this IChunkManager world,int x, int y, int z)
        {
            if (y < 0) return new AlphaBlock(BlockType.AIR);
            var chunkPos = new Point(x / 16, z / 16);

            var chunk = world.GetChunk(chunkPos);
            if (chunk == null) return new AlphaBlock(BlockType.AIR);
            var chunkRef = chunk.ChunkRef;
            if (chunkRef == null) return new AlphaBlock(BlockType.AIR);
            var blocks = chunkRef.Blocks;
            if (y >= blocks.YDim) return new AlphaBlock(BlockType.AIR);
            return blocks.GetBlock(Mod(x, 16), y, Mod(z, 16));
        }
    }
}
