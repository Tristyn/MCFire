﻿using System;
using JetBrains.Annotations;
using MCFire.Client.Components;
using MCFire.Graphics.Components;

namespace MCFire.Client.Gui.Modules.Toolbox.ViewModels
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
