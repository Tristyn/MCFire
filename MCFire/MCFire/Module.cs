using Gemini.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCFire
{
    [Export(typeof(IModule))]
    class Module : ModuleBase
    {
        public override void Initialize()
        {
            MainWindow.Title = "MCFire - Getchyo snacks!";
        }
    }
}
