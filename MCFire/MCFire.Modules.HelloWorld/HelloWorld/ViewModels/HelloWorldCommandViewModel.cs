using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Modules.Metro.Commands;

namespace MCFire.Modules.HelloWorld.HelloWorld.ViewModels
{
    [Export(typeof(IWindowCommand))]
    public class HelloWorldCommandViewModel : IWindowCommand
    {
        [Import] IShell _shell;

        public void OpenView()
        {
           _shell.OpenDocument(IoC.Get<HelloWorldViewModel>());
        }
    }
}
