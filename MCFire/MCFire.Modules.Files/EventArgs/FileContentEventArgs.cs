using MCFire.Modules.Files.Content;

namespace MCFire.Modules.Files.EventArgs
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
