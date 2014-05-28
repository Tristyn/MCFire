using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using JetBrains.Annotations;
using MCFire.Modules.Toolbox.Messages;
using MCFire.Modules.Toolbox.Models;
using MCFire.Modules.Toolbox.Services;

namespace MCFire.Modules.Toolbox.ViewModels
{
    [Export]
    public class ToolboxViewModel : Tool, IHandle<CurrentToolboxChangedMessage>
    {
        IEnumerable<IEditorTool> _tools;

        public ToolboxViewModel()
        {
            DisplayName = "Tools";
        }

        void IHandle<CurrentToolboxChangedMessage>.Handle(CurrentToolboxChangedMessage message)
        {
            SetTools(message.Toolbox);
        }

        void SetTools([CanBeNull] EditorToolbox toolbox)
        {
            Tools = toolbox != null ? toolbox.Tools : Enumerable.Empty<IEditorTool>();
        }

        [Import]
        ToolboxService Toolbox
        {
            // set it to whatever value is present, listen for CurrentToolboxChangedMessages to update
            set { SetTools(value.CurrentToolbox); }
        }

        [Import]
        public IEventAggregator Aggregator
        {
            set { value.Subscribe(this); }
        }

        public IEnumerable<IEditorTool> Tools
        {
            get { return _tools; }
            private set
            {
                if (Equals(value, _tools)) return;
                _tools = value;
                NotifyOfPropertyChange(() => Tools);
                NotifyOfPropertyChange(() => NoTools);
            }
        }

        public bool NoTools { get { return Tools == null || !Tools.Any(); } }

        public override double PreferredWidth
        {
            get { return 50; }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }
    }
}
