using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public interface IEditorTool
    {
        void Initialize(EditorViewModel viewModel);
        void Selected();
        void Unselected();
    }
}
