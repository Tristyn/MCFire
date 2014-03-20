using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Messages
{
    public class FileCreatedEvent<TFile> where TFile : IFile
    {
        public TFile File { get; set; }

        public FileCreatedEvent(TFile file)
        {
            File = file;
        }
    }
}
