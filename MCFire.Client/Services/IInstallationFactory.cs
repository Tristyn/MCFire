using JetBrains.Annotations;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Services
{
    public interface IInstallationFactory
    {
        // TODO: world discovery system (Installations) needs to be moved to Core/Common
        [CanBeNull]
        IInstallation Create([CanBeNull]string path);
    }
}
