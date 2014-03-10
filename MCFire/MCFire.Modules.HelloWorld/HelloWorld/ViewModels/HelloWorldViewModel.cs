using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
