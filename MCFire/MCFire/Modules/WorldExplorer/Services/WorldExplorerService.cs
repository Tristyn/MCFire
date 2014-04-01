using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using MCFire.Modules.WorldExplorer.Models;

namespace MCFire.Modules.WorldExplorer.Services
{
    [Export]
    public class WorldExplorerService
    {
        readonly ObservableCollection<Installation> _installations = new ObservableCollection<Installation>();
        
        public WorldExplorerService()
        {
            _installations.Add(Installation.New(@"C:\Users\Tristyn\AppData\Roaming\.minecraft"));
        }

        public ObservableCollection<Installation> Installations { get { return _installations; } }
    }
}