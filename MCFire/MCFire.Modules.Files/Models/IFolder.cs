using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MCFire.Modules.Files.Models
{
    public interface IFolder : IFolderItem
    {
        #region methods

        Task<bool> CreateFileAsync(string name);
        Task<bool> DeleteFileAsync(IFile file);
        Task<bool> PasteFileAsync(IFile file);
        Task<bool> PasteFolderAsync(IFolder folder);
        Task OpenFileAsync(IFile file);
        Task<bool> ReplaceFileWithAsync(IFile originalFile, IFile newFile);

        #endregion

        #region Properties

        bool Empty { get; }

        [NotNull]
        IEnumerable<IFolder> Folders { get; }

        IEnumerable<IFolder> AllFolders { get; }

        [NotNull]
        IEnumerable<IFile> Files { get; }

        IEnumerable<IFile> AllFiles { get; }

        [NotNull]
        IEnumerable<IFolderItem> Children { get; }

        #endregion
    }
}