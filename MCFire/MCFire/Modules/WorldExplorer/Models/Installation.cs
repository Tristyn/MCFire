using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Substrate;

namespace MCFire.Modules.WorldExplorer.Models
{
    public abstract class Installation : WorldBrowserItem
    {
        protected readonly string Path;
        protected readonly DirectoryInfo Directory;

        protected Installation([NotNull] string path)
        {
            Path = path;
            Directory = new DirectoryInfo(path);
        }

        public abstract InstallationType Type { get; }

        public abstract ObservableCollection<NbtWorld> Worlds { get; protected set; }

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

            if (directory.EnumerateFiles().Any(file => file.Name.ToLower() == "launcher.jar"))
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

        public override string Title
        {
            get { return Directory.Name; }
        }
    }

    public enum InstallationType
    {
        Game,
        Server
    }
}
