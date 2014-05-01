using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Messages;
using MCFire.Modules.Editor.ViewModels;
using MCFire.Modules.Toolbox.Models;

namespace MCFire.Modules.Toolbox.Services
{
    /// <summary>
    /// Manages the lifetime of each tool instances for each editor.
    /// </summary>
    [Export]
    public class ToolboxService : IHandle<EditorCreatedMessage>, IHandle<EditorClosingMessage>
    {
        [NotNull]
        readonly List<EditorToolbox> _toolBoxes = new List<EditorToolbox>();

        public void Handle(EditorCreatedMessage message)
        {
            var tool = IoC.Get<EditorToolbox>();
            tool.Initialize(message.EditorViewModel);
            _toolBoxes.Add(tool);
        }

        public void Handle(EditorClosingMessage message)
        {
            var tool = _toolBoxes.First(toolbox => toolbox.Editor == message.EditorViewModel);
            tool.Dispose();
            _toolBoxes.Remove(tool);
        }

        public EditorToolbox GetToolboxForEditor(EditorViewModel editor)
        {
            return _toolBoxes.First(toolbox => toolbox.Editor == editor);
        }

        [Import]
        IEventAggregator Aggregator { set { value.Subscribe(this); } }
    }
}
