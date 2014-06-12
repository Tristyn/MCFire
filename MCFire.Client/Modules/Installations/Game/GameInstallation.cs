using System.Collections.ObjectModel;
using System.Linq;
using MCFire.Client.Primitives.Installations;
using MCFire.Common;

namespace MCFire.Client.Modules.Installations.Game
{
    class GameInstallation : Installation
    {
        ObservableCollection<World> _worlds;

        internal GameInstallation(string path)
            : base(path)
        {
        }

        public override ObservableCollection<World> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;

                _worlds = new ObservableCollection<World>();
                var savesFolder = Directory.EnumerateDirectories().FirstOrDefault(folder => folder.Name.ToLower() == "saves");
                if (savesFolder == null)
                    return _worlds;
                foreach (var world in savesFolder.EnumerateDirectories()
                    .Select(folder => World.Open(folder.FullName))
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
