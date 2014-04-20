using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Results;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Files.ViewModels;

namespace MCFire.Modules.Files
{
#if DEBUG
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Import]
        FolderService _folderService;

        public override void Initialize()
        {
            MainMenu.All.First(item => item.Name == "View")
                .Add(new MenuItem("File Explorer [Deprecated]", OpenFileExplorer));

            _folderService.GetOrCreateFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),".minecraft"));
        }

        private static IEnumerable<IResult> OpenFileExplorer()
        {
            yield return Show.Tool<ExplorerViewModel>();
        }
    }
#endif
}
