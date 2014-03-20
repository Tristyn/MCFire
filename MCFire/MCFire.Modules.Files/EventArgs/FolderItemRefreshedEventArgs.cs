using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.EventArgs
{
    public class FolderItemRefreshedEventArgs : System.EventArgs
    {
        public FolderItemRefreshedEventArgs(IFolderItem folderItem)
        {
            FolderItem = folderItem;
        }

        public IFolderItem FolderItem { get; set; }
    }
}
