using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Messages
{
    public class FileOpenedMessage<TFile> where TFile : IFile
    {
        public TFile File { get; set; }

        public FileOpenedMessage(TFile file)
        {
            File = file;
        }
    }
}
