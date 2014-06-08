using JetBrains.Annotations;

namespace MCFire.Client.Gui.Modules.Toolbox.ViewModels
{
    public interface IEditorTool
    {
        [NotNull]
        string ToolName { get; }
        [NotNull]
        string ToolCategory { get; }
        bool Selected { get; set; }
    }
}
