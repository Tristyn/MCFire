using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Client.Modules;
using MCFire.Client.Modules.Toolbox.Services;
using MCFire.Core.Modules.Startup.Models;

namespace MCFire.Client.Services
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
