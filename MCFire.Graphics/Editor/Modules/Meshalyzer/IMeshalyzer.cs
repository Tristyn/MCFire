using JetBrains.Annotations;
using MCFire.Graphics.Editor.Tools.BoxSelector;
using SharpDX;

namespace MCFire.Graphics.Editor.Modules.Meshalyzer
{
    public interface IMeshalyzer : ILoadContent
    {
        /// <summary>
        /// Meshalyzes a section of a world.
        /// </summary>
        /// <returns>A drawable part of the world. Null denotes that no more work is to be done.</returns>
        [NotNull]
        IChunkMesh Meshalyze([NotNull] IEditorGame game, [NotNull] BlockSelection volume, Vector3 origin = default(Vector3));
        // TODO:
        //[NotNull]
        //IChunkMesh MeshalyzeWithBorder([NotNull] IEditorGame game, [NotNull] BlockSelection volume, int borderBlock,
        //    Vector3 origin = default(Vector3));
    }
}
