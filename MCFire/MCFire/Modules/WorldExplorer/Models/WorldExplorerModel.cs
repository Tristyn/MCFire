using System.Collections.Generic;
using System.Collections.Specialized;
using Caliburn.Micro;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class WorldExplorerModel
    {
        public WorldExplorerModel(BindableCollection<IFolder> rootFolders)
        {
            Installations = new BindableCollection<Installation>();
            Children = new BindableCollection<WorldBrowserItem>();
            Installations.CollectionChanged += HandleWorldBrowserItems;
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

        public BindableCollection<Installation> Installations { get; private set; }

        public BindableCollection<WorldBrowserItem> Children { get; private set; }
    }
}
