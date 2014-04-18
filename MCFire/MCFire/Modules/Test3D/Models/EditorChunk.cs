using Substrate;
using Substrate.Core;

namespace MCFire.Modules.Test3D.Models
{
    /// <summary>
    /// A representation of a Minecraft chunk for use by the editor.
    /// </summary>
    public class EditorChunk
    {
        public EditorChunk(IChunkManager manager, int cx, int cy)
        {
            SubstrateChunk = manager.GetChunkRef(cx, cy);
            
        }

        public ChunkRef SubstrateChunk { get; set; }
        public ChunkVisual Visual { get; private set; }
    }
}
