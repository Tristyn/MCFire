using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using Substrate;

namespace MCFire.Common.Processing
{
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
}