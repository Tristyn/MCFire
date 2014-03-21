using MCFire.Modules.Files.Content;

namespace MCFire.Modules.Files.Events
{
    public class FileContentEventArgs : System.EventArgs
    {
        public IFileContent Content { get; private set; }

        public FileContentEventArgs(IFileContent content)
        {
            Content = content;
        }
    }
}
