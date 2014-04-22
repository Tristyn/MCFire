using JetBrains.Annotations;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Editor.Messages
{
    public class EditorGainedFocusMessage : EditorMessage
    {
        public EditorGainedFocusMessage([NotNull] EditorViewModel editorViewModel) : base(editorViewModel)
        {
        }
    }
}
