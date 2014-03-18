using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MCFire.Modules.Files.Framework;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Services
{
    [Export]
    public class FolderService
    {
        #region Fields

        readonly FileFactory _fileFactory;

        #endregion

        #region Constructor

        [ImportingConstructor]
        public FolderService(FileFactory fileFactory)
        {
            _fileFactory = fileFactory;
            RootFolders = new List<Folder>();
        }

        #endregion

        #region Methods

        public Folder GetOrCreateFolder(string path)
        {
            // try to return existing folder
            var first = RootFolders.FirstOrDefault(folder => folder.Path == path.ToLower());
            if (first != null) return first;

            // create new folder
            var newFolder = new Folder(null, path, _fileFactory);
            RootFolders.Add(newFolder);
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

        public List<Folder> RootFolders { get; private set; }
        public event EventHandler<FolderEventArgs> RootFolderAdded;

        #endregion
    }
}
