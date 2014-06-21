using System.ComponentModel.Composition;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Client.Services.Clipboard;
using MCFire.Common.Components;
using MCFire.Graphics.Messages;

namespace MCFire.Client.Components
{
    [Export(typeof(ICreateAtStartup))]
    class BoxSelectorGlueComponent : ICreateAtStartup, IHandle<BoxSelectionCopiedMessage>
    {
        IEventAggregator _aggregator;

        [NotNull,Import]
        IEventAggregator Aggregator
        {
            set
            {
                value.Subscribe(this);
                _aggregator = value;
            }
        }

        public void Handle([NotNull] BoxSelectionCopiedMessage m)
        {
            _aggregator.Publish(new ClipboardCopyMessage(m.Selection));
        }
    }
}
