using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Events
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
