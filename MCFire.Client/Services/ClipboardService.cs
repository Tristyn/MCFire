using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Client.Services.Clipboard;
using MCFire.Core.Modules;

namespace MCFire.Client.Services
{
    [Export(typeof(IClipboardService))]
    [Export(typeof(ICreateAtStartup))]
    class ClipboardService : IClipboardService, IHandle<ClipboardCopyMessage>, ICreateAtStartup
    {
        [Import]
        IEventAggregator Aggregator { set { value.Subscribe(this); } }

        void IHandle<ClipboardCopyMessage>.Handle(ClipboardCopyMessage message)
        {
            Data = message.Data;
        }

        public object Data { get; private set; }
    }
}
