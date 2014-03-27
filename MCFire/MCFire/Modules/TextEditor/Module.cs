using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.TextEditor.Models;
using MCFire.Modules.TextEditor.ViewModels;

namespace MCFire.Modules.TextEditor
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase, IHandle<FileOpenedMessage<TextContent>>
    {

        [ImportingConstructor]
        public Module(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
        }
        public void Handle(FileOpenedMessage<TextContent> message)
        {
            Shell.OpenDocument(new TextEditorViewModel(message.File,message.Content));
        }
    }
}
