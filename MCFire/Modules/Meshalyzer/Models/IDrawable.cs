using MCFire.Modules.Editor.Models;
using MCFire.Modules.Editor.Models.MCFire.Modules.Infrastructure.Interfaces;

namespace MCFire.Modules.Meshalyzer.Models
{
    public interface IDrawable : ICleanup
    {
        void Draw(EditorGame game);
    }
}
