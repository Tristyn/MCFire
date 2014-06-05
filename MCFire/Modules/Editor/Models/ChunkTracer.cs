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
            var currentChunkPos = (ChunkPosition)(BlockPosition)_tracer.Ray.Position;
            var enumerator = _tracer.GetEnumerator();
            enumerator.MoveNext();

            var size = _world.ChunkSize;
            var positions = new List<LocalBlockPosition>(20);
            var alphaBlocks = new List<AlphaBlock>(20);
            var chunkNull = false;

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
                                // get block data
                                var blocks = chunk.Blocks;
                                var localPos = new LocalBlockPosition(chunk, enumerator.Current);
                                positions.Add(localPos);
                                // if it is out of range, it's air
                                alphaBlocks.Add((localPos.Y >= 256 || localPos.Y < 0) ? new AlphaBlock(0) : blocks.GetBlock(localPos));
                            }
                            else chunkNull = true;

                            if (enumerator.MoveNext()) continue;

                            // enumerator cant MoveNext, break and return the last of our data
                            tracerReachedLimit = true;
                            break;
                        }
                    });

                // yield return our current ChunkTraceData

                yield return new ChunkTraceData(currentChunkPos, size??new ChunkSize(16,256,16), chunkNull ? null : positions, chunkNull ? null : alphaBlocks);

                // create new collections for reading the next chunk data
                positions = new List<LocalBlockPosition>(20);
                alphaBlocks = new List<AlphaBlock>(20);
                chunkNull = false;

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
        /// The size of the chunk.
        /// </summary>
        ChunkSize Size { get; set; }

        /// <summary>
        /// The positions of each intersecting block in chunk-space
        /// </summary>
        [CanBeNull]
        List<LocalBlockPosition> Positions { get; }
        /// <summary>
        /// The positions of each intersecting block
        /// </summary>
        [CanBeNull]
        List<AlphaBlock> Blocks { get; }
    }

    internal class ChunkTraceData : IChunkTraceData
    {
        public ChunkTraceData(ChunkPosition chunkPosition, ChunkSize size, [CanBeNull] List<LocalBlockPosition> positions = null,
             [CanBeNull] List<AlphaBlock> blocks = null)
        {
            ChunkPosition = chunkPosition;
            Size = size;
            Positions = positions;
            Blocks = blocks;
        }

        /// <inheritDoc/>
        public ChunkPosition ChunkPosition { get; private set; }

        public ChunkSize Size { get; set; }

        /// <inheritDoc/>
        public List<LocalBlockPosition> Positions { get; private set; }

        /// <inheritDoc/>
        public List<AlphaBlock> Blocks { get; private set; }
    }
}
