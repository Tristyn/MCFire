using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.Services;

namespace MCFire.Modules.Files.Models
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

        /// <summary>
        /// Gets the stream associated with this file.
        /// </summary>
        /// <param name="stream">The file stream. Can be null.</param>
        /// <returns>If the stream was retrieved successfully.</returns>
        bool TryOpenRead([CanBeNull] out Stream stream);

        /// <summary>
        /// Returns the file stream for this file.
        /// </summary>
        /// <param name="mode">The file mode</param>
        /// <param name="access">The access mode</param>
        /// <param name="stream">The stream, can be null.</param>
        /// <returns>If the stream was successfully retrieved</returns>
        bool TryOpen(FileMode mode, FileAccess access, [CanBeNull] out Stream stream);
    }
}