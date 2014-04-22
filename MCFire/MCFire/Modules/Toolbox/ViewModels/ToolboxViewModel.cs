using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Messages;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    [Export]
    public class ToolboxViewModel : Tool, IHandle<EditorGainedFocusMessage>, IHandle<EditorClosingMessage>
    {
        [NotNull]
        List<Toolbox> _tools = new List<Toolbox>();

        public ToolboxViewModel()
        {
            DisplayName = "Tools";
        }

        public void Handle(EditorGainedFocusMessage message)
        {
            Console.WriteLine("Hah!");
        }

        public void Handle(EditorClosingMessage message)
        {
            var tool = _tools.FirstOrDefault(toolbox => toolbox.Editor == message.EditorViewModel);
            if (tool == null) return;

            _tools.RemoveAll(toolbox => toolbox.Editor == message.EditorViewModel);
        }

        Toolbox GetToolboxForEditor(EditorViewModel editor)
        {
            var tool = _tools.FirstOrDefault(toolbox => toolbox.Editor == editor);
            if (tool != null)
                return tool;

            // create a new toolbox
            tool = IoC.Get<Toolbox>();
            tool.Initialize(editor);
            return tool;
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }

        [Import]
        public IEventAggregator Aggregator
        {
            set { value.Subscribe(this); }
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Toolbox : IDisposable
    {
        [ImportMany(typeof(IEditorTool))]
        IEnumerable<IEditorTool> _tools;
        public EditorViewModel Editor;

        public void Initialize(EditorViewModel editor)
        {
            Editor = editor;
            foreach (var tool in _tools)
            {
                tool.Initialize(editor);
            }
        }

        public void Dispose()
        {
            foreach (var tool in _tools)
            {
                tool.Dispose();
            }
            _tools = null;
            Editor = null;
        }
    }
}
