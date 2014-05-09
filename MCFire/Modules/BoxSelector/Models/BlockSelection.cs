using System;
using System.Collections.Generic;
using System.Linq;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using Substrate;
using Substrate.Core;

namespace MCFire.Modules.BoxSelector.Models
{
    /// <summary>
    /// Specifies a box-shaped selection in a minecraft world.
    /// A selection is immutable and can be used to enumerate through the collective data of multiple chunks.
    /// </summary>
    public class BlockSelection
    {
        //readonly int _dimension;
        //readonly MCFireWorld _world;

        ///// <summary>
        ///// Creates a new BoxSelection with the specified dimensions and world.
        ///// </summary>
        ///// <param name="cornerOne">One corner of the box</param>
        ///// <param name="cornerTwo">The other corner of the box</param>
        ///// <param name="dimension">The dimension of the selection</param>
        ///// <param name="world">The world containing the selection</param>
        //public BoxSelection(BlockPosition cornerOne, BlockPosition cornerTwo, int dimension, [NotNull] MCFireWorld world)
        //{
        //    if (world == null) throw new ArgumentNullException("world");

        //    _lesser = new BlockPosition(
        //        Math.Min(cornerOne.X, cornerTwo.X),
        //        Math.Min(cornerOne.Y, cornerTwo.Y),
        //        Math.Min(cornerOne.Z, cornerTwo.Z));
        //    _greater = new BlockPosition(
        //        Math.Max(cornerOne.X, cornerTwo.X),
        //        Math.Max(cornerOne.Y, cornerTwo.Y),
        //        Math.Max(cornerOne.Z, cornerTwo.Z));
        //    _dimension = dimension;
        //    _world = world;
        //}

        /// <summary>
        /// Creates a new BoxSelection with the specified selection, dimensions and world.
        /// </summary>
        public BlockSelection(BoxSelection selection, int dimension, MCFireWorld world)
        {
            Selection = selection;
            Dimension = dimension;
            World = world;
        }

        public BoxSelection Selection { get; private set; }
        public int Dimension { get; private set; }
        public MCFireWorld World { get; private set; }

        //public int Dimension { get { return _dimension; } }

        public void GetChunks(AccessMode mode, PartionedChunksFunc chunksAction)
        {
            var lesserChunk = (ChunkPosition)Selection.Lesser;
            var greaterChunk = (ChunkPosition)Selection.Greater;

            // create an enumerator from every chunk that is within the selection
            var chunkPositions = from x in Enumerable.Range(lesserChunk.ChunkX, greaterChunk.ChunkX)
                                 from z in Enumerable.Range(lesserChunk.ChunkZ, greaterChunk.ChunkZ)
                                 select new ChunkPositionDimension(x, z, Dimension);

            // TODO: cant edit huge amounts of chunks (its a list, therefor all loaded at once, check MCFireWorld for more comments on this)
            World.GetChunks(chunkPositions, mode, chunks => chunksAction(PartitionChunks(chunks, Selection)));
        }

        static IEnumerable<IChunkPartition> PartitionChunks(IEnumerable<IChunk> chunks, BoxSelection boundaries)
        {
            return chunks.Select(chunk => new ChunkPartition(chunk, boundaries));
        }

        // TODO: edit chunks inside selection
        // TODO: GetChunks method that can access infinite chunks at once, enumerates them and assumes that they wont be accessed after being enumerated over
    }

    public delegate void PartionedChunksFunc(IEnumerable<IChunkPartition> chunkPortions);


    /// <summary>
    /// A chunk that may be partitioned. When accessing the Chunk, ignore blocks, entities
    /// and other data that is not contained within the IChunkPartion's boundaries.
    /// </summary>
    public interface IChunkPartition
    {
        int XMin { get; }
        int XMax { get; }
        int YMin { get; }
        int YMax { get; }
        int ZMin { get; }
        int ZMax { get; }
        IChunk Chunk { get; }
    }

    internal class ChunkPartition : IChunkPartition
    {
        public ChunkPartition(IChunk chunk, BoxSelection boundaries)
            : this(chunk, new LocalBlockPosition(chunk, boundaries.Lesser), new LocalBlockPosition(chunk, boundaries.Greater))
        {
        }

        public ChunkPartition(IChunk chunk, BlockPosition minimum, BlockPosition maximum)
            : this(chunk, new LocalBlockPosition(chunk, minimum), new LocalBlockPosition(chunk, maximum))
        {
        }

        public ChunkPartition(IChunk chunk, LocalBlockPosition minimum, LocalBlockPosition maximum)
        {
            Chunk = chunk;

            var size = chunk.Size();
            // if minimum or maximum are out of bounds, cap them
            XMin = Math.Min(minimum.X, 0);
            YMin = Math.Min(minimum.Y, 0);
            ZMin = Math.Min(minimum.Z, 0);

            XMax = Math.Max(maximum.X, size.X - 1);
            YMax = Math.Max(maximum.Y, size.Y - 1);
            ZMax = Math.Max(maximum.Z, size.Z - 1);
        }

        /// <summary>
        /// A chunk portion with no constraints.
        /// </summary>
        /// <param name="chunk">The chunk to portion.</param>
        public ChunkPartition(IChunk chunk)
        {
            Chunk = chunk;
            var size = chunk.Size();
            XMin = 0;
            YMin = 0;
            ZMin = 0;
            XMax = size.X;
            YMax = size.Y;
            ZMax = size.Z;
        }

        public int XMin { get; private set; }
        public int XMax { get; private set; }
        public int YMin { get; private set; }
        public int YMax { get; private set; }
        public int ZMin { get; private set; }
        public int ZMax { get; private set; }
        public IChunk Chunk { get; private set; }
    }
}