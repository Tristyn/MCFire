using JetBrains.Annotations;

namespace MCFire.Modules.Toolbox.ViewModels
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
