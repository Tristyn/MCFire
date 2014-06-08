using JetBrains.Annotations;
using MCFire.Graphics.Modules.Editor.Models;

namespace MCFire.Graphics.Modules.Editor.Messages
{
    public class EditorClosingMessage : EditorMessage
    {
        public EditorClosingMessage([NotNull] EditorGame editorGame) : base(editorGame)
        {
        }
    }
}
