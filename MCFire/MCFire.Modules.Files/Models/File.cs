using System;
using System.IO;
using System.Security.AccessControl;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.Framework;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.Events;

namespace MCFire.Modules.Files.Models
{
    public class File : IFile
    {
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

        public Task<bool> Cut()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Copy()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete()
        {
            return _parent.DeleteFile(this);
        }

        /// <summary>
        /// Renames the file, leaving the extension unchanged.
        /// </summary>
        /// <param name="name">The new name, excluding extension.</param>
        /// <returns></returns>
        public async Task<bool> Rename(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            await Refresh();
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
        public Task<bool> RenameExtention(string name)
        {
            throw new NotImplementedException("Live change format not implemented yet.");
            //await Refresh();
            //if (!Info.Exists) return false;
            //if (Info.DirectoryName == null) throw new InvalidOperationException();
            //System.IO.File.Move(Info.FullName, System.IO.Path.Combine(Info.DirectoryName, name));

            //return true;
        }

        public virtual Task Refresh()
        {
            return Task.Run(() =>
            {
                Info.Refresh();
                OnRefreshed(new FolderItemRefreshedEventArgs(this));
            });
        }

        public virtual Task Open()
        {
            return _parent.OpenFile(this);
        }

        public Task ReplaceWith(IFormat format)
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

        #endregion

        #region Properties

        public bool Exists
        {
            get { return System.IO.File.Exists(Path); }
        }

        public string Name
        {
            get { return Info.Name; }
            set { throw new NotImplementedException(); }
        }

        public string Path { get { return Info.FullName; } }

        public string Extension { get { return Info.Extension; } }

        public IFolder Parent { get { return _parent; } }

        public event EventHandler<FolderItemRefreshedEventArgs> Refreshed;

        #endregion
    }
}
