using System;
using System.ComponentModel.Composition;
using MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.WorldExplorer.Services;

namespace MCFire.Modules.Startup.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConfigureInstallationsViewModel : IModalOverlay
    {
        [Import] 
        WorldExplorerService _explorerService;
        
        public void Close()
        {
            if (CloseOverlay != null) CloseOverlay(this, EventArgs.Empty);
        }

        public void Continue()
        {
            Close(); // TODO:
        }

        public event EventHandler CloseOverlay;
    }
}
