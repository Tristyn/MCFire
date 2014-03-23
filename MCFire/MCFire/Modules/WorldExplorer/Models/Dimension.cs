using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class Dimension : WorldBrowserItem
    {
        public Dimension([NotNull] IFolder folder) : base(folder)
        {
        }
    }
}
