using MCFire.Modules.Editor.Models.MCFire.Modules.Infrastructure.Interfaces;

namespace MCFire.Modules.Editor.Models
{
    public interface ILoadContent : ICleanup
    {
        void LoadContent(EditorGame game);
        void UnloadContent(EditorGame game);
    }
}
