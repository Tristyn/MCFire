using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Client.Modules.Installations.Game;
using MCFire.Client.Modules.Installations.Server;
using MCFire.Common;

namespace MCFire.Client.Primitives.Installations
{
    public abstract class Installation:IInstallation
    {
        readonly string _path;
        protected readonly DirectoryInfo Directory;

        protected Installation([NotNull] string path)
        {
            _path = path;
            Directory = new DirectoryInfo(path);
        }

        public abstract ObservableCollection<World> Worlds { get; }

        /// <summary>
        /// Detects if an installation is a server or game, and returns an instance.
        /// </summary>
        /// <param name="path">The path to the installation</param>
        [CanBeNull]
        public static Installation New(string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                return null;

            if (directory.EnumerateFiles().Any(
                file => file.Name.ToLower() == "options.txt" || file.Name.ToLower()=="launcher.jar"))
            {
                return new GameInstallation(path);
            }
            if (directory.EnumerateFiles().Any(file => file.Name.ToLower() == "server.properties")
                || directory.EnumerateFiles().Any(file => file.Name.ToLower() == "white-list.txt"))
            {
                return new ServerInstallation(path);
            }
            return null;
        }

        public virtual string Title
        {
            get { return Directory.Name; }
        }

        public string Path
        {
            get { return _path; }
        }
    }
}

