using System.ComponentModel.Composition;
using MCFire.Modules.Metro.Commands;
using MCFire.Modules.Metro.ViewModels;

namespace MCFire.Modules.OverlayTest.ViewModels
{
    [Export(typeof(IWindowCommand))]
    public class OpenOverlayViewModel : IWindowCommand
    {
        [Import(typeof(IOverlayHost))]
        IOverlayHost _mainWindow;

        public void OpenOverlay()
        {
            _mainWindow.TrySetOverlay<OverlayTestViewModel>();
        }
    }
}
