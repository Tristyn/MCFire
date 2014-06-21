using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Client.Primitives.Installations;
using MCFire.Common;

namespace MCFire.Client.Components.InstallationTypeFactory
{
    class ServerInstallation : Installation
    {
        ObservableCollection<World> _worlds;

        internal ServerInstallation([NotNull] string folder) : base(folder)
        {
        }

        public override ObservableCollection<World> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;

                _worlds = new ObservableCollection<World>();
                foreach (var world in Directory.EnumerateDirectories()
                    .Select(folder => World.Open(folder.FullName))
                    .Where(world => world != null))
                {
                    _worlds.Add(world);
                }

                return _worlds;
            }
        }
    }
}
