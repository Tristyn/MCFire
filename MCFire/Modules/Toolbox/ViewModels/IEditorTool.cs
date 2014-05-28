using System;
using MCFire.Modules.Editor.Models;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public interface IEditorTool : IDisposable
    {
        void Initialize(EditorGame editor);
        void Selected();
        void Unselected();
    }
}
