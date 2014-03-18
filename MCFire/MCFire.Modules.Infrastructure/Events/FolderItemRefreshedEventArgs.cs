namespace MCFire.Modules.Infrastructure.Events
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
