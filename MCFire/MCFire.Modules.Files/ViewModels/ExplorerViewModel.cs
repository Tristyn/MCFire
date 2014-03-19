using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Files.Framework;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.ViewModels;

namespace MCFire.Modules.Files.ViewModels
{
    [Export]
    public class ExplorerViewModel : Tool, IFileExplorerViewModel
    {
        #region Fields

        readonly FolderService _folderService;
        readonly BindableCollection<IFolderItemViewModel> _rootFolders = new BindableCollection<IFolderItemViewModel>();

        #endregion

        #region Constructor

        [ImportingConstructor]
        public ExplorerViewModel(FolderService folderService, [ImportMany] IEnumerable<IFileExplorerCommand> commands)
        {
            DisplayName = "File Explorer";
            _folderService = folderService;
            foreach (var folder in folderService.RootFolders)
            {
                AddFolderViewModel(folder);
            }

            folderService.RootFolderAdded += AddFolderViewModelHandler;

            Commands = commands;
            foreach (var command in commands)
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

        private void AddFolderViewModelHandler(object sender, FolderEventArgs e)
        {
            RootFolders.Add(new FolderItemViewModel { Model = e.Folder });
        }

        private void AddFolderViewModel(IFolderItem folder)
        {
            RootFolders.Add(new FolderItemViewModel { Model = folder });
        }

        #endregion

        #region Properties

        public BindableCollection<IFolderItemViewModel> RootFolders
        {
            get { return _rootFolders; }
        }

        public IEnumerable<IFileExplorerCommand> Commands { get; private set; }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public object SelectedItem { get; set; }

        #endregion
    }
}
