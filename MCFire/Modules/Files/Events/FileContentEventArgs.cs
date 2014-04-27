using MCFire.Modules.Files.Content;

namespace MCFire.Modules.Files.Events
{
    public class FileContentEventArgs : System.EventArgs
    {
        public FileContent Content { get; private set; }

        public FileContentEventArgs(FileContent content)
        {
            Content = content;
        }
    }
}
