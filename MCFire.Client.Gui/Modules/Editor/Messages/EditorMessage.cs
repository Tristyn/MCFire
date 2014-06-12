using System;
using JetBrains.Annotations;
using MCFire.Graphics.Editor;

namespace MCFire.Client.Gui.Modules.Editor.Messages
{
    public abstract class EditorMessage
    {
        [NotNull]
        public readonly IEditorGameFacade EditorGame;

        protected EditorMessage([NotNull] IEditorGameFacade editorGame)
        {
            if (editorGame == null) throw new ArgumentNullException("editorGame");
            EditorGame = editorGame;
        }
    }
}
