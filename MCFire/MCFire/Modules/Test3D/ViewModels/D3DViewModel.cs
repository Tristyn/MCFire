using System.ComponentModel.Composition;
using MCFire.Modules.Infrastructure.ViewModels;
using MCFire.Modules.Test3D.Models;
using MCFire.Modules.Test3D.Views;

namespace MCFire.Modules.Test3D.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class D3DViewModel : SharpDxViewModelBase
    {
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            var d3DView = view as D3DView;
            if (d3DView == null)
                return;
            RunGame(new D3DTestGame(), d3DView.SharpDx);
        }
    }
}
