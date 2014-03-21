using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Messages
{
    public class FileCreatedMessage<TFile> where TFile : IFile
    {
        public TFile File { get; set; }

        public FileCreatedMessage(TFile file)
        {
            File = file;
        }
    }
}
