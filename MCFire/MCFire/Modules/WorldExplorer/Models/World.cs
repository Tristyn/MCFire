using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class World : WorldBrowserItem
    {
        public World([NotNull] IFolder folder) : base(folder)
        {
        }
    }
}
