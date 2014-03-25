using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public abstract class Installation : WorldBrowserItem
    {
        protected Installation(IFolder folder) : base(folder)
        {
            
        }

        public abstract InstallationType Type { get; }

        public abstract ObservableCollection<World> Worlds { get; protected set; }

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
