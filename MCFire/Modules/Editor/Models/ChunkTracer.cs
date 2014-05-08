using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using Substrate;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Raytraces through chunks and returns block informaton that the ray intersects.
    /// </summary>
    internal class ChunkTracer : IEnumerable<IChunkTraceData>
    {
        [NotNull]
        readonly VoxelTracer _tracer;

        readonly int _dimension;

        [NotNull]
        readonly MCFireWorld _world;

        public ChunkTracer([NotNull] VoxelTracer tracer, int dimension, [NotNull] MCFireWorld world)
        {
            if (tracer == null) throw new ArgumentNullException("tracer");
            if (world == null) throw new ArgumentNullException("world");

            _tracer = tracer;
            _dimension = dimension;
            _world = world;
        }

        public IEnumerator<IChunkTraceData> GetEnumerator()
        {

            // fields
            var currentChunkPos = (ChunkPosition)(BlockPosition)_tracer.Origin;
            var enumerator = _tracer.GetEnumerator();
            enumerator.MoveNext();

            var positions = new List<BlockPosition>(20);
            var alphaBlocks = new List<AlphaBlock>(20);

            // this was supposed to be simple and beautiful, but the fact that you cant yield in a lambda made it pretty nasty.
            var tracerReachedLimit = false;
            while (!tracerReachedLimit)
            {
                _world.GetChunk(new ChunkPositionDimension(currentChunkPos, _dimension), AccessMode.Read, chunk =>
                    {
                        while (true)
                        {
                            // check to see if we've moved on to the next chunk. if so, return without reading its data.
                            var exactPos = (ChunkPosition)enumerator.Current;
                            if (currentChunkPos != exactPos)
                            {
                                // yield return our current ChunkTraceData, loads the next chunk
                                break;
                            }

                            // gather data
                            if (chunk != null)
                            {
                                var blocks = chunk.Blocks;
                                var localPos = ((BlockPosition)enumerator.Current).GetLocalPosition(blocks.XDim,
                                    blocks.YDim, blocks.ZDim);
                                positions.Add(localPos);
                                alphaBlocks.Add(blocks.GetBlock(localPos));
                            }
                            else
                            {
                                positions.Add(((BlockPosition)enumerator.Current).GetLocalPosition(16,256,16));
                                alphaBlocks.Add(new AlphaBlock(0));
                            }

                            if (enumerator.MoveNext()) continue;

                            // enumerator cant MoveNext, break and return the last of our data
                            tracerReachedLimit = true;
                            break;
                        }
                    });

                // yield return our current ChunkTraceData
                yield return new ChunkTraceData(currentChunkPos, positions, alphaBlocks);

                // create new collections for reading the next chunk data
                positions = new List<BlockPosition>(20);
                alphaBlocks = new List<AlphaBlock>(20);

                // move currentChunkPos to the next chunk
                currentChunkPos = enumerator.Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface IChunkTraceData
    {
        /// <summary>
        /// The position of the chunk.
        /// </summary>
        ChunkPosition ChunkPosition { get; }

        /// <summary>
        /// The positions of each intersecting block in chunk-space
        /// </summary>
        [NotNull]
        List<BlockPosition> Positions { get; }
        /// <summary>
        /// The positions of each intersecting block
        /// </summary>
        List<AlphaBlock> Blocks { get; set; }
    }

    internal class ChunkTraceData : IChunkTraceData
    {
        public ChunkTraceData(ChunkPosition chunkPosition, List<BlockPosition> positions, List<AlphaBlock> blocks)
        {
            ChunkPosition = chunkPosition;
            Positions = positions;
            Blocks = blocks;
        }

        /// <inheritDoc/>
        public ChunkPosition ChunkPosition { get; private set; }

        /// <inheritDoc/>
        public List<BlockPosition> Positions { get; private set; }

        /// <inheritDoc/>
        public List<AlphaBlock> Blocks { get; set; }
    }
}
