using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCFire.Modules.HelloWorld
{
    public interface IFolderItem
    {
        #region Methods

        Task<bool> Cut();
        Task<bool> Copy();
        Task<bool> Delete();

        #endregion

        #region Properties

        bool Exists { get; }
        string Name { get; }
        string Path { get; }

        #endregion
    }

    public class Folder : IFolderItem
    {
        #region Fields

        private List<Folder> _folders;
        private List<File> _files;
        private string _name;
        private string _path;

        #endregion

        #region Methods

        public async Task<bool> Cut()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Copy()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateFile(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteFile(File file)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PasteFile(File file)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PasteFolder(Folder folder)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Folder> Folders
        {
            get { return _folders; }
        }

        public IEnumerable<Folder> AllFolders
        {
            get
            {
                return Folders.Aggregate(Folders, (current, folder) => current.Concat(folder.AllFolders));
            }
        }

        public IEnumerable<File> Files
        {
            get { return _files; }
        }

        public IEnumerable<File> AllFiles
        {
            get
            {
                var allFiles = Files;

                foreach (var folder in Folders)
                {
                    allFiles = allFiles.Concat(folder.AllFiles);
                }

                return allFiles;
            }
        }

        #endregion

        #region Properties

        public bool Exists
        {
            get { throw new NotImplementedException(); }
        }

        public bool Empty
        {
            get { return Folders.Any() || Files.Any(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Path
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class File : IFolderItem
    {
        #region Fields

        Folder _parent;
        string _name;
        string _path;

        #endregion

        #region Constructor

        public File(Folder parent, string path)
        {
            _parent = parent;
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

        public async Task<bool> Rename(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        public bool Exists
        {
            get { return System.IO.File.Exists(Path); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Path
        {
            get { return _path; }
        }

        #endregion
    }
}
