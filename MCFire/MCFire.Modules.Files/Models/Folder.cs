using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MCFire.Modules.Files.EventArgs;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure;

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
        readonly object _lock = new object();

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

        public Task<bool> CutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CopyAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public bool Rename(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            lock (_lock)
            {
                Refresh();
                if (!_info.Exists) return false;
                try
                {
                    var parentFolderPath = _info.FullName.FindReplaceLastOccurance(_info.Name, "");
                    System.IO.File.Move(_info.FullName, System.IO.Path.Combine(parentFolderPath, name));
                    return true;
                }
                catch (ArgumentException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (NotSupportedException) { }

                return false;
            }
        }

        public Task<bool> RenameAsync(string name)
        {
            return Task.Run(() => Rename(name));
        }

        /// <summary>
        /// Refreshes this objects state, ignoring child FolderItems.
        /// </summary>
        public void Refresh()
        {
            var oldExists = _info.Exists;
            var oldName = _info.Name;

            _info.Refresh();

            if(oldExists != _info.Exists)
                OnExistsChanged(new FolderItemExistsChangedEventArgs(_info.Exists));
            if(oldName!=_info.Name)
                OnNameChanged(new FolderItemNameChangedEventArgs(_info.Name));
            OnRefreshed(new FolderItemRefreshedEventArgs(this));
        }

        /// <summary>
        /// Refreshes this objects state and the states of its children.
        /// </summary>
        public async Task RefreshAsync()
        {
            // TODO: create a view centric refresh, where only items visible in the treeview are refreshed.
            Refresh();

            // get the FileInfos and DirectoryInfos that have been created since the last refresh.

            var newFiles = _files != null ? GetNewFiles() : new List<FileInfo>();
            var newFolders = _folders != null ? GetNewDirectories() : new List<DirectoryInfo>();

            // refresh old children
            await RefreshChildren();

            // create new files
            if (newFiles.Any())
            {
                if (_files == null)
                    _files = new List<IFile>();

                foreach (var fileInfo in newFiles)
                {
                    _files.Add(_fileFactory.CreateFile(this, fileInfo));
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

        private List<FileInfo> GetNewFiles()
        {
            List<FileInfo> currentFiles;
            try
            {
                currentFiles = _info.Exists ? _info.EnumerateFiles().ToList() : new List<FileInfo>();
            }
            catch (DirectoryNotFoundException) { currentFiles = new List<FileInfo>(); }
            catch (SecurityException) { currentFiles = new List<FileInfo>(); }
            var oldFileNames = _files != null ? _files.Select(file => file.Name.ToLower()).ToList() : new List<string>();
            return currentFiles
                   .Where(file => currentFiles
                   .Select(currentFile => currentFile.Name.ToLower())
                   .Except(oldFileNames)
                   .Contains(file.Name.ToLower())).ToList();
        }

        private List<DirectoryInfo> GetNewDirectories()
        {
            List<DirectoryInfo> currentFolders;
            try
            {
                currentFolders = _info.Exists ? _info.EnumerateDirectories().ToList() : new List<DirectoryInfo>();
            }
            catch (DirectoryNotFoundException) { currentFolders = new List<DirectoryInfo>(); }
            catch (SecurityException) { currentFolders = new List<DirectoryInfo>(); }
            var oldFolderNames = _folders != null ? _folders.Select(folder => folder.Name.ToLower()).ToList() : new List<string>();
            return currentFolders
                   .Where(folder => currentFolders
                   .Select(currentFolder => currentFolder.Name.ToLower())
                   .Except(oldFolderNames)
                   .Contains(folder.Name.ToLower())).ToList();
        }

        private async Task RefreshChildren()
        {
            var tasks = new List<Task>();

            if (_files != null)
                tasks.AddRange(_files.Select(file => file.RefreshAsync()));
            if (_folders != null)
                tasks.AddRange(_folders.Select(folder => folder.RefreshAsync()));

            await Task.WhenAll(tasks);
        }

        public Task<bool> CreateFileAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFileAsync(IFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PasteFileAsync(IFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PasteFolderAsync(IFolder folder)
        {
            throw new NotImplementedException();
        }

        public Task OpenFileAsync(IFile file)
        {
            //if (!_files.Contains(originalFile));
            throw new NotImplementedException();

        }

        public Task<bool> ReplaceFileWithAsync(IFile originalFile, IFile newFile)
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

        private void OnExistsChanged(FolderItemExistsChangedEventArgs e)
        {
            EventHandler<FolderItemExistsChangedEventArgs> handler = ExistsChanged;
            if (handler != null) handler(this, e);
        }

        private void OnNameChanged(FolderItemNameChangedEventArgs e)
        {
            EventHandler<FolderItemNameChangedEventArgs> handler = NameChanged;
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
                    _files.Add(_fileFactory.CreateFile(this, fileInfo));
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
        public event EventHandler<FolderItemExistsChangedEventArgs> ExistsChanged;
        public event EventHandler<FolderItemNameChangedEventArgs> NameChanged;

        #endregion
    }
}
