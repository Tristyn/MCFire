using System.Collections.ObjectModel;
using System.Linq;
using MCFire.Client.Modules.Explorer.Models;
using MCFire.Common;

namespace MCFire.Client.Services.Explorer
{
    public class GameInstallation : Installation
    {
        ObservableCollection<World> _worlds;

        public GameInstallation(string path)
            : base(path)
        {
        }

        public override InstallationType Type
        {
            get { return InstallationType.Game; }
        }

        public override ObservableCollection<World> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;
                _worlds = new ObservableCollection<IWorld>();
                var savesFolder = Directory.EnumerateDirectories().FirstOrDefault(folder => folder.Name.ToLower() == "saves");
                if (savesFolder == null)
                    return _worlds;
                foreach (var world in savesFolder.EnumerateDirectories()
                    .Select(folder => new IWorld(folder.FullName))
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
