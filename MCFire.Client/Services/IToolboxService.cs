using JetBrains.Annotations;
using MCFire.Client.Components;
using MCFire.Graphics.Components;

namespace MCFire.Client.Services
{
    public interface IToolboxService
    {
        void SetCurrentTool([NotNull] IEditorTool tool);
    }
}
