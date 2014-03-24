using System.Collections.ObjectModel;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class WorldExplorerModel
    {
        public WorldExplorerModel(ObservableCollection<IFolder> rootFolders)
        {
            Children = new ObservableCollection<WorldBrowserItem>();
            Installations = new ObservableCollection<Installation>();
            Children.Link(Installations);
            Installations.Link(
                rootFolders,
                Installation.ConstructInstallation,
                (folder, install) => install.Folder == folder);
        }

        public ObservableCollection<Installation> Installations { get; private set; }

        public ObservableCollection<WorldBrowserItem> Children { get; private set; }
    }
}
