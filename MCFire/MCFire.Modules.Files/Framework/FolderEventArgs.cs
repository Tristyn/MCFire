using System;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Framework
{
    public class FolderEventArgs : EventArgs
    {
        public FolderEventArgs([NotNull] IFolder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            Folder = folder;
        }

        public IFolder Folder { get; private set; }
    }
}
