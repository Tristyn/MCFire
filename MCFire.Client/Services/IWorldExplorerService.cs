using System.Collections.ObjectModel;
using JetBrains.Annotations;
using MCFire.Client.Primitives.Installations;
using MCFire.Client.Services.Explorer;

namespace MCFire.Client.Services
{
    public interface IWorldExplorerService
    {
        /// <summary>
        /// Adds the installation if an installation with the same path doesn't already exist.
        /// </summary>
        /// <param name="install">The install.</param>
        /// <returns>If the install was added sucessfully</returns>
        bool TryAddInstallation([CanBeNull] Installation install);

        ObservableCollection<Installation> Installations { get; }
    }
}
