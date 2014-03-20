using System;
using MCFire.Modules.Files.EventArgs;

namespace MCFire.Modules.Files.Content
{
    public interface IFileContent
    {
        void Save();
        bool Dirty { get; }
        event EventHandler<FileContentEventArgs> Dirtied;
        event EventHandler<FileContentEventArgs> Saved;
    }
}
