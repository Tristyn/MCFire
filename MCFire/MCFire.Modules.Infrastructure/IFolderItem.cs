using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.Events;

namespace MCFire.Modules.Infrastructure
{
    public interface IFolderItem
    {
        #region Methods

        Task<bool> CutAsync();
        Task<bool> CopyAsync();
        Task<bool> DeleteAsync();

        /// <summary>
        /// Renames the file, leaving the extension unchanged.
        /// </summary>
        /// <param name="name">The new name, excluding extension.</param>
        /// <returns></returns>
        bool Rename(string name);

        Task<bool> RenameAsync([NotNull] string name);
        void Refresh();
        Task RefreshAsync();

        #endregion

        #region Properties

        bool Exists { get; }
        [NotNull]
        string Name { get; set; }
        [NotNull]
        string Path { get; }
        event EventHandler<FolderItemRefreshedEventArgs> Refreshed;
        event EventHandler<FolderItemExistsChangedEventArgs> ExistsChanged;
        event EventHandler<FolderItemNameChangedEventArgs> NameChanged;

        #endregion
    }
}
