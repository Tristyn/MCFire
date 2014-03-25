using System.Collections.ObjectModel;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class WorldExplorerModel
    {
        public WorldExplorerModel(ObservableCollection<IFolder> rootFolders)
        {
            Installations = rootFolders.Link<Installation, IFolder>(
                Installation.ConstructInstallation,
                (folder, install) => install.Folder == folder);
            Children = Installations.Link<WorldBrowserItem, Installation, ObservableCollection<WorldBrowserItem>>();
        }

        public ObservableCollection<Installation> Installations { get; private set; }

        public ObservableCollection<WorldBrowserItem> Children { get; private set; }
    }
}
