using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Modules.Metro.Commands;

namespace MCFire.Modules.HelloWorld.HelloWorld.ViewModels
{
    [Export(typeof(IWindowCommand))]
    public class HelloWorldCommandViewModel : IWindowCommand
    {
#pragma warning disable 0649
        [Import]
        IShell _shell;
#pragma warning restore 0649

        public void OpenView()
        {
           _shell.OpenDocument(IoC.Get<HelloWorldViewModel>());
        }
    }
}
