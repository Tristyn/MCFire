﻿using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Files.Content;
using MCFire.Modules.Files.EventArgs;
using MCFire.Modules.Files.Messages;
using MCFire.Modules.Files.Services;

namespace MCFire.Modules.Files.Models
{
    public class File : IFile
    {
        // TODO: whenever a SecurityException is thrown, set a security bool to false

        // TODO: make FileContent implementation cleaner/less oportunity for inheritors to mess it 
        // TODO: up by implementing a generic or overridable methods
        #region Fields

        [NotNull]
        IFolder _parent;
        [NotNull]
        protected FileInfo Info;    

        private IFileContent _content;
        private WeakReference<IFileContent> _weakContentReference;
        protected readonly object Lock = new object();

        #endregion

        #region Constructors

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

        /// <summary>
        /// Renames the file, leaving the extension unchanged.
        /// </summary>
        /// <param name="name">The new name, excluding extension.</param>
        /// <returns></returns>
        public virtual bool Rename(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            Refresh();
            lock (Lock)
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
            lock (Lock)
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
        /// Notifies all ViewModels that are listening for this type of format that they should open its contents. 
        /// This method is generally invoked by a user interface.
        /// </summary>
        /// <returns></returns>
        public virtual Task OpenAsync()
        {
            return Task.Run(() => IoC.Get<IEventAggregator>().Publish(new FileOpenedMessage<File>(this)));
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
        /// Returns the file stream for this file.
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
                stream = Info.Open(mode, access);
            }
            catch (SecurityException) { }
            catch (IOException) { }
            catch (ArgumentException) { }
            catch (UnauthorizedAccessException) { }
            return stream != null;
        }

        public virtual Task ReplaceWithAsync(IFormat format)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }

        protected void OnContentDirtied(object sender, FileContentEventArgs e)
        {
            lock (Lock)
            {
                IFileContent content;
                if (!_weakContentReference.TryGetTarget(out content)) return; // only way this can happen is if the finalizer calls Dirty=false
                if (e.Content != content)
                    throw new InvalidOperationException("FileContentEventArgs.Content != _weakContentReference.target");
                _content = content;
                content.Dirtied -= OnContentDirtied;
                content.Saved += OnContentSaved;
            }
        }

        private void OnContentSaved(object sender, FileContentEventArgs e)
        {
            lock (Lock)
            {
                if (e.Content != _content)
                    throw new InvalidOperationException("FileContentEventArgs.Content != _content");
                _content.Dirtied += OnContentDirtied;
                _content.Saved -= OnContentSaved;
                _content = null;
            }
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
