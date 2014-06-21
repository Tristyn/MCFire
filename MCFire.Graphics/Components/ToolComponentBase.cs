using JetBrains.Annotations;

namespace MCFire.Graphics.Components
{
    public abstract class ToolComponentBase : GameComponentBase
    {
        [NotNull]
        protected abstract IEditorTool Tool { get; set; }

        protected bool Selected { get { return Tool.Selected; }}
    }
}
