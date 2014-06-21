using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Graphics.Editor;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Components
{
    public interface IMeshalyzer : ILoadContent
    {
        /// <summary>
        /// Meshalyzes a section of a world.
        /// </summary>
        /// <returns>A drawable part of the world. Null denotes that no more work is to be done.</returns>
        [CanBeNull]
        Buffer<VertexPositionColorTexture> Meshalyze([NotNull] IEditorGame game, [NotNull] BlockSelection volume);
        // TODO:
        //[NotNull]
        //IChunkMesh MeshalyzeWithBorder([NotNull] IEditorGame game, [NotNull] BlockSelection volume, int borderBlock,
        //    Vector3 origin = default(Vector3));
    }
}
