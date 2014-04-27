using System.ComponentModel.Composition;
using JetBrains.Annotations;
using MCFire.Modules.Metro.Commands;
using MCFire.Modules.Metro.ViewModels;
using MCFire.Modules.Startup.ViewModels;

namespace MCFire.Modules.Explorer.ViewModels
{
    [Export(typeof(IWindowCommand))]
    public class EditInstallationsViewModel : IWindowCommand
    {
        [Import] IOverlayHost _overlayHost;

        [UsedImplicitly]
        public void Edit()
        {
            _overlayHost.TrySetOverlay<ConfigureInstallationsViewModel>();
        }
    }
}
