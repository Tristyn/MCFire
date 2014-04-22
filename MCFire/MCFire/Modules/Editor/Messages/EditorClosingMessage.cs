using JetBrains.Annotations;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorClosingMessage : EditorMessage
    {
        public EditorClosingMessage([NotNull] EditorViewModel editorViewModel) : base(editorViewModel)
        {
        }
    }
}
