using System.Collections.ObjectModel;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public class World : WorldBrowserItem
    {
        public World([NotNull] IFolder folder)
            : base(folder)
        {
            Children = new ObservableCollection<WorldBrowserItem>();
        }

        public override sealed ObservableCollection<WorldBrowserItem> Children { get; protected set; }
    }
}
