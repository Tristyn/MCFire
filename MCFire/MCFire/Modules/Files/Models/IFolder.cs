using System.Collections.ObjectModel;
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
        Task<bool> ReplaceFileWithAsync(IFile originalFile, IFile newFile);

        #endregion

        #region Properties

        bool Empty { get; }

        [NotNull]
        ObservableCollection<IFolder> Folders { get; }

        [NotNull]
        ObservableCollection<IFile> Files { get; }

        [NotNull]
        ObservableCollection<IFolderItem> Children { get; }

        #endregion
    }
}