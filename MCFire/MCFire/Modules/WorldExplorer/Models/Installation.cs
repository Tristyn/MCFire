using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.WorldExplorer.Models
{
    public abstract class Installation : WorldBrowserItem
    {
        protected Installation(IFolder folder) : base(folder)
        {
            Worlds = new ObservableCollection<World>();
            Worlds.CollectionChanged += HandleWorlds;
        }

        void HandleWorlds(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.Handle<WorldBrowserItem, World>(Children, item => item, (world, worldItem) => world == worldItem);
        }

        public abstract InstallationType Type { get; }

        public ObservableCollection<World> Worlds { get; private set; }

        [CanBeNull]
        public static Installation ConstructInstallation([NotNull] IFolder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            if (folder.Files.Any(file => file.Name.ToLower() == "launcher.jar"))
            {
                return new GameInstallation(folder);
            }
            if (folder.Files.Any(file => file.Name.ToLower() == "server.properties")
                     || folder.Files.Any(file => file.Name.ToLower() == "white-list.txt"))
            {
                return new ServerInstallation(folder);
            }
            return null;
        }
    }

    public enum InstallationType
    {
        Game,
        Server
    }
}
