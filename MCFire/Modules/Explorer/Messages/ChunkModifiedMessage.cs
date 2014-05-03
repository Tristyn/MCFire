using MCFire.Modules.Infrastructure.Models;

namespace MCFire.Modules.Explorer.Messages
{
    public class ChunkModifiedMessage : ChunkMessage
    {
        public ChunkModifiedMessage(ChunkPositionDimensionWorld position) : base(position) { }
    }
}
