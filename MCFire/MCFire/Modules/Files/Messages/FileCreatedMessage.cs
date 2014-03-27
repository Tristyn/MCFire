using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Messages
{
    public class FileCreatedMessage
    {
        public IFile File { get; set; }

        public FileCreatedMessage(IFile file)
        {
            File = file;
        }
    }
}
