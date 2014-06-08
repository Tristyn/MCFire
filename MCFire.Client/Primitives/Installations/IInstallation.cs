using System.Collections.ObjectModel;
using MCFire.Common;

namespace MCFire.Client.Primitives.Installations
{
    public interface IInstallation
    {
        ObservableCollection<World> Worlds { get; }
        string Title { get; }
    }
}