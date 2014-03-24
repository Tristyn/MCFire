using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Services
{
    [Export]
    public class FolderService
    {
        #region Fields

        readonly FileFactory _fileFactory;
        readonly ObservableCollection<IFolder> _rootFolders = new ObservableCollection<IFolder>();
        readonly object _lock = new object();

        #endregion

        #region Constructor

        [ImportingConstructor]
        public FolderService(FileFactory fileFactory)
        {
            _fileFactory = fileFactory;
        }

        #endregion

        #region Methods

        public IFolder GetOrCreateFolder(string path)
        {
            IFolder newFolder;
            lock (_lock)
            {
                // try to return existing folder
                var first = _rootFolders.FirstOrDefault(folder => folder.Path == path.ToLower());
                if (first != null) return first;

                // create new folder
                newFolder = new Folder(null, path.ToLower(), _fileFactory);
                _rootFolders.Add(newFolder);
            }
            return newFolder;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns all root folders. 
        /// The IEnumerable is thread safe to the consumer, 
        /// and won't be updated when changes are made to this object.
        /// </summary>
        public ObservableCollection<IFolder> RootFolders
        {
            get
            {
                lock (_lock)
                {
                    return _rootFolders;
                }
            }
        }

        #endregion
    }
}
