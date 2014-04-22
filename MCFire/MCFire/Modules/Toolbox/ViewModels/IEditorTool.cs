using System;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public interface IEditorTool : IDisposable
    {
        void Initialize(EditorViewModel viewModel);
        void Selected();
        void Unselected();
    }
}
