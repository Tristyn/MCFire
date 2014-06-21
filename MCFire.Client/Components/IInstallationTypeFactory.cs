using JetBrains.Annotations;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Components
{
    public interface IInstallationTypeFactory
    {
        [CanBeNull]
        IInstallation Create([NotNull] string path);
    }
}
