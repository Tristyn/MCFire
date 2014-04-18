using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Editor.Views;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Explorer.Services;
using MCFire.Modules.Infrastructure.ViewModels;
using Action = System.Action;

namespace MCFire.Modules.Editor.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EditorViewModel : SharpDxViewModelBase
    {
        [CanBeNull]
        EditorGame _game;
        [CanBeNull]
        EditorView _view;
        [CanBeNull]
        Action _viewGained;

        EditorBridge _bridge;

        public EditorViewModel()
        {
            InitializeTo(IoC.Get<WorldExplorerService>().Installations.First().Worlds.First(), 0);
        }

        public void InitializeTo(MCFireWorld world, int dimension)
        {
            if (_view != null)
            {
                InitializeToInternal(world, dimension);
                return;
            }

            // we dont have the view yet. when we do, this will be called
            _viewGained = () => InitializeToInternal(world, dimension);
        }

        private void InitializeToInternal(MCFireWorld world, int dimension)
        {
            if (_game != null)
                _game.Dispose();

            _game = new EditorGame(_view.SharpDx);
            _game.Disposing += (s, e) => _game = null;
            if (_game != null) RunGame(_game, _view.SharpDx);
            _bridge = new EditorBridge(world, dimension, _game);
        }

        protected override void OnViewLoaded(object view)
        {
            _view = view as EditorView;
            if (_viewGained != null) _viewGained();
        }
    }
}
