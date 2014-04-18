using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.ViewModels;
using MCFire.Modules.Test3D.Models;
using MCFire.Modules.Test3D.Views;
using MCFire.Modules.WorldExplorer.Services;

namespace MCFire.Modules.Test3D.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class D3DViewModel : SharpDxViewModelBase
    {
        [Import]
        WorldExplorerService _explorer;

        [CanBeNull]
        D3DTestGame _game;

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            var d3DView = view as D3DView;
            if (d3DView == null)
                return;

            _game = new D3DTestGame(d3DView.SharpDx);
            _game.Disposing += (s, e) => _game = null;
            if (_game != null) RunGame(_game, d3DView.SharpDx);

            await Task.Delay(1000);
            var chunkManager = _explorer.Installations.First().Worlds.First().GetChunkManager();
            for (int i = 0; i < 3; i++)
            {
                var visual = new ChunkVisual(_game.GraphicsDevice, chunkManager, 0, i);
                _game.AddChunkVisual(visual);
            }
        }
    }
}
