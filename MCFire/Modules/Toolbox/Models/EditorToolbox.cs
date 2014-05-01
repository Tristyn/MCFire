using System.Collections.Generic;
using System.ComponentModel.Composition;
using MCFire.Modules.Editor.Models.MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.Editor.ViewModels;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.Toolbox.Models
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditorToolbox : ICleanup
    {
        [ImportMany(typeof(IEditorTool))]
        IEnumerable<IEditorTool> _tools;
        public EditorViewModel Editor;

        public void Initialize(EditorViewModel editor)
        {
            Editor = editor;
            foreach (var tool in _tools)
            {
                tool.Initialize(editor);
            }
        }

        public void Dispose()
        {
            foreach (var tool in _tools)
            {
                tool.Dispose();
            }
        }
    }
}
