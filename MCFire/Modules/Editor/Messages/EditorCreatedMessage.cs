using JetBrains.Annotations;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorCreatedMessage : EditorMessage
    {
        public EditorCreatedMessage([NotNull] EditorViewModel editorViewModel) : base(editorViewModel)
        {
        }
    }
}
