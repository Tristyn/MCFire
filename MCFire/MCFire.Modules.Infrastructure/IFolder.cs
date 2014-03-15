using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure
{
    public interface IFolder : IFolderItem
    {
        Task<bool> CreateFile(string name);
        Task<bool> DeleteFile(IFile file);
        Task<bool> PasteFile(IFile file);
        Task<bool> PasteFolder(IFolder folder);
        Task OpenFile(IFile file);
        Task<bool> ReplaceFileWith(IFile originalFile, IFile newFile);
        bool Empty { get; }

        [NotNull]
        IEnumerable<IFolder> Folders { get; }

        IEnumerable<IFolder> AllFolders { get; }

        [NotNull]
        IEnumerable<IFile> Files { get; }

        IEnumerable<IFile> AllFiles { get; }

        [NotNull]
        IEnumerable<IFolderItem> Children { get; }
    }
}