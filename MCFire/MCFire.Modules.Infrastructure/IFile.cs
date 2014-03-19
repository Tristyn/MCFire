using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure
{
    public interface IFile : IFolderItem
    {
        #region Methods

        [NotNull]
        Task Open();

        [NotNull]
        Task ReplaceWith([NotNull] IFormat format);

        #endregion

        #region Properties

        string Extension { get; }
        IFolder Parent { get; }

        #endregion

        /// <summary>
        /// Renames the extension, and swaps out this file with a file that is assigned to that extension.
        /// </summary>
        /// <param name="name">The extension</param>
        /// <returns></returns>
        Task<bool> RenameExtention(string name);
    }
}