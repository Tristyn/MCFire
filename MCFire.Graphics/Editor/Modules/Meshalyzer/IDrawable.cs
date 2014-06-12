using MCFire.Graphics.Editor.MCFire.Modules.Infrastructure.Interfaces;

namespace MCFire.Graphics.Editor.Modules.Meshalyzer
{
    public interface IDrawable : ICleanup
    {
        void Draw(IEditorGame game);
    }
}
