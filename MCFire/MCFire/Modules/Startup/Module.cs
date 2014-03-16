using System.ComponentModel.Composition;
using Gemini.Framework;

namespace MCFire.Modules.Startup
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
