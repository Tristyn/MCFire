using MCFire.Modules.Infrastructure.Models;

namespace MCFire.Modules.Explorer.Messages
{
    public class ChunkMessage
    {
        public ChunkPositionDimensionWorld Position { get; private set; }

        public ChunkMessage(ChunkPositionDimensionWorld position)
        {
            Position = position;
        }
    }
}