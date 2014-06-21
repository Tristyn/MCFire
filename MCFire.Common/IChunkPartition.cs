using Substrate.Core;

namespace MCFire.Common
{
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
}