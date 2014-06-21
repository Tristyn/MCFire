using JetBrains.Annotations;

namespace MCFire.Graphics.Components
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
