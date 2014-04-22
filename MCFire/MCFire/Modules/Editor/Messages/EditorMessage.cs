using System;
using JetBrains.Annotations;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Editor.Messages
{
    public abstract class EditorMessage
    {
        public readonly EditorViewModel EditorViewModel;

        protected EditorMessage([NotNull] EditorViewModel editorViewModel)
        {
            if (editorViewModel == null) throw new ArgumentNullException("editorViewModel");
            EditorViewModel = editorViewModel;
        }
    }
}
