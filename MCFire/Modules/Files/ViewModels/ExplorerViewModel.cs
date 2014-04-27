using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Files.Commands;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.Files.ViewModels
{
    [Export]
    public class ExplorerViewModel : Tool, IFileExplorerViewModel
    {
        #region Fields

        readonly FolderService _folderService;
        readonly BindableCollection<FolderItemViewModel> _rootFolders;

        #endregion

        #region Constructor

        [ImportingConstructor]
        public ExplorerViewModel(FolderService folderService, [ImportMany] IEnumerable<IFileExplorerCommand> commands)
        {
            DisplayName = "File Explorer";
            _folderService = folderService;
            _rootFolders = folderService.RootFolders.Link
                <FolderItemViewModel, IFolder, BindableCollection<FolderItemViewModel>>(
                model => new FolderItemViewModel { Model = model },
                (model, viewModel) => viewModel.Model == model);

            var commandsList = commands as IList<IFileExplorerCommand> ?? commands.ToList();
            Commands = commandsList;
            foreach (var command in commandsList)
            {
                command.FileExplorer = this;
            }
        }

        #endregion

        #region Methods

        public void NewFolder()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder to be added to the File Explorer."
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                _folderService.GetOrCreateFolder(dialog.SelectedPath);
        }

        public void OnDoubleClick()
        {
            // expands the tree view automatically
            if (SelectedItem == null) return;
            var file = SelectedItem.Model as IFile;
            if (file != null)
            {
                throw new NotImplementedException("changes to files and content have made opening files via double click not work anymore.");
                //await file.OpenAsync();
            }
        }

        #endregion

        #region Properties

        public BindableCollection<FolderItemViewModel> RootFolders
        {
            get { return _rootFolders; }
        }

        public IEnumerable<IFileExplorerCommand> Commands { get; private set; }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        /// <summary>
        /// The selected TreeViewItem item in the FileExplorerView. 
        /// Setting this property doesn't affect the TreeView, as it is a one way bind to an auto-property.
        /// </summary>
        public FolderItemViewModel SelectedItem { get; set; }

        #endregion
    }
}
