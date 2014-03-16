using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;
using MCFire.Modules.Infrastructure;

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
