using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Files.Commands;
using MCFire.Modules.Files.EventArgs;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Files.Services;

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

        public async Task OnDoubleClick()
        {
            if (SelectedItem == null) return;
            var file = SelectedItem.Model as IFile;
            if (file != null)
            {
                await file.OpenAsync();
            }
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

        /// <summary>
        /// The selected TreeViewItem item in the FileExplorerView. 
        /// Setting this property doesn't affect the TreeView, as it is a one way bind to an auto-property.
        /// </summary>
        public IFolderItemViewModel SelectedItem { get; set; }

        #endregion
    }
}
