﻿using System.ComponentModel.Composition;
using MCFire.Modules.Files.Commands;

namespace MCFire.Modules.Files.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IFileExplorerCommand))]
    [Export]
    public class NewFolderCommandViewModel : IFileExplorerCommand
    {
        public IFileExplorerViewModel FileExplorer { set; private get; }

        public void AddFolder()
        {
            FileExplorer.NewFolder();
        }
        public bool Visible { get; set; }
    }
}
