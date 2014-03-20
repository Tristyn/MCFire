using System;

namespace MCFire.Modules.Infrastructure.Events
{
    public class FolderItemExistsChangedEventArgs:EventArgs
    {
        public FolderItemExistsChangedEventArgs(bool newState)
        {
            NewState = newState;
        }

        public bool NewState { get; set; }
    }
}
