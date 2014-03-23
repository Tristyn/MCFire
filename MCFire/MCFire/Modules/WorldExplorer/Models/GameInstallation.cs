using System.Linq;
using MCFire.Modules.Files.Models;
using MCFire.Modules.TextEditor.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class GameInstallation : Installation
    {
        public GameInstallation(IFolder folder) : base(folder)
        {
            
        }

        public override string Title
        {
            get { return Folder.Name.ToLower() == ".minecraft" ? "Mine Craft" : Folder.Name; }
        }

        public override InstallationType Type
        {
            get { return InstallationType.Game; }
        }

        public TextFile Options
        {
            get { return Folder.Files.FirstOrDefault(file => file.Name.ToLower() == "options.txt") as TextFile; }
        }
    }
}
