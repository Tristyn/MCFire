using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public abstract class EditorToolBase : IEditorTool
    {
        public EditorViewModel EditorViewModel;

        /// <inheritDoc/>
        public virtual void Initialize(EditorViewModel viewModel)
        {
            EditorViewModel = viewModel;
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
        public void Dispose()
        {
            EditorViewModel = null;
        }
    }
}
