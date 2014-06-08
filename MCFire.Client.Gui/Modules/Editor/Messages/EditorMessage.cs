using System;
using JetBrains.Annotations;
using MCFire.Graphics.Modules.Editor.Models;

namespace MCFire.Graphics.Modules.Editor.Messages
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
