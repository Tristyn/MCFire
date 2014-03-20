using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Messages
{
    public class FileOpenedEvent<TFile> where TFile : IFile
    {
        public TFile File { get; set; }

        public FileOpenedEvent(TFile file)
        {
            File = file;
        }
    }
}
