using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using MCFire.Modules.Explorer.Models;

namespace MCFire.Modules.Explorer.Services
{
    [Export]
    public class WorldExplorerService
    {
        readonly ObservableCollection<Installation> _installations = new ObservableCollection<Installation>();

        public WorldExplorerService()
        {
#if DEBUG && !FIRSTRUN
            // if debug, add game installation automatically
            if(Installations.Count!=0)
                return;
            var path =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")
                    .ToLower();
            var gameInstall = Installation.New(path);
            if(gameInstall!=null)
                Installations.Add(gameInstall);
#endif
        }

        public ObservableCollection<Installation> Installations { get { return _installations; } }
    }
}