using JetBrains.Annotations;
using MCFire.Graphics.Editor;

namespace MCFire.Client.Gui.Modules.Editor.Messages
{
    public class EditorOpenedMessage : EditorMessage
    {
        public EditorOpenedMessage([NotNull] IEditorGameFacade editorViewModel) : base(editorViewModel)
        {
        }
    }
}
