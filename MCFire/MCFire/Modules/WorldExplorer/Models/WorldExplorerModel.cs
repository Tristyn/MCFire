using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class WorldExplorerModel
    {
        public WorldExplorerModel(ObservableCollection<IFolder> rootFolders)
        {
            Installations = new ObservableCollection<Installation>();
            Children = new ObservableCollection<WorldBrowserItem>();
            Children.Link(Installations);

            // TODO: Link with transformations
            rootFolders.CollectionChanged += HandleRootFolders;
            HandleRootFolders(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, rootFolders, 0));
        }

        private void HandleRootFolders(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.Handle<Installation, IFolder>(Installations, Installation.ConstructInstallation, (folder, install) => install.Folder == folder);
        }

        private void HandleWorldBrowserItems(object s, NotifyCollectionChangedEventArgs e)
        {
            e.Handle<WorldBrowserItem, Installation>(Children, install => install, (install, item) => install == item);
        }

        public ObservableCollection<Installation> Installations { get; private set; }

        public ObservableCollection<WorldBrowserItem> Children { get; private set; }
    }
}
