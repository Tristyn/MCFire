using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Files.Framework;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.ViewModels
{
    [Export]
    public class ExplorerViewModel : Tool
    {
        #region Fields

        readonly FolderService _folderService;
        readonly ExplorerComposer _composer;
        readonly BindableCollection<FolderItemViewModel> _rootFolders = new BindableCollection<FolderItemViewModel>();

        #endregion

        #region Constructor

        [ImportingConstructor]
        public ExplorerViewModel(FolderService folderService, ExplorerComposer composer)
        {
            DisplayName = "File Explorer";
            _folderService = folderService;
            _composer = composer;
            foreach (var folder in folderService.RootFolders)
            {
                AddFolderViewModel(folder);
            }

            folderService.RootFolderAdded += AddFolderViewModelHandler;
        }

        #endregion

        #region Methods

        public void NewFolder()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select a folder to be added to the Explorer."
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

        public BindableCollection<FolderItemViewModel> RootFolders
        {
            get { return _rootFolders; }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        #endregion
    }
}
