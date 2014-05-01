using System;
using JetBrains.Annotations;
using MCFire.Modules.Toolbox.ViewModels;

namespace MCFire.Modules.Toolbox.Messages
{
    /// <summary>
    /// Specifies that the type of tool has been selected.
    /// </summary>
    public class EditorToolSelectedMessage
    {
        public EditorToolSelectedMessage([NotNull] Type toolType)
        {
            if (toolType == null) throw new ArgumentNullException("toolType");
            if(!typeof(IEditorTool).IsAssignableFrom(toolType))
                throw new ArgumentException("toolType must be an IEditorTool");

            ToolType = toolType;
        }

        public Type ToolType { get; private set; }
    }
}
