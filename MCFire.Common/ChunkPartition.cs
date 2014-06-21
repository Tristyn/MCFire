using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using MCFire.Common.Infrastructure.Extensions;
using Substrate.Core;

namespace MCFire.Common
{
    internal class ChunkPartition : IChunkPartition
    {
        public ChunkPartition([NotNull] IChunk chunk, BoxSelection boundaries)
        {
            Chunk = chunk;

            var size = chunk.Size();
            var position = chunk.Position();
            var lesserCorner = new BlockPosition(position, new LocalBlockPosition(0, 0, 0), size);
            var greaterCorner = new BlockPosition(position, new LocalBlockPosition(size.X, size.Y, size.Z), size);

            // translate boundaries into chunk-local
            // if minimum or maximum are out of bounds, cap them
            XMin = (boundaries.Left - lesserCorner.X).Clamp(0, size.X);
            YMin = (boundaries.Bottom - lesserCorner.Y).Clamp(0,size.Y);
            ZMin = (boundaries.Forward - lesserCorner.Z).Clamp(0,size.Z);

            XMax = (boundaries.Right - lesserCorner.X + 1).Clamp(0, size.X);
            YMax = (boundaries.Top - lesserCorner.Y + 1).Clamp(0,size.Y);
            ZMax = (boundaries.Backward - lesserCorner.Z + 1).Clamp(0,size.Z);
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