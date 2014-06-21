using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using Substrate;

namespace MCFire.Common.Processing
{
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