using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorClosingMessage : EditorMessage
    {
        public EditorClosingMessage([NotNull] EditorGame editorGame) : base(editorGame)
        {
        }
    }
}
