namespace MCFire.Modules.Files.Events
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
