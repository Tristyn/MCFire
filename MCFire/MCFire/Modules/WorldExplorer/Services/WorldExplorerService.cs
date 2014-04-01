using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using MCFire.Modules.Files.Models;
using MCFire.Modules.WorldExplorer.Models;

namespace MCFire.Modules.WorldExplorer.Services
{
    [Export]
    public class WorldExplorerService
    {
        List<Installation> _installations = new List<Installation>();
        
        public WorldExplorerService()
        {
            _installations.Add(Installation.New(@"C:\Users\Tristyn\AppData\Roaming\.minecraft"));
        }

        public ObservableCollection<Installation> Installations { get; private set; }

        public ObservableCollection<WorldBrowserItem> Children { get; private set; }
    }
}