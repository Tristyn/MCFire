using MCFire.Modules.Editor.Models.MCFire.Modules.Infrastructure.Interfaces;

namespace MCFire.Modules.Editor.Models
{
    // TODO: with entity in place, could ILoadContent be phased out?
    public interface ILoadContent : ICleanup
    {
        void LoadContent(EditorGame game);
        void UnloadContent(EditorGame game);
    }
}
