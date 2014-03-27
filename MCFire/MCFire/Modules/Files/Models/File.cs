using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.Events;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Services;

namespace MCFire.Modules.Files.Models
{
    public class File : IFile
    {
        // TODO: whenever a SecurityException is thrown, set a security bool to false
        #region Fields

        [NotNull]
        IFolder _parent;
        [NotNull]
        protected FileInfo Info;
        [CanBeNull]
        WeakReference<FileContent> _weakContentReference;
        [CanBeNull]
        FileContent _strongContentReference;
        readonly object _lock = new object();

        #endregion

        #region Constructors

        // TODO: parent should be able to be null, passing a string instead of info
        public File([NotNull] IFolder parent, [NotNull] FileInfo info)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (info == null) throw new ArgumentNullException("info");

            if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("parent path must equal FileInfo path");
            // ReSharper disable once UnusedVariable - Testing if the property was set properly
            var directoryAssignedTest = info.DirectoryName;
            _parent = parent;
            Info = info;
        }

        #endregion

        #region Methods

        public virtual Task<bool> CutAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> CopyAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> DeleteAsync()
        {
            return _parent.DeleteFileAsync(this);
        }

        public virtual Task ReplaceWithAsync(IFormat format)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renames the file, leaving the extension unchanged.
        /// </summary>
        /// <param name="name">The new name, excluding extension.</param>
        /// <returns></returns>
        public virtual bool Rename(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            Refresh();
            lock (_lock)
            {
                if (!Info.Exists) return false;
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute - Directory wont be null because it is asserted during construction
                    System.IO.File.Move(Info.FullName, System.IO.Path.Combine(Info.DirectoryName, name + Extension));
                    return true;
                }
                catch (ArgumentException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (NotSupportedException) { }

                return false;
            }
        }

        /// <summary>
        /// Renames the file, leaving the extension unchanged.
        /// </summary>
        /// <param name="name">The new name, excluding extension.</param>
        /// <returns></returns>
        public virtual async Task<bool> RenameAsync(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            await RefreshAsync();
            lock (_lock)
            {
                if (!Info.Exists) return false;
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute - Directory wont be null because it is asserted during construction
                    System.IO.File.Move(Info.FullName, System.IO.Path.Combine(Info.DirectoryName, name + Extension));
                    return true;
                }
                catch (ArgumentException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (NotSupportedException) { }

                return false;
            }
        }

        /// <summary>
        /// Renames the extension, and swaps out this file with a file that is assigned to that extension.
        /// </summary>
        /// <param name="name">The extension</param>
        /// <returns></returns>
        public virtual Task<bool> RenameExtentionAsync(string name)
        {
            throw new NotImplementedException("Live change format not implemented yet.");
        }

        public virtual void Refresh()
        {
            var oldExists = Info.Exists;
            var oldName = Info.Name;

            Info.Refresh();

            if (oldExists != Info.Exists)
                OnExistsChanged(new FolderItemExistsChangedEventArgs(Info.Exists));
            if (oldName != Info.Name)
                OnNameChanged(new FolderItemNameChangedEventArgs(Info.Name));

            OnRefreshed(new FolderItemRefreshedEventArgs(this));
        }

        public virtual Task RefreshAsync()
        {
            return Task.Run(() => Refresh());
        }

        /// <summary>
        /// Returns the file stream for this file, catching any exception.
        /// </summary>
        /// <param name="mode">The file mode</param>
        /// <param name="access">The access mode</param>
        /// <param name="stream">The stream, can be null.</param>
        /// <returns>If the stream was successfully retrieved</returns>
        public bool TryOpen(FileMode mode, FileAccess access, out Stream stream)
        {
            stream = null;
            try
            {
                Info.Create();
                stream = Info.Open(mode, access);
            }
            catch (SecurityException) { }
            catch (IOException) { }
            catch (ArgumentException) { }
            catch (UnauthorizedAccessException) { }
            return stream != null;
        }

