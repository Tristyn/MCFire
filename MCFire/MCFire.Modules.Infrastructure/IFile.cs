using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure
{
    public interface IFile : IFolderItem
    {
        #region Methods

        [NotNull]
        Task OpenAsync();

        [NotNull]
        Task ReplaceWithAsync([NotNull] IFormat format);

        /// <summary>
        /// Renames the extension, and swaps out this file with a file that is assigned to that extension.
        /// </summary>
        /// <param name="name">The extension</param>
        /// <returns></returns>
        Task<bool> RenameExtentionAsync(string name);

        #endregion

        #region Properties

        string Extension { get; }
        IFolder Parent { get; }

        #endregion
    }
}