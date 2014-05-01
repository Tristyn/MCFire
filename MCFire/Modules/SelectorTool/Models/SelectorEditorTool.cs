using System.ComponentModel.Composition;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.SelectorTool.Models
{
    [Export(typeof(IEditorTool))]
    public class SelectorEditorTool : EditorToolBase
    {
    }
}
