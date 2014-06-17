using SharpDX;

namespace MCFire.Graphics.Editor.Modules.Meshalyzer
{
    public interface IChunkMesh : IDrawable
    {
        Vector3 ModelOrigin { get; }
    }
}
