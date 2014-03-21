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
#pragma warning disable 0649
        [Import]
        FolderService _folderService;
#pragma warning restore 0649

        public override void Initialize()
        {
            MainMenu.All.First(item => item.Name == "View")
                .Add(new MenuItem("File Explorer", OpenFileExplorer));

            _folderService.GetOrCreateFolder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        private static IEnumerable<IResult> OpenFileExplorer()
        {
            yield return Show.Tool<ExplorerViewModel>();
        }
    }
}