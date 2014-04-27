using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.Explorer.Models
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
                _worlds.AddForeach(
                    Directory.EnumerateDirectories()
                    .Select(folder => new MCFireWorld(folder.FullName))
                    .Where(world => world != null));

                return _worlds;
            }
        }
    }
}
