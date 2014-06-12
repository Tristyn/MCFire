using JetBrains.Annotations;
using MCFire.Client.Modules;

namespace MCFire.Client.Services
{
    public interface IToolboxService
    {
        void SetCurrentTool([NotNull] IEditorTool tool);
    }
}
