using MCFire.Modules.Editor.Models;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public abstract class EditorToolBase : IEditorTool
    {
        EditorGame _editor;

        /// <inheritDoc/>
        public virtual void Initialize(EditorGame editor)
        {
            _editor = editor;
        }

        /// <inheritDoc/>
        public virtual void Selected()
        {
        }

        /// <inheritDoc/>
        public virtual void Unselected()
        {
        }

        /// <inheritDoc/>
        public virtual void Dispose()
        {
        }
    }
}
