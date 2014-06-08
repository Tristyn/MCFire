using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Coordinates;

namespace MCFire.Client.Services.Explorer.Messages
{
    public class ChunkModifiedMessage : ChunkMessage
    {
        public ChunkModifiedMessage(ChunkPositionDimension position, [NotNull] World world) : base(position, world)
        {
        }
    }
}
