using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Messages;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Editor.ViewModels;
using MCFire.Modules.Startup.Models;
using MCFire.Modules.Toolbox.Messages;
using MCFire.Modules.Toolbox.Models;

namespace MCFire.Modules.Toolbox.Services
{
    /// <summary>
    /// Manages the lifetime of each tool instances for each editor.
    /// </summary>
    [Export]
    [Export(typeof(ICreateAtStartup))]
    public class ToolboxService : IHandle<EditorOpenedMessage>, IHandle<EditorGainedFocusMessage>, IHandle<EditorClosingMessage>, ICreateAtStartup
    {
        [NotNull]
        readonly List<EditorToolbox> _toolBoxes = new List<EditorToolbox>();
        IEventAggregator _aggregator;
        private EditorToolbox _currentToolbox;

        void IHandle<EditorOpenedMessage>.Handle(EditorOpenedMessage message)
        {
            // create a new toolbox for this instance, notify with publish
            CurrentToolbox= GetToolboxForEditor(message.EditorGame);
        }

        void IHandle<EditorGainedFocusMessage>.Handle(EditorGainedFocusMessage message)
        {
            CurrentToolbox = GetToolboxForEditor(message.EditorGame);
        }

        void IHandle<EditorClosingMessage>.Handle(EditorClosingMessage message)
        {
            // dispose of the toolbox
            var tool = _toolBoxes.FirstOrDefault(toolbox => toolbox.Editor == message.EditorGame);
            Debug.Assert(tool != null);
            tool.Dispose();
            _toolBoxes.Remove(tool);

            if (CurrentToolbox != tool) return;
            CurrentToolbox = null;
        }

        /// <summary>
        /// Returns the <see cref="Toolbox"/> for the specified <see cref="EditorViewModel"/>.
        /// </summary>
        /// <param name="editor">The <see cref="EditorViewModel"/> that the <see cref="Toolbox"/> is initialized to.</param>
        /// <returns></returns>
        [NotNull]
        public EditorToolbox GetToolboxForEditor([NotNull] EditorGame editor)
        {
            if (editor == null) throw new ArgumentNullException("editor");
            var tools = _toolBoxes.FirstOrDefault(toolbox => toolbox.Editor == editor);
            if (tools != null)
                return tools;

            // create a new toolbox
            tools = IoC.Get<EditorToolbox>();
            tools.Initialize(editor);
            _toolBoxes.Add(tools);
            return tools;
        }

        [CanBeNull]
        public EditorToolbox CurrentToolbox
        {
            get { return _currentToolbox; }
            private set
            {
                _currentToolbox = value;
                Aggregator.Publish(new CurrentToolboxChangedMessage(value));
            }
        }

        [Import]
        IEventAggregator Aggregator
        {
            get { return _aggregator; }
            set
            {
                _aggregator = value;
                value.Subscribe(this);
            }
        }
    }
}
