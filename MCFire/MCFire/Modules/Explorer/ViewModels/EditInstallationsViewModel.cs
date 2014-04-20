using System.ComponentModel.Composition;
using JetBrains.Annotations;
using MCFire.Modules.Metro.ViewModels;
using MCFire.Modules.Startup.ViewModels;

namespace MCFire.Modules.Explorer.ViewModels
{
    [Export(typeof(IWorldExplorerCommand))]
    public class EditInstallationsViewModel : IWorldExplorerCommand
    {
        [Import] IOverlayHost _overlayHost;

        [UsedImplicitly]
        public void Edit()
        {
            _overlayHost.TrySetOverlay<ConfigureInstallationsViewModel>();
        }
    }
}
