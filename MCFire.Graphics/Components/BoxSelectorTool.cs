using System.ComponentModel.Composition;

namespace MCFire.Graphics.Components
{
    [Export(typeof(IEditorTool))]
    [Export(typeof(BoxSelectorTool))]
    public class BoxSelectorTool:EditorToolBase
    {
        public override string ToolName { get { return "Box Selector"; } }
        public override string ToolCategory { get { return "Selection"; } }
    }
}
