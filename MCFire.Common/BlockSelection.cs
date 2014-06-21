using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using MCFire.Common.MCFire.Modules.Infrastructure.Models;
using Substrate.Core;

namespace MCFire.Common
{
    /// <summary>
    /// Specifies a box-shaped selection in a minecraft world.
    /// A selection is immutable and can be used to enumerate through the collective data of multiple chunks.
    /// </summary>
    public class BlockSelection : IBlockSelection
    {
        /// <summary>
        /// Creates a new BoxSelection with the specified selection, dimensions and world.
        /// </summary>
        public BlockSelection([NotNull] BoxSelection selection, int dimension, [NotNull] World world)
        {
            Selection = selection;
            Dimension = dimension;
            World = world;
        }

        public BoxSelection Selection { get; private set; }
        public int Dimension { get; private set; }
        public World World { get; private set; }

        // TODO: ive got a serious problem with this class holding onto a reference to World and using it for GetChunks()
        public void GetChunks(AccessMode mode, PartionedChunksFunc chunksAction)
        {
            var lesserChunk = (ChunkPosition)Selection.Lesser;
            var greaterChunk = (ChunkPosition)Selection.Greater;
            // create an enumerator from every chunk that is within the selection
            var chunkPositions = from x in Enumerable.Range(lesserChunk.ChunkX, greaterChunk.ChunkX - lesserChunk.ChunkX + 1)
                                 from z in Enumerable.Range(lesserChunk.ChunkZ, greaterChunk.ChunkZ - lesserChunk.ChunkZ + 1)
                                 select new ChunkPositionDimension(x, z, Dimension);

            // TODO: cant edit huge amounts of chunks (its a list, therefor all loaded at once, check MCFireWorld for more comments on this)
            World.GetChunks(chunkPositions, mode, chunks => chunksAction(PartitionChunks(chunks, Selection)));
        }

        static IEnumerable<IChunkPartition> PartitionChunks(IEnumerable<IChunk> chunks, BoxSelection boundaries)
        {
            return chunks.Select(chunk => new ChunkPartition(chunk, boundaries));
        }

        public override string ToString()
        {
            return String.Format("{0}, Dimension: {1}, World: {2}", Selection, Dimension, World);
        }

        // TODO: edit chunks inside selection
        // TODO: GetChunks method that can access infinite chunks at once, enumerates them and assumes that they wont be accessed after being enumerated over
    }
}