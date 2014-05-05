using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.StatsForNerds.ViewModels;

namespace MCFire.Modules.StatsForNerds
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            MainMenu.All.First(item=> item.Name=="View").Add(new MenuItem("Stats for Nerds", Execute));
        }

        static IEnumerable<IResult> Execute()
        {
            yield return Show.Tool<StatsForNerdsViewModel>();
        }
    }
}
