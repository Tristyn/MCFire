using System.ComponentModel.Composition;
using Gemini.Framework;

namespace MCFire.Modules.Main.Startup
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
