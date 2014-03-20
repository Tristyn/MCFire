using System;

namespace MCFire.Modules.Infrastructure.Events
{
    public class FolderItemNameChangedEventArgs : EventArgs
    {
        public FolderItemNameChangedEventArgs(string newName)
        {
            NewName = newName;
        }

        public string NewName { get; set; }
    }
}
