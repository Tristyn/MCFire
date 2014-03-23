using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
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
        readonly BindableCollection<FolderItemViewModel> _rootFolders = new BindableCollection<FolderItemViewModel>();
        readonly object _lock = new object();

        #endregion

        #region Constructor

        [ImportingConstructor]
        public ExplorerViewModel(FolderService folderService, [ImportMany] IEnumerable<IFileExplorerCommand> commands)
        {
            DisplayName = "File Explorer";
            _folderService = folderService;
            folderService.RootFolders.CollectionChanged += HandleRootFolders;
            HandleRootFolders(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _folderService.RootFolders, 0));

            var commandsList = commands as IList<IFileExplorerCommand> ?? commands.ToList();
            Commands = commandsList;
            foreach (var command in commandsList)
            {
                command.FileExplorer = this;
            }
        }

        private void HandleRootFolders(object s, NotifyCollectionChangedEventArgs e)
        {
            lock (_lock)
            {
                e.Handle<FolderItemViewModel, IFolderItem>(
                    RootFolders,
                    model => new FolderItemViewModel { Model = model },
                    (model, viewModel) => viewModel.Model == model);
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
        {// expands the tree view automatically
            if (SelectedItem == null) return;
            var file = SelectedItem.Model as IFile;
            if (file != null)
            {
                await file.OpenAsync();
            }
        }

        private void FoldersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var index = e.NewStartingIndex;
                    foreach (var viewModel in from IFolderItem newItem in e.NewItems select new FolderItemViewModel { Model = newItem })
                    {
                        RootFolders.Insert(index, viewModel);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    //for (var i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems.Count; i++)
                    //{
                    //    RootFolders.RemoveAt(i);
                    //}
                    var viewModels = from IFolderItem oldItem in e.OldItems
                                     select RootFolders.First(oldViewModel => oldViewModel.Model == oldItem);
                    RootFolders.RemoveRange(viewModels);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RootFolders[e.NewStartingIndex] =
                        e.NewItems.Cast<IFolderItem>() // cast for linq functions
                            .Select(folderItem => new FolderItemViewModel { Model = folderItem })
                            .First();
                    break;
                case NotifyCollectionChangedAction.Move: // move 1
                    RootFolders.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RootFolders.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
