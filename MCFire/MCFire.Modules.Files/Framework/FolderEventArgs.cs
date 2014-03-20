using System;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Framework
{
    public class FolderEventArgs : System.EventArgs
    {
        public FolderEventArgs([NotNull] IFolder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");
            Folder = folder;
        }

        public IFolder Folder { get; private set; }
    }
}
