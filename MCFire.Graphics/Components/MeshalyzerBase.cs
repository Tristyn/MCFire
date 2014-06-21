using MCFire.Common;
using MCFire.Graphics.Editor;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Components
{
    public abstract class MeshalyzerBase : IMeshalyzer
    {
        public virtual void Dispose() { }
        public virtual void LoadContent(IEditorGame game) { }
        public virtual void UnloadContent(IEditorGame game) { }
        public abstract Buffer<VertexPositionColorTexture> Meshalyze(IEditorGame game, BlockSelection volume);
        //public abstract IChunkMesh MeshalyzeWithBorder(IEditorGame game, BlockSelection volume, int borderBlock, Vector3 origin = new Vector3());
    }
}