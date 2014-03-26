using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.HelloWorld.Test3D.ViewModels;

namespace MCFire.Modules.HelloWorld.Test3D
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            MainMenu.All.First(menu => menu.Name=="View")
                .Add(new MenuItem("3D Test", Open3DTest));
        }

        static IEnumerable<IResult> Open3DTest()
        {
            yield return Show.Document<D3DViewModel>();
        }
    }
}
