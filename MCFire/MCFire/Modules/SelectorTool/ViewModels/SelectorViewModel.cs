using System.ComponentModel.Composition;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.SelectorTool.ViewModels
{
    [Export(typeof(IEditorTool))]
    public class SelectorViewModel : EditorToolBase
    {
    }
}
