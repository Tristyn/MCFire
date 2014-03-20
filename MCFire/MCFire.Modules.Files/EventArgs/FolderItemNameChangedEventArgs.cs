namespace MCFire.Modules.Files.EventArgs
{
    public class FolderItemNameChangedEventArgs : System.EventArgs
    {
        public FolderItemNameChangedEventArgs(string newName)
        {
            NewName = newName;
        }

        public string NewName { get; set; }
    }
}
