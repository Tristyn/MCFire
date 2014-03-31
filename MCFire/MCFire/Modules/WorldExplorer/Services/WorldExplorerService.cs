using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.WorldExplorer.Models;

namespace MCFire.Modules.WorldExplorer.Services
{
    [Export]
    public class WorldExplorerService
    {
        [ImportingConstructor]
        public WorldExplorerService(FolderService folderService)
        {
            Installations = folderService.RootFolders.Link(
                Installation.ConstructInstallation,
                (folder, install) => install.Folder == folder);
            Children = Installations.Link<WorldBrowserItem, Installation, ObservableCollection<WorldBrowserItem>>();
        }

        public ObservableCollection<Installation> Installations { get; private set; }

        public ObservableCollection<WorldBrowserItem> Children { get; private set; }
    }
}