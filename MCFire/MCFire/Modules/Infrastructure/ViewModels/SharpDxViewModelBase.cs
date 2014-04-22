using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using JetBrains.Annotations;
using SharpDX.Toolkit;

namespace MCFire.Modules.Infrastructure.ViewModels
{
    public class SharpDxViewModelBase : Document
    {
        Game _game;
        SharpDXElement _sharpDxElement;

        /// <summary>
        /// Manages the lifetime of a Game and a SharpDxElement. 
        /// This calls game.Run(SharpDxElement), and it will call dispose on both objects when the document closes.
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="sharpDxElement">The SharpDxElement for the game to run on</param>
        protected virtual void RunGame([NotNull] Game game, SharpDXElement sharpDxElement)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (sharpDxElement == null) throw new ArgumentNullException("sharpDxElement");

            _game = game;
            _sharpDxElement = sharpDxElement;

            game.Run(sharpDxElement);
            Deactivated += ExitGame;

            // this is an extremely dirty hack. SharpDxElement will dispose when Unloaded is called,
            // but that can be triggered by avalondock, while the control should still be kept alive.
            // We can't prevent disposing because SharpDxElement is sealed, and the methods are private.
            // The only option then is to remove the event handler via reflection.
            // We select the method with the matching name and remove it.
            var handlers = GetRoutedEventHandlers(sharpDxElement, FrameworkElement.UnloadedEvent);
            var sharpDxElementDisposer = handlers.FirstOrDefault(b => b.Handler.Method.Name == "HandleUnloaded");
            sharpDxElement.RemoveHandler(FrameworkElement.UnloadedEvent, sharpDxElementDisposer.Handler);
        }

        protected virtual void ExitGame(object sender, DeactivationEventArgs e)
        {
            if (!e.WasClosed)
                return;

            if (_game != null)
            {
                _game.Exit();
                _game.Dispose();
                _game = null;
            }
            
            _sharpDxElement.Dispose();
            _sharpDxElement = null;
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
    }
}
