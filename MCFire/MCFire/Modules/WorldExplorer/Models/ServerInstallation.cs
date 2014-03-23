using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    class ServerInstallation : Installation
    {
        public ServerInstallation(IFolder folder) : base(folder)
        {
        }

        public override InstallationType Type
        {
            get { return InstallationType.Server; }
        }
    }
}
