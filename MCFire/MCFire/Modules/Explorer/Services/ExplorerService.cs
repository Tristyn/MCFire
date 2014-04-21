using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.Explorer.Services
{
    [Export]
    public class WorldExplorerService
    {
        readonly ObservableCollection<Installation> _installations = new ObservableCollection<Installation>();

        public WorldExplorerService()
        {
#if DEBUG && !FIRSTRUN
            // if debug, add game installation automatically
            if(Installations.Count!=0)
                return;
            var path =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")
                    .ToLower();
            var gameInstall = Installation.New(path);
            if(gameInstall!=null)
                Installations.Add(gameInstall);
#endif
        }

        /// <summary>
        /// Adds the installation if an installation with the same path doesn't already exist.
        /// </summary>
        /// <param name="install">The install.</param>
        /// <returns>If the install was added sucessfully</returns>
        public bool TryAddInstallation([CanBeNull] Installation install)
        {
            if (install == null) return false;

            if (Installations.Any(testInstall => install.Path.NormalizePath() == testInstall.Path.NormalizePath()))
                return false;

            Installations.Add(install);
            return true;
        }

        public ObservableCollection<Installation> Installations { get { return _installations; } }
    }
}