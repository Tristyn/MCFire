using Gemini;
using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MCFire
{
    class MCFireBootstrapper : AppBootstrapper
    {
        protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
        {
            var list = new List<Assembly>(base.SelectAssemblies())
            {
                typeof(Gemini.Modules.Metro.ViewModels.MainWindowViewModel).Assembly,
                typeof(MCFire.Modules.HelloWorld.HelloWorld.ViewModels.HelloWorldViewModel).Assembly
            };
            return list;
        }
    }
}
