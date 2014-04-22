using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Editor.Messages;
using MCFire.Modules.Editor.ViewModels;

namespace MCFire.Modules.Toolbox.ViewModels
{
    [Export]
    public class ToolboxViewModel : Tool, IHandle<EditorGainedFocusMessage>
    {
        [Import]
        IEventAggregator _aggregator;


        public ToolboxViewModel()
        {
            DisplayName = "Tools";
            _aggregator.Subscribe(this);
        }

        public void Handle(EditorGainedFocusMessage message)
        {
            Console.WriteLine("Hah!");
        }

        void GetToolsForEditor(EditorViewModel editor)
        {

        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Toolbox
    {
        [ImportMany(typeof(IEditorTool))]
        IEnumerable<IEditorTool> _tools;

        void Initialize(EditorViewModel editor)
        {
            foreach (var tool in _tools)
            {
                tool.Initialize(editor);
            }
        }
    }
}
