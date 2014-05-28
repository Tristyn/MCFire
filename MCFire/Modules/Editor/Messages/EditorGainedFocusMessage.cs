using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorGainedFocusMessage : EditorMessage
    {
        public EditorGainedFocusMessage([NotNull] EditorGame editorGame) : base(editorGame)
        {
        }
    }
}
