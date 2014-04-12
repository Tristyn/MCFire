using System.Collections.ObjectModel;
using System.Linq;
using MCFire.Modules.Infrastructure.Extensions;
using Substrate;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class GameInstallation : Installation
    {
        private ObservableCollection<NbtWorld> _worlds;

        public GameInstallation(string path)
            : base(path)
        {
        }

        public override InstallationType Type
        {
            get { return InstallationType.Game; }
        }

        public override ObservableCollection<NbtWorld> Worlds
        {
            get
            {
                if (_worlds != null) return _worlds;

                _worlds = new ObservableCollection<NbtWorld>();
                var savesFolder = Directory.EnumerateDirectories().FirstOrDefault(folder => folder.Name.ToLower() == "saves");
                if (savesFolder == null)
                    return _worlds;
                _worlds.AddForeach(
                    savesFolder.EnumerateDirectories()
                    .Select(folder => NbtWorld.Open(folder.FullName))
                    .Where(world => world != null));

                return _worlds;
            }
            protected set { _worlds = value; }
        }

        public override string Title
        {
            get
            {
                var title= base.Title;
                return title == ".minecraft" ? "Mine Craft" : title;
            }
        }
    }
}
