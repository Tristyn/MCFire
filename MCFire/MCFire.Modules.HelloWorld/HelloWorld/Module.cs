﻿using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.HelloWorld.HelloWorld;
using MCFire.Modules.HelloWorld.HelloWorld.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCFire.Modules.HelloWorld.HelloWorld
{

    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                yield return IoC.Get<HelloWorldViewModel>();
            }
        }

        public override void Initialize()
        {
            MainMenu.All.First(menuItem => menuItem.Name == "View")
                .Add(new MenuItem("Hello World", OpenHelloWorld));
        }

        public override void PostInitialize()
        {
            Shell.OpenDocument(IoC.Get<HelloWorldViewModel>());
            
        }

        private IEnumerable<IResult> OpenHelloWorld()
        {
            yield return Show.Document<HelloWorldViewModel>();
        }
    }
}