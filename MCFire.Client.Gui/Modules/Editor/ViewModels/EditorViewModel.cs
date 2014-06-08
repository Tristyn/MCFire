using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using MCFire.Client.Gui.Modules.Editor.Messages;
using MCFire.Common;
using MCFire.Graphics.Modules.Editor.Messages;
using MCFire.Graphics.Modules.Editor.Models;
using MCFire.Graphics.Modules.Editor.Views;
using Action = System.Action;

namespace MCFire.Client.Gui.Modules.Editor.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EditorViewModel : SharpDxViewModelBase, IDragSource, IDropTarget
    {
        [CanBeNull]
        EditorView _view;
        [CanBeNull]
        EditorGame _game;
        [CanBeNull]
        Action _viewGained;
        [Import]
        IEventAggregator _aggregator;

        public EditorViewModel()
        {
            DisplayName = "Editor";
        }

        public bool TryInitializeTo([NotNull] World world, int dimension)
        {
            if (_view != null)
                return TryInitializeToInternal(world, dimension);

            // we dont have the view yet. when we do, this will be called
            _viewGained = () => TryInitializeToInternal(world, dimension);
            return true;
        }

        bool TryInitializeToInternal([NotNull] World world, int dimension)
        {
            if (world == null) throw new ArgumentNullException("world");

            var view = _view;
            if (view == null)
                return false;

            DisplayName = "Starting Up - Editor";
            if (!RunGame(() =>
            {
                var game = new EditorGame(view.SharpDx, IoC.GetAll<IGameComponent>(), world, dimension);
                _game = game;
                _aggregator.Publish(new EditorOpenedMessage(game));
                _aggregator.Publish(new EditorGainedFocusMessage(game));
                return game;
            }, view.SharpDx))
                return false;

            DisplayName = String.Format("{0} - Editor", world.Title);
            return true;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = view as EditorView;
            if (_view == null) return;

            if (_viewGained != null) _viewGained();
            _view.GotFocus += delegate
            {
                var game = _game;
                if (game != null)
                    _aggregator.Publish(new EditorGainedFocusMessage(game));
            };
        }

        protected override void ExitGame(object sender, DeactivationEventArgs e)
        {
            var game = _game;
            if (e.WasClosed&&game!=null)
                _aggregator.Publish(new EditorClosingMessage(game));
            base.ExitGame(sender, e);
        }

        public void WpfKeyDown(KeyEventArgs e)
        {
            var game = _game;
            if (game == null) return;
            game.WpfKeyDown(e);
        }

        #region Drag Drop

        #region Drag Source

        public void StartDrag(IDragInfo dragInfo)
        {
            var game = _game;
            if (game == null) return;
            game.StartDrag(new HandleableDragInfo(dragInfo));
        }

        public void Dropped(IDropInfo dropInfo)
        {
            var game = _game;
            if (game == null) return;
            game.Dropped(dropInfo);
        }

        public void DragCancelled()
        {
            var game = _game;
            if (game == null) return;
            game.DragCancelled();
        }

        #endregion

        #region Drop Target

        public void DragOver(IDropInfo dropInfo)
        {
            var game = _game;
            if (game == null) return;
            game.DragOver(dropInfo);
        }

        public void Drop(IDropInfo dropInfo)
        {
            var game = _game;
            if (game == null) return;
            game.Drop(dropInfo);
        }

        #endregion

        #endregion
    }
}
