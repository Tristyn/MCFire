using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.Framework;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Models
{
    public class File : IFile
    {
        #region Fields

        [NotNull]
        IFolder _parent;
        [NotNull]
        protected FileInfo _info;

        #endregion

        #region Constructors

        public File([NotNull] IFolder parent, [NotNull] FileInfo info)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (info == null) throw new ArgumentNullException("info");

            if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("parent path != FileInfo path");

            _parent = parent;
            _info = info;
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

        public Task<bool> Rename(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task Refresh()
        {
            return Task.Run(() =>
            {
                _info.Refresh(); 
                OnFileRefreshed(new FileRefreshedEventArgs(this));
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

        protected virtual void OnFileRefreshed(FileRefreshedEventArgs e)
        {
            var handler = FileRefreshed;
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
            get { return _info.Name; }
            set { throw new NotImplementedException(); }
        }

        public string Path { get { return _info.FullName; } }

        public string Extension { get { return _info.Extension; } }

        public IFolder Parent { get { return _parent; } }

        public event EventHandler<FileRefreshedEventArgs> FileRefreshed;

        #endregion
    }
}
