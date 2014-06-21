using JetBrains.Annotations;
using MCFire.Common;
using SharpDX;

namespace MCFire.Graphics.Editor.Modules.Meshalyzer
{
    public interface IChunkMesh : ICleanup
    {
        Vector3 ModelOrigin { get; }
        Matrix World { set; }
        void Draw([NotNull] IEditorGame game);
    }
}
