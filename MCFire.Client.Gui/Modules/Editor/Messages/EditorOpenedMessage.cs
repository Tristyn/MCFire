using JetBrains.Annotations;
using MCFire.Graphics.Modules.Editor.Models;

namespace MCFire.Graphics.Modules.Editor.Messages
{
    public class EditorOpenedMessage : EditorMessage
    {
        public EditorOpenedMessage([NotNull] EditorGame editorViewModel) : base(editorViewModel)
        {
        }
    }
}
