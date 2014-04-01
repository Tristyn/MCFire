using System;
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

        public IFolder Folder { get; private set; }
    }
}
