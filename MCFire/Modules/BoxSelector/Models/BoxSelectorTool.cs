using System.ComponentModel.Composition;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.BoxSelector.Models
{
    [Export(typeof(IEditorTool))]
    [Export(typeof(BoxSelectorTool))]
    public class BoxSelectorTool:EditorToolBase
    {
        public override string ToolName { get { return "Box Selector"; } }
        public override string ToolCategory { get { return "Selection"; } }
    }
}
