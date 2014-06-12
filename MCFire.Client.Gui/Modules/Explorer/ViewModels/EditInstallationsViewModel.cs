using System.ComponentModel.Composition;
using JetBrains.Annotations;
using MCFire.Client.Gui.Modules.Startup.ViewModels;
using MCFire.Client.Modules;
using MCFire.Client.Services;

namespace MCFire.Client.Gui.Modules.Explorer.ViewModels
{
    [Export(typeof(IWindowCommand))]
    public class EditInstallationsViewModel : IWindowCommand
    {
        [Import] IOverlayService _overlayService;

        [UsedImplicitly]
        public void Edit()
        {
            _overlayService.TrySetOverlay<ConfigureInstallationsViewModel>();
        }
    }
}
