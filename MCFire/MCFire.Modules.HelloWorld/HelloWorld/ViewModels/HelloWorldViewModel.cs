using Gemini.Framework;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace MCFire.Modules.HelloWorld.HelloWorld.ViewModels
{
    [DisplayName("Home View Model")]
    [Export(typeof(HelloWorldViewModel))]
    public class HelloWorldViewModel : Document
    {
        public HelloWorldViewModel()
        {
            DisplayName = "Hello World Document";
        }
    }
}
