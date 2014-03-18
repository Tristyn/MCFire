using System;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Framework
{
    public class FolderRefreshedEventArgs : EventArgs
    {
        public FolderRefreshedEventArgs(Folder folder)
        {
            Folder = folder;
        }
        public Folder Folder { get; private set; }
    }
}
