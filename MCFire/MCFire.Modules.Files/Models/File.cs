using System;
using System.IO;
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

        #endregion

        #region Constructors

        public File([NotNull] IFolder parent, [NotNull] FileInfo info)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (info == null) throw new ArgumentNullException("info");

            if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("parent path != FileInfo path");

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

        public Task<bool> Rename(string name)
        {
            throw new NotImplementedException();
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
