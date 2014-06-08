using JetBrains.Annotations;
using MCFire.Graphics.Modules.Editor.Messages;
using MCFire.Graphics.Modules.Editor.Models;

namespace MCFire.Client.Gui.Modules.Editor.Messages
{
    public class EditorClosingMessage : EditorMessage
    {
        public EditorClosingMessage([NotNull] EditorGame editorGame) : base(editorGame)
        {
        }
    }
}
