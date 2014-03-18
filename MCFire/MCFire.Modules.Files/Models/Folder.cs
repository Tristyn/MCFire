using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.Events;

namespace MCFire.Modules.Files.Models
{
    public sealed class Folder : IFolder
    {
        //TODO: whenever a SecurityException is thrown, set a security bool to false
        #region Fields

        [CanBeNull]
        IFolder _parent;
        [CanBeNull]
        List<IFolder> _folders;
        [CanBeNull]
        List<IFile> _files;
        [CanBeNull]
        List<IFolderItem> _children;
        [NotNull]
        DirectoryInfo _info;
        [NotNull]
        readonly FileFactory _fileFactory;

        #endregion

        #region Constructor

        public Folder([CanBeNull] IFolder parent, [NotNull] string path, FileFactory fileFactory)
            : this(parent, new DirectoryInfo(path.ToLower()), fileFactory)
        {

        }

        public Folder([CanBeNull] IFolder parent, [NotNull] DirectoryInfo directory, [NotNull] FileFactory fileFactory)
        {
            if (directory == null) throw new ArgumentNullException("directory");
            if (fileFactory == null) throw new ArgumentNullException("fileFactory");

            _parent = parent;
            _info = directory;
            _fileFactory = fileFactory;
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

        public async Task Refresh()
        {
            // TODO: create a view centric refresh, where only items visible in the treeview are refreshed.
            _info.Refresh();

            // get the FileInfos that have been created since the last refresh.
            List<FileInfo> currentFiles;
            try
            {
                currentFiles = _info.Exists ? _info.EnumerateFiles().ToList() : new List<FileInfo>();
            }
            catch (DirectoryNotFoundException) { currentFiles = new List<FileInfo>(); }
            catch (SecurityException) { currentFiles = new List<FileInfo>(); }
            var oldFileNames = _files != null ? _files.Select(file => file.Name.ToLower()).ToList() : new List<string>();
            var newFiles = currentFiles
                .Where(file => currentFiles
                .Select(currentFile => currentFile.Name.ToLower())
                .Except(oldFileNames)
                .Contains(file.Name.ToLower())).ToList();

            // get the DirectoryInfos that have been created since the last refresh.
            List<DirectoryInfo> currentFolders;
            try
            {
                currentFolders = _info.Exists ? _info.EnumerateDirectories().ToList() : new List<DirectoryInfo>();
            }
            catch (DirectoryNotFoundException) { currentFolders = new List<DirectoryInfo>(); }
            catch (SecurityException) { currentFolders = new List<DirectoryInfo>(); }
            var oldFolderNames = _files != null ? _files.Select(file => file.Name.ToLower()).ToList() : new List<string>();
            var newFolders = currentFolders
                .Where(folder => currentFolders
                .Select(currentFolder => currentFolder.Name.ToLower())
                .Except(oldFolderNames)
                .Contains(folder.Name.ToLower())).ToList();

            // refresh old children
            await RefreshChildren();

            // create new files
            if (newFiles.Any())
            {
                if (_files == null)
                    _files = new List<IFile>();

                foreach (var fileInfo in newFiles)
                {
                    _files.Add(_fileFactory.GetFile(this, fileInfo));
                }
            }

            // create new folders
            if (newFolders.Any())
            {
                if (_folders == null)
                    _folders = new List<IFolder>();

                foreach (var folderInfo in newFolders)
                {
                    _folders.Add(new Folder(this, folderInfo, _fileFactory));
                }
            }

            OnRefreshed(new FolderItemRefreshedEventArgs(this));
        }

        private async Task RefreshChildren()
        {
            if (_files != null)
                foreach (var file in _files)
                {
                    await file.Refresh();
                }

            if (_folders != null)
                foreach (var folder in _folders)
                {
                    await folder.Refresh();
                }
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

        private void OnRefreshed(FolderItemRefreshedEventArgs e)
        {
            var handler = Refreshed;
            if (handler != null) handler(this, e);
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

        public IEnumerable<IFolder> Folders
        {
            get
            {
                if (_folders != null) return _folders;

                _folders = new List<IFolder>();
                foreach (var directoryInfo in _info.EnumerateDirectories())
                {
                    _folders.Add(new Folder(this, directoryInfo, _fileFactory));
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

        public IEnumerable<IFile> Files
        {
            get
            {
                if (_files != null) return _files;

                _files = new List<IFile>();
                foreach (var fileInfo in _info.EnumerateFiles())
                {
                    // add a originalFile to _files, using fileInfo and the shared originalFile factory
                    _files.Add(_fileFactory.GetFile(this, fileInfo));
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

        public IEnumerable<IFolderItem> Children
        {
            get
            {
                return _children ?? (_children = new List<IFolderItem>(Files.Concat<IFolderItem>(Folders)));
            }
        }

        public event EventHandler<FolderItemRefreshedEventArgs> Refreshed;

        #endregion
    }
}
