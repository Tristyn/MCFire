using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
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
        Task<bool> ReplaceFileWithAsync(IFile originalFile, IFile newFile);

        #endregion

        #region Properties

        bool Empty { get; }

        [NotNull]
        BindableCollection<IFolder> Folders { get; }

        [NotNull]
        BindableCollection<IFile> Files { get; }

        [NotNull]
        BindableCollection<IFolderItem> Children { get; }

        #endregion
    }
}