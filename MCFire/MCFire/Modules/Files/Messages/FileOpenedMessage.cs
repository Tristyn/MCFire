using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Messages
{
    public class FileOpenedMessage<TContent> where TContent : FileContent
    {
        public IFile File { get; set; }
        public TContent Content { get; set; }

        public FileOpenedMessage(IFile file, TContent content)
        {
            File = file;
            Content = content;
        }
    }
}
