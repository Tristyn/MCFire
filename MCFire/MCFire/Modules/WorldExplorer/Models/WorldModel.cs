using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class WorldModel : WorldBrowserItem
    {
        public WorldModel([NotNull] IFolder folder) : base(folder)
        {
        }
    }
}
