using System;
using System.Collections.Generic;
using MCFire.Client.Gui.Modules.Explorer.ViewModels;

namespace MCFire.Client.Gui.Modules.Explorer
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            MainMenu.All.First(item => item.Name == "View")
              .Add(new MenuItem("World Explorer", OpenWorldExplorer));
        }

        static IEnumerable<IResult> OpenWorldExplorer()
        {
            yield return Show.Tool<WorldExplorerViewModel>();
        }

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof (WorldExplorerViewModel); }
        }
    }
}
