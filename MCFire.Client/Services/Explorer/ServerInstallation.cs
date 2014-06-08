using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MCFire.Client.Modules.Explorer.Models
{
    class ServerInstallation : Installation
    {
        private ObservableCollection<MCFireWorld> _worlds;

        public ServerInstallation([NotNull] string folder) : base(folder)
        {
        }

        public override InstallationType Type
        {
            get { return InstallationType.Server; }
        }

        public override ObservableCollection<MCFireWorld> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;

                _worlds = new ObservableCollection<MCFireWorld>();
                foreach (var world in Directory.EnumerateDirectories()
                    .Select(folder => new MCFireWorld(folder.FullName))
                    .Where(world => world != null))
                {
                    _worlds.Add(world);
                }

                return _worlds;
            }
        }
    }
}
