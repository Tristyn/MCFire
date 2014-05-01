using System.Collections.ObjectModel;
using System.Linq;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.Explorer.Models
{
    public class GameInstallation : Installation
    {
        ObservableCollection<MCFireWorld> _worlds;

        public GameInstallation(string path)
            : base(path)
        {
        }

        public override InstallationType Type
        {
            get { return InstallationType.Game; }
        }

        public override ObservableCollection<MCFireWorld> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;
                _worlds = new ObservableCollection<MCFireWorld>();
                var savesFolder = Directory.EnumerateDirectories().FirstOrDefault(folder => folder.Name.ToLower() == "saves");
                if (savesFolder == null)
                    return _worlds;
                foreach (var world in savesFolder.EnumerateDirectories()
                    .Select(folder => new MCFireWorld(folder.FullName))
                    .Where(world => world != null))
                {
                    _worlds.Add(world);
                }

                return _worlds;
            }
        }

        public override string Title
        {
            get
            {
                var title= base.Title;
                return title.ToLowerInvariant() == ".minecraft" ? "Mine Craft" : title;
            }
        }
    }
}
