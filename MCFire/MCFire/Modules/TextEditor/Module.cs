using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Models;
using MCFire.Modules.TextEditor.Models;
using MCFire.Modules.TextEditor.ViewModels;

namespace MCFire.Modules.TextEditor
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase, IHandle<FileOpenedMessage<TextFile>>
    {

        [ImportingConstructor]
        public Module(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
        }

        public void Handle(FileOpenedMessage<TextFile> message)
        {
            Shell.OpenDocument(new TextEditorViewModel(message.File));
        }
    }
}
