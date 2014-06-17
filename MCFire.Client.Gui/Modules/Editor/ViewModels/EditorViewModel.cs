using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using MCFire.Client.Gui.Modules.Editor.Messages;
using MCFire.Client.Gui.Modules.Editor.Views;
using MCFire.Common;
using MCFire.Common.Infrastructure.DragDrop;
using MCFire.Graphics.Editor;
using SharpDX.Toolkit;
using Action = System.Action;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace MCFire.Client.Gui.Modules.Editor.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EditorViewModel : Document, IDragSource, IDropTarget
    {
        [CanBeNull]
        EditorView _view;
        [CanBeNull]
        IEditorGameFacade _game;
        [CanBeNull]
        Action _viewGained;
        [Import]
        IEventAggregator _aggregator;

        public EditorViewModel()
        {
            DisplayName = "Editor";
        }

        public void TryInitializeTo([NotNull] World world, int dimension)
        {
            if (world == null) throw new ArgumentNullException("world");
            var view = _view;
            if (view != null)
                Initialize(world, dimension);
            else
                // we dont have the view yet. when we do, this will be called.
                _viewGained = () => Initialize(world, dimension);
        }

        void Initialize([NotNull] World world, int dimension)
        {
            var view = _view;

            DisplayName = "Starting Up - Editor";

            var sharpDxElement = view.SharpDx;
            var game = new EditorGameFactory().Create(sharpDxElement, IoC.GetAll<IGameComponent>(), world, dimension);
            _game = game;

            Deactivated += ExitGame;

            // this is an extremely dirty hack. SharpDxElement will dispose when Unloaded is called,
            // but that can be triggered by avalondock, while the control should still be kept alive.
            // We can't prevent disposing because SharpDxElement is sealed, and the methods are private.
            // The only option then is to remove the event handler via reflection.
            // We select the method with the matching name and remove it.
            var handlers = GetRoutedEventHandlers(sharpDxElement, FrameworkElement.UnloadedEvent);
            var sharpDxElementDisposer = handlers.FirstOrDefault(b => b.Handler.Method.Name == "HandleUnloaded");
            sharpDxElement.RemoveHandler(FrameworkElement.UnloadedEvent, sharpDxElementDisposer.Handler);
            _game = game;
            _aggregator.Publish(new EditorOpenedMessage(game));
            _aggregator.Publish(new EditorGainedFocusMessage(game));

            DisplayName = String.Format("{0} - Editor", world.Title);
        }

        /// <summary>
        /// I HAVE SOWN THE SEEDS OF SIN, THIS IS MY PUNISHMENT. Creds to Mauricio Rojas for this slick and dirty code.
        /// returns all event handlers subscribed to the specified routed event in the specified element.
        /// </summary>
        /// <param name="element">The UI element on which the routed event is defined.</param>
        /// <param name="routedEvent">The routed event for which to retrieve the event handlers.</param>
        static IEnumerable<RoutedEventHandlerInfo> GetRoutedEventHandlers([NotNull] UIElement element,
            [NotNull] RoutedEvent routedEvent)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
            // for getting an array of the subscribed event handlers.
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });

            return routedEventHandlers;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = view as EditorView;
            if (_view == null) return;

            if (_viewGained != null) _viewGained();
            _viewGained = null;
            _view.GotFocus += delegate
            {
                var game = _game;
                if (game != null)
                    _aggregator.Publish(new EditorGainedFocusMessage(game));
            };
        }

        private void ExitGame(object sender, DeactivationEventArgs e)
        {
            var game = _game;
            if (!e.WasClosed || game == null)
                return;

            // todo: all of these EditorMessages shouldn't exist
            _aggregator.Publish(new EditorClosingMessage(game));
            game.Dispose();
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
