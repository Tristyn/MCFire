using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Coordinates;
using MCFire.Common.Infrastructure.Extensions;
using MCFire.Common.Infrastructure.Models.MCFire.Modules.Infrastructure.Models;
using MCFire.Graphics.Primitives;
using Substrate.Core;

namespace MCFire.Graphics.Editor.Tools.BoxSelector
{
    /// <summary>
    /// A generic view of a selection of partitioned chunks.
    /// </summary>
    public interface IBlockSelection
    {
        void GetChunks(AccessMode mode, PartionedChunksFunc chunksAction);
        BoxSelection Selection { get; }
        int Dimension { get; }
        World World { get; }
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
        public BlockSelection([NotNull] BoxSelection selection, int dimension, [NotNull] World world)
        {
            Selection = selection;
            Dimension = dimension;
            World = world;
        }

        public BoxSelection Selection { get; private set; }
        public int Dimension { get; private set; }
        public World World { get; private set; }

        //public int Dimension { get { return _dimension; } }

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
            var cuboid = boundaries.GetCuboid();
            return chunks.Select(chunk => new ChunkPartition(chunk, cuboid));
        }

        public override string ToString()
        {
            return String.Format("{0}, Dimension: {1}, World: {2}", Selection, Dimension, World);
        }

        // TODO: edit chunks inside selection
        // TODO: GetChunks method that can access infinite chunks at once, enumerates them and assumes that they wont be accessed after being enumerated over
    }

    public delegate void PartionedChunksFunc(IEnumerable<IChunkPartition> chunkPartitions);


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
        public ChunkPartition([NotNull] IChunk chunk, Cuboid boundaries)
        {
            Chunk = chunk;

            var size = chunk.Size();
            var position = chunk.Position();
            var lesserCorner = new BlockPosition(position, new LocalBlockPosition(0, 0, 0), size);

            // if minimum or maximum are out of bounds, cap them
            XMin = Math.Max(boundaries.Left - lesserCorner.X, 0);
            YMin = Math.Max(boundaries.Bottom - lesserCorner.Y, 0);
            ZMin = Math.Max(boundaries.Forward - lesserCorner.Z, 0);

            XMax = Math.Min(boundaries.Left + boundaries.Length - lesserCorner.X, size.X);
            YMax = Math.Min(boundaries.Bottom + boundaries.Height - lesserCorner.Y, size.Y);
            ZMax = Math.Min(boundaries.Forward + boundaries.Width - lesserCorner.Z, size.Z);
        }

        /// <summary>
        /// A chunk portion with no constraints.
        /// </summary>
        /// <param name="chunk">The chunk to portion.</param>
        public ChunkPartition([NotNull] IChunk chunk)
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
        // TODO: comment, mention max properties are collection lengths (+1)
        public int XMin { get; private set; }
        public int XMax { get; private set; }
        public int YMin { get; private set; }
        public int YMax { get; private set; }
        public int ZMin { get; private set; }
        public int ZMax { get; private set; }
        public IChunk Chunk { get; private set; }
    }
}