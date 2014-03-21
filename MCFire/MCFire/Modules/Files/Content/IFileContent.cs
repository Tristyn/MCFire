using System;
using System.IO;
using MCFire.Modules.Files.Events;

namespace MCFire.Modules.Files.Content
{
    public interface IFileContent
    {
        void Save();
        bool Dirty { get; }
        event EventHandler<FileContentEventArgs> Dirtied;
        event EventHandler<FileContentEventArgs> Saved;
        void Save(Stream stream);
    }
}
