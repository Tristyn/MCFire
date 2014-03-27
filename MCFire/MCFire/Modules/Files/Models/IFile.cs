using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Services;

namespace MCFire.Modules.Files.Models
{
    public interface IFile : IFolderItem
    {
        #region Methods

        // TODO: changes to files and content have made this not work anymore, and the file explorer uses this method to open files.
        //[NotNull]
        //Task OpenAsync();

        [NotNull]
        Task ReplaceWithAsync([NotNull] IFormat format);

        /// <summary>
        /// Renames the extension, and swaps out this file with a file that is assigned to that extension.
        /// </summary>
        /// <param name="name">The extension</param>
        /// <returns></returns>
        Task<bool> RenameExtentionAsync(string name);

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

        /// <summary>
        /// Gets a writable stream for this file, with the FileMode.Truncate setting.
        /// </summary>
        /// <param name="stream">The file stream. Can be null.</param>
        /// <returns>If the file stream was returned successfully.</returns>
        bool TryOpenWrite(out Stream stream);

        #endregion

        #region Properties

        string Extension { get; }
        IFolder Parent { get; }

        #endregion

        /// <summary>
        /// Sets this file to read its content as the specified type.
        /// The file will manage the saving of content, though you can still call Save().
        /// If the content has already been set, it will return that instance if the type is assignable.
        /// Use ForceContent if you want to force a file to change content types.
        /// </summary>
        /// <typeparam name="TContent">The type of content to create.</typeparam>
        /// <param name="content">The instance of content to return, can be null.</param>
        /// <returns>
        /// False if set type and requested type is incompatible, 
        /// or an exception was raised while loading content from disk.
        /// </returns>
        bool TryOpenContent<TContent>(out TContent content)
            where TContent : FileContent, new();

        /// <summary>
        /// Sets this file to read its content as the specified type.
        /// The file will manage the saving of content, though you can still call Save().
        /// If the content has already been set, it will return that instance if the type is assignable.
        /// Use ForceContent if you want to force a file to change content types.
        /// </summary>
        /// <typeparam name="TContent">The type of content to create.</typeparam>
        /// <param name="content">The instance of content to return, can be null.</param>
        /// <returns>
        /// False if set type and requested type is incompatible, 
        /// or an exception was raised while loading content from disk.
        /// </returns>
        bool TryForceContent<TContent>(out TContent content)
            where TContent : FileContent, new();
    }
}