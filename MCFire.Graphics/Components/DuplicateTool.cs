using System.ComponentModel.Composition;

namespace MCFire.Graphics.Components
{
    [Export, Export(typeof(IEditorTool))]
    class DuplicateTool : EditorToolBase
    {
        public override string ToolName
        {
            get { return "Duplicate"; }
        }

        public override string ToolCategory
        {
            get { return "Transform"; }
        }
    }
}
