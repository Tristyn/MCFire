using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using MCFire.Common.Infrastructure.Extensions;
using MCFire.Common.MCFire.Modules.Infrastructure.Models;
using Substrate;

namespace MCFire.Common.Processing
{
    /// <summary>
    /// Raytraces through chunks and returns block informaton that the ray intersects.
    /// </summary>
    public class ChunkTracer : IEnumerable<IChunkTraceData>
    {
        [NotNull]
        readonly VoxelTracer _tracer;

        readonly int _dimension;

        [NotNull]
        readonly World _world;

        public ChunkTracer([NotNull] Vector3 position, [NotNull] Vector3 direction, int dimension, [NotNull] World world)
            : this(new VoxelTracer(position, direction), dimension, world)
        {
        }

        public ChunkTracer([NotNull] VoxelTracer tracer, int dimension, [NotNull] World world)
        {
            if (tracer == null) throw new ArgumentNullException("tracer");
            if (world == null) throw new ArgumentNullException("world");

            _tracer = tracer;
            _dimension = dimension;
            _world = world;
        }
        // TODO: Return bare minimum required to do collision, user gets the rest.
        public IEnumerator<IChunkTraceData> GetEnumerator()
        {
            // fields
            var rayPos = _tracer.Position;
            var currentChunkPos = (ChunkPosition)new BlockPosition((int)rayPos.X, (int)rayPos.Y, (int)rayPos.Z);
            var enumerator = _tracer.GetEnumerator();
            enumerator.MoveNext();

            // TODO: IChunkTraceData is a problem when traversing ungenerated chunks. (wont know size of a chunk or when it enters a new one.)
            var size = new ChunkSize(16,256,16);
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

                yield return new ChunkTraceData(currentChunkPos, size, chunkNull ? null : positions, chunkNull ? null : alphaBlocks);

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

        /// <summary>
        /// Returns the block that the users mouse is hovering over.
        /// The rules are that 
        /// </summary>
        /// <param name="collidingBlock">The position of the block that collided with the ChunkTracer.</param>
        /// <param name="behaviour">The behaviour that defines the rules of hit testing.</param>
        /// <returns>If the raytrace returned any blocks.</returns>
        public static bool TryHitTestBlock(CollisionBehaviour behaviour, [NotNull] ChunkTracer tracer, out BlockPosition collidingBlock)
        {
            switch (behaviour)
            {
                case CollisionBehaviour.Player:
                    return TryHitTestPlayer(tracer, out collidingBlock);
                case CollisionBehaviour.Editor:
                    return TryHitTestEditor(tracer, out collidingBlock);
                default:
                    throw new ArgumentOutOfRangeException("behaviour");
            }
        }

        static bool TryHitTestPlayer([NotNull] ChunkTracer tracer, out BlockPosition position)
        {
            foreach (var data in tracer)
            {
                if (data.Blocks == null || data.Positions == null)
                    continue;

                for (var i = 0; i < data.Blocks.Count; i++)
                {
                    var block = data.Blocks[i];
                    if (block.ID == 0 || block.Info.State == BlockState.FLUID)
                        continue;

                    position = new BlockPosition(data.ChunkPosition, data.Positions[i], data.Size);
                    return true;
                }
            }
            position = default(BlockPosition);
            return false;
        }

        static bool TryHitTestEditor([NotNull] ChunkTracer tracer, out BlockPosition position)
        {
            bool exitedSolid = false;
            bool exitedAir = false;
            bool includeLiquids = false;
            bool firstEnumeration = true;

            foreach (var traceData in tracer)
            {
                // determine if we include liquids by checking if the block we start at is also a liquid
                if (firstEnumeration)
                {
                    if (traceData.Blocks == null)
                    {
                        // trace started in a non-generated chunk, which visually is air. IncludeLiquids = false
                        firstEnumeration = false;
                    }
                    else
                    {
                        var firstBlock = traceData.Blocks.FirstOrDefault();
                        includeLiquids = firstBlock == null || firstBlock.Info.State != BlockState.FLUID;
                        firstEnumeration = false;
                    }
                }

                if (traceData.Positions == null || traceData.Blocks == null) continue;
                for (var i = 0; i < traceData.Blocks.Count; i++)
                {
                    var block = traceData.Blocks[i];
                    // enumerate until it isn't a solid
                    if (!exitedSolid)
                    {
                        if (block.Info.State != BlockState.SOLID)
                            exitedSolid = true;
                        else continue;
                    }

                    // enumerate until it isn't air
                    if (!exitedAir)
                        if (block.ID != 0)
                            exitedAir = true;
                        else continue;

                    if (includeLiquids)
                    {
                        // enumerate to any block that isn't air
                        if (block.ID == 0) continue;
                        position = new BlockPosition(traceData.ChunkPosition, traceData.Positions[i], traceData.Size);
                        return true;
                    }

                    // enumerate to the first solid or nonsolid
                    if (block.Info.State == BlockState.FLUID || block.ID == 0) continue;

                    position = new BlockPosition(traceData.ChunkPosition, traceData.Positions[i], traceData.Size);
                    return true;
                }
            }
            position = new BlockPosition();
            return false;
        }
    }

    /// <summary>
    /// Used by ChunkTracer.
    /// </summary>
    public enum CollisionBehaviour
    {
        /// <summary>
        /// Recreates the behaviour of Minecraft when it determines the block that the player is looking at.
        /// </summary>
        Player,

        /// <summary>
        /// Used by the MCFire editor.
        /// if we are in a solid, we will jump to the first nonsolid.
        /// At that point, we will select the first non-air block.
        /// An exception is that if we start in a liquid, we also ignore liquids (so you can select blocks while underwater)
        /// </summary>
        Editor
    }
}
