using Gemini.Framework;
using System.ComponentModel.Composition;

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
