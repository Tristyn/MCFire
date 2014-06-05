using System;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;

namespace MCFire.Modules.Editor.Messages
{
    public abstract class EditorMessage
    {
        [NotNull]
        public readonly EditorGame EditorGame;

        protected EditorMessage([NotNull] EditorGame editorGame)
        {
            if (editorGame == null) throw new ArgumentNullException("editorGame");
            EditorGame = editorGame;
        }
    }
}
