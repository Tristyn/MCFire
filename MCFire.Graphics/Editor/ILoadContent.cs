using MCFire.Common;

namespace MCFire.Graphics.Editor
{
    // TODO: with entity in place, could ILoadContent be phased out?
    public interface ILoadContent : ICleanup
    {
        void LoadContent(IEditorGame game);
        void UnloadContent(IEditorGame game);
    }
}
