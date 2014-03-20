using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MCFire.Modules.Files.EventArgs;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Services
{
    [Export]
    public class FolderService
    {
        #region Fields

        readonly FileFactory _fileFactory;
        readonly List<IFolder> _rootFolders = new List<IFolder>();
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
                newFolder = new Folder(null, path, _fileFactory);
                _rootFolders.Add(newFolder);
            }
            OnRootFolderAdded(new FolderEventArgs(newFolder));
            return newFolder;
        }

        protected virtual void OnRootFolderAdded(FolderEventArgs e)
        {
            var handler = RootFolderAdded;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns all root folders. 
        /// The IEnumerable is thread safe to the consumer, 
        /// and won't be updated when changes are made to this object.
        /// </summary>
        public IEnumerable<IFolder> RootFolders
        {
            get
            {
                lock (_lock)
                {
                    return new List<IFolder>(_rootFolders);
                }
            }
        }

        public event EventHandler<FolderEventArgs> RootFolderAdded;

        #endregion
    }
}
