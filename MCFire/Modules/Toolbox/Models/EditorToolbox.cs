using System.Collections.Generic;
using System.ComponentModel.Composition;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Editor.Models.MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.Toolbox.Models
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditorToolbox : ICleanup
    {
        [ImportMany]
        public IEnumerable<IEditorTool> Tools { get; private set; }
        public EditorGame Editor { get; private set; }

        public void Initialize(EditorGame editor)
        {
            Editor = editor;
            foreach (var tool in Tools)
            {
                tool.Initialize(editor);
            }
        }

        public void Dispose()
        {
            foreach (var tool in Tools)
            {
                tool.Dispose();
            }
        }
    }
}
