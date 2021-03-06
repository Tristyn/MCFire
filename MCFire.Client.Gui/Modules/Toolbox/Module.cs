﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Client.Gui.Modules.Toolbox.ViewModels;

namespace MCFire.Client.Gui.Modules.Toolbox
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            MainMenu.All.First(menu=> menu.Name=="View").Add(new MenuItem("Tools", OpenTools));
        }

        static IEnumerable<IResult> OpenTools()
        {
            yield return Show.Tool<ToolboxViewModel>();
        }

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(ToolboxViewModel); }
        }
    }
}
