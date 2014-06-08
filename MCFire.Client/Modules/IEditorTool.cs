using JetBrains.Annotations;

namespace MCFire.Client.Modules
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
