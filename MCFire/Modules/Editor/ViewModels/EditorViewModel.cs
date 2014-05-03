using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Messages;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Editor.Views;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.ViewModels;
using Action = System.Action;

namespace MCFire.Modules.Editor.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EditorViewModel : SharpDxViewModelBase
    {
        [CanBeNull]
        EditorView _view;
        [CanBeNull]
        Action _viewGained;
        [Import]
        IEventAggregator _aggregator;

        public EditorViewModel()
        {
            DisplayName = "Editor";
        }

        public bool TryInitializeTo([NotNull] MCFireWorld world, int dimension)
        {
            if (_view != null)
                return TryInitializeToInternal(world, dimension);

            // we dont have the view yet. when we do, this will be called
            _viewGained = () => TryInitializeToInternal(world, dimension);
            return true;
        }

        bool TryInitializeToInternal([NotNull] MCFireWorld world, int dimension)
        {
            if (world == null) throw new ArgumentNullException("world");

            var view = _view;
            if (view == null)
                return false;

            DisplayName = "Starting Up - Editor";
            if (!RunGame(() => new EditorGame(view.SharpDx, IoC.GetAll<IGameComponent>(), world, dimension), view.SharpDx))
                return false;

            _aggregator.Publish(new EditorOpenedMessage(this));
            _aggregator.Publish(new EditorGainedFocusMessage(this));

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
                _aggregator.Publish(new EditorGainedFocusMessage(this));
            };
        }

        protected override void ExitGame(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
                _aggregator.Publish(new EditorClosingMessage(this));
            base.ExitGame(sender, e);
        }
    }
}
