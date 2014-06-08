using System.ComponentModel.Composition;
using System.IO;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Modules.Installations.Game
{
    [Export(typeof(IInstallationTypeFactory))]
    class GameInstallationFactory : IInstallationTypeFactory
    {
        public IInstallation Create(string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                return null;

            if (!File.Exists(Path.Combine(path, "launcher.jar")))
                return null;

            if (!Directory.Exists(Path.Combine(path, "saves")))
                return null;

            return new GameInstallation(path);
        }
    }
}
