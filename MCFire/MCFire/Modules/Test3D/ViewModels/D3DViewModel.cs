
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Gemini.Framework;
using JetBrains.Annotations;
using MCFire.Modules.Test3D.Models;
using SharpDX.Toolkit;

namespace MCFire.Modules.Test3D.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class D3DViewModel : Document
    {
        D3DTestGame _game;
        bool _loadedFirstTime;

        public D3DViewModel()
        {
            Deactivated += D3DViewModel_Deactivated;
        }

        void D3DViewModel_Deactivated(object sender, Caliburn.Micro.DeactivationEventArgs e)
        {
            _game.Exit();
            _game.Dispose();
        }

        public void Loaded(object sharpDx)
        {
            // controls can be loaded multiple times because of avalondock docking.
            // run this code only once
            if (_loadedFirstTime)
                return;

            _game = new D3DTestGame();
            _game.Run(sharpDx as SharpDXElement);

            // this is an extremely dirty hack. SharpDxElement will dispose when Unloaded is called,
            // but that can be triggered by avalondock, while the control should still be kept alive.
            // We can't prevent disposing because SharpDxElement is sealed, and the methods are private.
            // The only option then is to remove the event handler via reflection.
            // We select the method with the matching name and remove it.
            var sharpDxElement = (sharpDx as SharpDXElement);
            if(sharpDxElement==null)
                return;
            var handlers = GetRoutedEventHandlers(sharpDxElement, FrameworkElement.UnloadedEvent);
            var sharpDxElementDisposer = handlers.FirstOrDefault(b => b.Handler.Method.Name == "HandleUnloaded");
            sharpDxElement.RemoveHandler(FrameworkElement.UnloadedEvent,sharpDxElementDisposer.Handler);

            _loadedFirstTime = true;
        }

        /// <summary>
        /// I HAVE SOWN THE SEEDS OF SIN, THIS IS MY PUNISHMENT. Creds to Mauricio Rojas for this slick and dirty code.
        /// returns all event handlers subscribed to the specified routed event in the specified element.
        /// </summary>
        /// <param name="element">The UI element on which the routed event is defined.</param>
        /// <param name="routedEvent">The routed event for which to retrieve the event handlers.</param>
        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers([NotNull] UIElement element,
            [NotNull] RoutedEvent routedEvent)
        {
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

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
