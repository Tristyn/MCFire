using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.EventArgs;
using MCFire.Modules.Files.Services;

namespace MCFire.Modules.Files.Models
{
    public class File : IFile
    {
        //TODO: whenever a SecurityException is thrown, set a security bool to false
        #region Fields

        [NotNull]
        IFolder _parent;
        [NotNull]
        protected FileInfo Info;
        readonly object _lock = new object();

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

        public virtual Task OpenAsync()
        {
            return _parent.OpenFileAsync(this);
        }

        public virtual Task ReplaceWithAsync(IFormat format)
        {
            throw new NotImplementedException();
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
