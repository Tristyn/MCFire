using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Startup.Models;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.Toolbox.Services
{
    /// <summary>
    /// Manages the lifetime of each tool instances for each editor.
    /// </summary>
    [Export]
    [Export(typeof(ICreateAtStartup))]
    internal class ToolboxService : ICreateAtStartup
    {
        [CanBeNull] IEditorTool _activeTool;
        [NotNull]
        IEnumerable<IEditorTool> _tools;

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
        public IEnumerable<IEditorTool> Tools
        {
            get { return _tools; }
            [UsedImplicitly]
            private set
            {
                _tools = value;
                var first = value.FirstOrDefault();
                if(first!=null)
                    SetCurrentTool(first);
            }
        }
    }
}
