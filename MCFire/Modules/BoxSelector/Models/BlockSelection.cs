using System;
using System.Collections.Generic;
using System.Linq;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using Substrate.Core;

namespace MCFire.Modules.BoxSelector.Models
{
    /// <summary>
    /// A generic view of a selection of partitioned chunks.
    /// </summary>
    public interface IBlockSelection
    {
        void GetChunks(AccessMode mode, PartionedChunksFunc chunksAction);
        BoxSelection Selection { get; }
        int Dimension { get; }
        MCFireWorld World { get; }
    }

    /// <summary>
    /// Specifies a box-shaped selection in a minecraft world.
    /// A selection is immutable and can be used to enumerate through the collective data of multiple chunks.
    /// </summary>
    public class BlockSelection : IBlockSelection
    {
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
            var cuboid = boundaries.GetCuboid();
            return chunks.Select(chunk => new ChunkPartition(chunk, cuboid));
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
        public ChunkPartition(IChunk chunk, Cuboid boundaries)
        {
            Chunk = chunk;

            var size = chunk.Size();
            // if minimum or maximum are out of bounds, cap them
            XMin = Math.Min(boundaries.Left, 0);
            YMin = Math.Min(boundaries.Bottom, 0);
            ZMin = Math.Min(boundaries.Width, 0);

            XMax = Math.Max(boundaries.Left + boundaries.Length, size.X - 1);
            YMax = Math.Max(boundaries.Bottom + boundaries.Height, size.Y - 1);
            ZMax = Math.Max(boundaries.Forward + boundaries.Width, size.Z - 1);
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