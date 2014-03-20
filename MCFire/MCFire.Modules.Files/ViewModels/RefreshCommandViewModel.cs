﻿using System.ComponentModel.Composition;
using System.Linq;
using MCFire.Modules.Files.Commands;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IFileExplorerCommand))]
    public class RefreshCommandViewModel : IFileExplorerCommand
    {
        public IFileExplorerViewModel FileExplorer { set; private get; }

        public async void Refresh()
        {
            var selectedFolderitem = FileExplorer.SelectedItem;
            if (selectedFolderitem != null)
            {
                await selectedFolderitem.Model.RefreshAsync();
                return;
            }

            // fall back to refreshing everything (OH GOD LOL)
            foreach (var folder in FileExplorer.RootFolders)
            {
                await folder.Model.RefreshAsync();
            }
        }

        public bool CanRefresh()
        {
            return FileExplorer.SelectedItem != null || (FileExplorer != null && FileExplorer.RootFolders.Any());
        }
    }
}
