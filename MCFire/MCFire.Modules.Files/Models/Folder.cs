using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Models
{
    public sealed class Folder : IFolder
    {
        #region Fields

        [CanBeNull]
        IFolder _parent;
        [CanBeNull]
        List<IFolder> _folders;
        [CanBeNull]
        List<IFile> _files;
        [CanBeNull] List<IFolderItem> _children;
        [NotNull]
        DirectoryInfo _info;


        #endregion

        #region Constructor

        public Folder([CanBeNull] Folder parent, [NotNull] string path)
            : this(parent, new DirectoryInfo(path.ToLower()))
        {

        }

        public Folder([CanBeNull] Folder parent, [NotNull] DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException("directory");

            _parent = parent;
            _info = directory;
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
            throw new NotImplementedException();
        }

        public Task<bool> Rename(string name)
        {
            throw new NotImplementedException();
        }

        public Task Refresh()
        {
            return Task.Run(() => _info.Refresh());
        }

        public Task<bool> CreateFile(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFile(IFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PasteFile(IFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PasteFolder(IFolder folder)
        {
            throw new NotImplementedException();
        }

        public Task OpenFile(IFile file)
        {
            //if (!_files.Contains(originalFile));
            throw new NotImplementedException();

        }

        public Task<bool> ReplaceFileWith(IFile originalFile, IFile newFile)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Properties

        public bool Exists
        {
            get { return _info.Exists; }
        }

        public string Name
        {
            get { return _info.Name; }
            set { throw new NotImplementedException(); }
        }

        public string Path
        {
            get { return _info.FullName; }
        }

        public bool Empty
        {
            get { return Folders.Any() || Files.Any(); }
        }

        [NotNull]
        public IEnumerable<IFolder> Folders
        {
            get
            {
                if (_folders != null) return _folders;

                _folders = new List<IFolder>();
                foreach (var directoryInfo in _info.EnumerateDirectories())
                {
                    _folders.Add(new Folder(this, directoryInfo));
                }

                return _folders;
            }
        }

        public IEnumerable<IFolder> AllFolders
        {
            get
            {
                return Folders.Aggregate(Folders, (current, folder) => current.Concat(folder.AllFolders));
            }
        }

        [NotNull]
        public IEnumerable<IFile> Files
        {
            get
            {
                if (_files != null) return _files;

                _files = new List<IFile>();
                foreach (var fileInfo in _info.EnumerateFiles())
                {
                    // add a originalFile to _files, using fileInfo and the shared originalFile factory
                    throw new NotImplementedException();
                }
                return _files;

            }
        }

        public IEnumerable<IFile> AllFiles
        {
            get
            {
                return Folders.Aggregate(Files, (current, folder) => current.Concat(folder.AllFiles));
            }
        }

        [NotNull]
        public IEnumerable<IFolderItem> Children
        {
            get
            {
                // TODO: when files are implemented properly, uncomment the below line and use that.
                // return _children ?? (_children = new List<IFolderItem>(Files.Concat<IFolderItem>(Folders)));
                return _children ?? (_children = new List<IFolderItem>(Folders));
            }
        }

        #endregion
    }
}
