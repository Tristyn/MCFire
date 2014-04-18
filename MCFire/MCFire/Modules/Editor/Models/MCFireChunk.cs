using JetBrains.Annotations;
using Substrate;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// A representation of a Minecraft chunk for use by the editor.
    /// </summary>
    public class MCFireChunk
    {
        public MCFireChunk(ChunkRef chunk)
        {
            SubstrateChunk = chunk;
        }

        [NotNull]
        public ChunkRef SubstrateChunk { get; set; }

        [CanBeNull]
        public VisualChunk Visual { get; set; }
    }
}
