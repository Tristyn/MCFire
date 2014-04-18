using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.Extensions;
using Substrate;

namespace MCFire.Modules.Explorer.Models
{
    class ServerInstallation : Installation
    {
        private ObservableCollection<NbtWorld> _worlds;

        public ServerInstallation([NotNull] string folder) : base(folder)
        {
        }

        public override InstallationType Type
        {
            get { return InstallationType.Server; }
        }

        public override ObservableCollection<NbtWorld> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;

                _worlds = new ObservableCollection<NbtWorld>();
                _worlds.AddForeach(
                    Directory.EnumerateDirectories()
                    .Select(folder => NbtWorld.Open(folder.FullName))
                    .Where(world => world != null));

                return _worlds;
            }
            protected set { _worlds = value; }
        }
    }
}
