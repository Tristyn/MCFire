using JetBrains.Annotations;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorOpenedMessage : EditorMessage
    {
        public EditorOpenedMessage([NotNull] EditorViewModel editorViewModel) : base(editorViewModel)
        {
        }
    }
}
