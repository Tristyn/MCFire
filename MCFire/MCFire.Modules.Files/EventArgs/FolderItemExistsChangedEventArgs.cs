namespace MCFire.Modules.Files.EventArgs
{
    public class FolderItemExistsChangedEventArgs:System.EventArgs
    {
        public FolderItemExistsChangedEventArgs(bool newState)
        {
            NewState = newState;
        }

        public bool NewState { get; set; }
    }
}