        /// <summary>
        /// Gets the stream associated with this file.
        /// </summary>
        /// <param name="stream">The file stream. Can be null.</param>
        /// <returns>If the stream was retrieved successfully.</returns>
        public bool TryOpenRead(out Stream stream)
        {
            stream = null;
            try
            {
                stream = Info.OpenRead();
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
            return stream != null;
        }

        /// <summary>
        /// Gets a writable stream for this file, with the FileMode.Truncate setting.
        /// </summary>
        /// <param name="stream">The file stream. Can be null.</param>
        /// <returns>If the file stream was returned successfully.</returns>
        public bool TryOpenWrite(out Stream stream)
        {
            try
            {
                Info.Create();
                stream = Info.Open(FileMode.Truncate, FileAccess.Write);
                return true;
            }
            catch (SecurityException) { }
            catch (ArgumentException) { }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            stream = null;
            return false;
            
        }

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
        public bool TryOpenContent<TContent>(out TContent content)
            where TContent : FileContent, new()
        {
            lock (_lock)
            {
                // if content already set
                FileContent existingContent;
                if (_weakContentReference != null && _weakContentReference.TryGetTarget(out existingContent))
                {
                    // check if set type is instance or derived.
                    if (existingContent is TContent)
                    {
                        content = existingContent as TContent;
                        return true;
                    }
                    content = null;
                    return false;
                }

                // create new content, return it
                content = new TContent();
                Stream stream;
                if (TryOpenRead(out stream) && content.Load(stream))
                {
                    // reading successful, set and return new instance
                    if (_weakContentReference == null)
                        _weakContentReference = new WeakReference<FileContent>(content);
                    else
                        _weakContentReference.SetTarget(content);
                    content.Dirtied += OnContentDirtied;
                    content.Saved += OnContentSaved;
                    return true;
                }

                // content loading failed, return false
                content = null;
                return false;
            }
        }

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
        public bool TryForceContent<TContent>(out TContent content)
            where TContent : FileContent, new()
        {
            lock (_lock)
            {
                // get content incase TryOpenContent fails and GC collects inbetween
                FileContent oldContent = null;
                if (_weakContentReference != null)
                    _weakContentReference.TryGetTarget(out oldContent);

                // try this, override content if it fails
                if (TryOpenContent(out content))
                    return true;

                // save and dispose content
                Stream stream;
                if (oldContent != null && TryOpenWrite(out stream))
                {
                    oldContent.Save(stream);
                    oldContent.Dispose();
                }

                // create new content, note: if TryOpenWrite fails, changes will be lost
                _weakContentReference = null;
                _strongContentReference = null;
                return TryOpenContent(out content);
            }
        }

        /// <summary>
        /// Gets a strong reference to the content, so it can be saved later.
        /// </summary>
        protected virtual void OnContentDirtied(object sender, FileContentEventArgs e)
        {
            lock (_lock)
            {
                FileContent content = null;
                if (_weakContentReference != null && !_weakContentReference.TryGetTarget(out content)) return;
                if (e.Content != content) return; // This is a ghost event from previously set content. ignore it
                _strongContentReference = content;
            }
        }

        /// <summary>
        /// Saves the content, then removes our strong reference to it.
        /// </summary>
        protected virtual void OnContentSaved(object sender, FileContentEventArgs e)
        {
            lock (_lock)
            {
                if (_strongContentReference != null && _strongContentReference != e.Content)
                    return;

                Stream stream;
                if (TryOpenWrite(out stream))
                    e.Content.Save(stream);

                _strongContentReference = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        protected virtual void OnRefreshed(FolderItemRefreshedEventArgs e)
        {
            var handler = Refreshed;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnExistsChanged(FolderItemExistsChangedEventArgs e)
        {
            EventHandler<FolderItemExistsChangedEventArgs> handler = ExistsChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnNameChanged(FolderItemNameChangedEventArgs e)
        {
            EventHandler<FolderItemNameChangedEventArgs> handler = NameChanged;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Properties

        public bool Exists
        {
            get { return System.IO.File.Exists(Path); }
        }

        public string Name
        {
            get { return Info.Name; }
            set { Rename(value); }
        }

        public virtual string Path { get { return Info.FullName; } }

        public virtual string Extension { get { return Info.Extension; } }

        public virtual IFolder Parent { get { return _parent; } }

        public event EventHandler<FolderItemRefreshedEventArgs> Refreshed;
        public event EventHandler<FolderItemExistsChangedEventArgs> ExistsChanged;
        public event EventHandler<FolderItemNameChangedEventArgs> NameChanged;

        #endregion
    }
}
