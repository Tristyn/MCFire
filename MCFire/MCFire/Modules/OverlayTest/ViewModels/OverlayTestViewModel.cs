using System;
using System.ComponentModel.Composition;
using MCFire.Modules.Infrastructure.Interfaces;
using Newtonsoft.Json.Bson;

namespace MCFire.Modules.OverlayTest.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class OverlayTestViewModel : IModalOverlay
    {
        public event EventHandler CloseOverlay;

        public void Close()
        {
            if (CloseOverlay != null) CloseOverlay(this, EventArgs.Empty);
        }
    }
}
