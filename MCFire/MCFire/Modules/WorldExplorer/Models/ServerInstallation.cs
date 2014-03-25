using System;
using System.Collections.ObjectModel;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    class ServerInstallation : Installation
    {
        public ServerInstallation(IFolder folder) : base(folder)
        {
            throw new NotImplementedException("servers not implemented");
        }

        public override InstallationType Type
        {
            get { return InstallationType.Server; }
        }

        public override sealed ObservableCollection<World> Worlds { get; protected set; }
        public override sealed ObservableCollection<WorldBrowserItem> Children { get; protected set; }
    }
}
