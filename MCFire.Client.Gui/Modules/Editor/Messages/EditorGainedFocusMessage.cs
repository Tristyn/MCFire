using JetBrains.Annotations;
using MCFire.Graphics.Editor;

namespace MCFire.Client.Gui.Modules.Editor.Messages
{
    public class EditorGainedFocusMessage : EditorMessage
    {
        public EditorGainedFocusMessage([NotNull] IEditorGameFacade editorGame) : base(editorGame)
        {
        }
    }
}
