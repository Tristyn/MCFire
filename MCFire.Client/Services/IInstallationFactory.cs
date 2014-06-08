using JetBrains.Annotations;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Services
{
    public interface IInstallationFactory
    {
        [CanBeNull]
        IInstallation Create([CanBeNull]string path);
    }
}
