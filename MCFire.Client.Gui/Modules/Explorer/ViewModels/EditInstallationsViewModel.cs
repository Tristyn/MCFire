using JetBrains.Annotations;

namespace MCFire.Client.Gui.Modules.Explorer.ViewModels
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
