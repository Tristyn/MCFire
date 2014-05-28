using JetBrains.Annotations;
using MCFire.Modules.Toolbox.Models;

namespace MCFire.Modules.Toolbox.Messages
{
    public class CurrentToolboxChangedMessage
    {
        [CanBeNull]
        public EditorToolbox Toolbox { get; private set; }

        public CurrentToolboxChangedMessage( [CanBeNull] EditorToolbox toolbox)
        {
            Toolbox = toolbox;
        }
    }
}
