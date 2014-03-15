﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Files.ViewModels;

namespace MCFire.Modules.Files
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            MainMenu.All.First(item => item.Name == "View")
                .Add(new MenuItem("File Explorer", OpenFileExplorer));
            MainWindow.Title = "FileManager Demo";

            IoC.Get<FolderService>()
                .GetOrCreateFolder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Shell.ShowTool(IoC.Get<ExplorerViewModel>());
        }

        private static IEnumerable<IResult> OpenFileExplorer()
        {
            yield return Show.Tool<ExplorerViewModel>();
        } 
    }
}
