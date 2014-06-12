using System.ComponentModel.Composition;
using System.IO;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Modules.Installations.Server
{
    [Export(typeof(IInstallationTypeFactory))]
    class ServerInstallationFactory : IInstallationTypeFactory
    {
        public IInstallation Create(string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                return null;

            if (!File.Exists(Path.Combine(path, "server.properties")))
                return null;

            if (!Directory.Exists(Path.Combine(path, "white-list.txt")))
                return null;

            return new ServerInstallation(path);
        }
    }
}
