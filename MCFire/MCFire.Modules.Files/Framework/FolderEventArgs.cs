using System;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Framework
{
    public class FolderEventArgs : EventArgs
    {
        public FolderEventArgs([NotNull] Folder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            Folder = folder;
        }

        public Folder Folder { get; private set; }
    }
}
