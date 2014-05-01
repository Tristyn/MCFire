using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.Explorer.ViewModels;

namespace MCFire.Modules.Explorer
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
