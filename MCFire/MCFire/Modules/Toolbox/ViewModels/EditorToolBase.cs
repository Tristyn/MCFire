using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public abstract class EditorToolBase : IEditorTool
    {
        public EditorViewModel EditorViewModel;

        public virtual void Initialize(EditorViewModel viewModel)
        {
            EditorViewModel = viewModel;
        }

        public virtual void Selected()
        {
        }

        public virtual void Unselected()
        {
        }
    }
}
