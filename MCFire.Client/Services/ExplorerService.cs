using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using MCFire.Client.Primitives.Installations;
using MCFire.Common.Infrastructure.Extensions;

namespace MCFire.Client.Services
{
    [Export(typeof(IWorldExplorerService))]
    public class WorldWorldExplorerService : IWorldExplorerService
    {
        readonly ObservableCollection<IInstallation> _installations = new ObservableCollection<IInstallation>();
        // TODO: the app shouldn't rely on if an MCFireWorld is found in WorldExporerService, it should:
        // TODO: 1) discover worlds found in an installation folder for use by the rest of the app
        // TODO: 2) save MCFireWorlds on shutdown
        // TODO: 3) MCFireWorlds should operate without being added to the service (operate after being removed too)
        // TODO: 4) MCFireWorlds should have clear guarantees when 2 operate on the same world (or when Minecraft open)

        public WorldWorldExplorerService()
        {
            // TODO: FIRSTRUN after new assemblies
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

        // TODO: This service should create the Installation itself
        /// <summary>
        /// Adds the installation if an installation with the same path doesn't already exist.
        /// </summary>
        /// <param name="install">The install.</param>
        /// <returns>If the install was added sucessfully</returns>
        public bool TryAddInstallation(IInstallation install)
        {
            if (install == null) return false;

            if (Installations.Any(testInstall => install.Path.NormalizePath() == testInstall.Path.NormalizePath()))
                return false;

            Installations.Add(install);
            return true;
        }

        public ObservableCollection<IInstallation> Installations { get { return _installations; } }
    }
}