using System;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Coordinates;

namespace MCFire.Client.Services.Explorer.Messages
{
    public class ChunkMessage
    {
        public ChunkPositionDimension Position { get; private set; }
        public World World { get; private set; }

        public ChunkMessage(ChunkPositionDimension position, [NotNull] World world)
        {
            if (world == null) throw new ArgumentNullException("world");

            Position = position;
            World = world;
        }
    }
}