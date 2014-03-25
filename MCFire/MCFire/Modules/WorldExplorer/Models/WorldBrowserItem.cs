using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.WorldExplorer.Models
{
    public abstract class WorldBrowserItem
    {
        protected WorldBrowserItem([NotNull] IFolder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            Folder = folder;
        }

        public abstract ObservableCollection<WorldBrowserItem> Children { get; protected set; }

        public IFolder Folder { get; private set; }

        public virtual string Title
        {
            get { return Folder.Name; }
        }
    }
}
