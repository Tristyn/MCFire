﻿namespace MCFire.Graphics.Components
{
    public abstract class EditorToolBase : IEditorTool
    {
        /// <inheritDoc/>
        public abstract string ToolName { get; }
        /// <inheritDoc/>
        public abstract string ToolCategory { get; }

        public virtual bool Selected { get; set; }
    }
}
