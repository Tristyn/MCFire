﻿using System.ComponentModel.Composition;
using System.Linq;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.ViewModels;

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
                await selectedFolderitem.Model.Refresh();
                return;
            }

            // fall back to refreshing everything (OH GOD LOL)
            foreach (var folder in FileExplorer.RootFolders)
            {
                await folder.Model.Refresh();
            }
        }

        public bool CanRefresh()
        {
            return FileExplorer.SelectedItem != null || (FileExplorer != null && FileExplorer.RootFolders.Any());
        }
    }
}
