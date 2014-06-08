using JetBrains.Annotations;
using MCFire.Graphics.Modules.Editor.Models;

namespace MCFire.Graphics.Modules.Editor.Messages
{
    public class EditorGainedFocusMessage : EditorMessage
    {
        public EditorGainedFocusMessage([NotNull] EditorGame editorGame) : base(editorGame)
        {
        }
    }
}
