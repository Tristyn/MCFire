namespace MCFire.Graphics.Modules.BoxSelector.Models
{
    [Export(typeof(IEditorTool))]
    [Export(typeof(BoxSelectorTool))]
    public class BoxSelectorTool:EditorToolBase
    {
        public override string ToolName { get { return "Box Selector"; } }
        public override string ToolCategory { get { return "Selection"; } }
    }
}
