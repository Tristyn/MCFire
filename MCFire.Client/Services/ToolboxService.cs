using System;
using System.ComponentModel.Composition;
using JetBrains.Annotations;

namespace MCFire.Client.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    [Export(typeof(ICreateAtStartup))]
    internal class ToolboxService : IToolboxService, ICreateAtStartup
    {
        [CanBeNull] IEditorTool _activeTool;

        public void SetCurrentTool([NotNull] IEditorTool tool)
        {
            if (tool == null) throw new ArgumentNullException("tool");

            var current = _activeTool;
            if (current != null)
                current.Selected = false;
            _activeTool = tool;
            tool.Selected = true;
        }

        [ImportMany, NotNull]
        IEnumerable<IEditorTool> Tools
        {
            set
            {
                var first = value.FirstOrDefault();
                if(first!=null)
                    SetCurrentTool(first);
            }
        }
    }
}
