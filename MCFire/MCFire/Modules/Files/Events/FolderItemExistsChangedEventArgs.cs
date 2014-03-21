namespace MCFire.Modules.Files.Events
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
