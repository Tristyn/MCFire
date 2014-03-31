using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Nbt.Models;

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

        public LevelContent Level
        {
            get
            {
                var level = Folder.Files.FirstOrDefault(file => file.Name.ToLower() == "level.dat");
                if (level == null) 
                    return null;

                LevelContent content;
                return level.TryOpenContent(out content) ? content : null;
            }
        }
    }
}
