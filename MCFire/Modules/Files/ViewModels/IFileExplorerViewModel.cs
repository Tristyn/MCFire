using System.Collections.Generic;
using Caliburn.Micro;
using MCFire.Modules.Files.Commands;

namespace MCFire.Modules.Files.ViewModels
{
    public interface IFileExplorerViewModel
    {
        void NewFolder();
        BindableCollection<FolderItemViewModel> RootFolders { get; }
        IEnumerable<IFileExplorerCommand> Commands { get; }

        /// <summary>
        /// The selected TreeViewItem item in the FileExplorerView. 
        /// Setting this property doesn't affect the TreeView, as it is a one way bind to an auto-property.
        /// </summary>
        FolderItemViewModel SelectedItem { get; set; }
    }
}
