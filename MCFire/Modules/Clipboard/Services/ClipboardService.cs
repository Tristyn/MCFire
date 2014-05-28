using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Modules.Startup.Models;

namespace MCFire.Modules.Clipboard.Services
{
    [Export(typeof(IClipboardService))]
    [Export(typeof(ICreateAtStartup))]
    class ClipboardService : IClipboardService, IHandle<ClipboardCopyEvent>, ICreateAtStartup
    {
        [Import]
        IEventAggregator Aggregator { set { value.Subscribe(this); } }

        void IHandle<ClipboardCopyEvent>.Handle(ClipboardCopyEvent message)
        {
            Data = message.Data;
        }

        public object Data { get; private set; }
    }
}
