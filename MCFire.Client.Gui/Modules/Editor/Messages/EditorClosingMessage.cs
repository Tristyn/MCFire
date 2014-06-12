using JetBrains.Annotations;
using MCFire.Graphics.Editor;

namespace MCFire.Client.Gui.Modules.Editor.Messages
{
    public class EditorClosingMessage : EditorMessage
    {
        public EditorClosingMessage([NotNull] IEditorGameFacade editorGame) : base(editorGame)
        {
        }
    }
}
