using System;
using JetBrains.Annotations;

namespace MCFire.Modules.Toolbox.ViewModels
{
    public class ToolModel
    {
        public ToolModel([NotNull] IEditorTool tool)
        {
            if (tool == null) throw new ArgumentNullException("tool");
            Tool = tool;
        }

        [NotNull]
        public IEditorTool Tool { get;private set; }
    }
}
