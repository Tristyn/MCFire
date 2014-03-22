using System;
using System.IO;
using MCFire.Modules.Files.Events;

namespace MCFire.Modules.Files.Content
{
    public interface IFileContent
    {
        void Save();
        bool Dirty { get; }

        /// <summary>
        /// If the content has invalid data during loading.
        /// </summary>
        bool ValidData { get; }

        event EventHandler<FileContentEventArgs> Dirtied;
        event EventHandler<FileContentEventArgs> Saved;
        // Todo: should be bool TrySave(), returns false if exception is thrown
        void Save(Stream stream);
        event EventHandler<FileContentEventArgs> InvalidData;
    }
}
