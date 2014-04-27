using System;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public interface IEditorTool : IDisposable
    {
        void Initialize(EditorViewModel editor);
        void Selected();
        void Unselected();
    }
}
