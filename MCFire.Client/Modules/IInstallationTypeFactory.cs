using JetBrains.Annotations;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Modules
{
    public interface IInstallationTypeFactory
    {
        [CanBeNull]
        IInstallation Create([NotNull] string path);
    }
}
