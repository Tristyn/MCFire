using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorOpenedMessage : EditorMessage
    {
        public EditorOpenedMessage([NotNull] EditorGame editorViewModel) : base(editorViewModel)
        {
        }
    }
}
