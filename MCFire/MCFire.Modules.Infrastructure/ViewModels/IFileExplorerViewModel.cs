using System.Collections.Generic;
using Caliburn.Micro;

namespace MCFire.Modules.Infrastructure.ViewModels
{
    public interface IFileExplorerViewModel
    {
        void NewFolder();
        BindableCollection<IFolderItemViewModel> RootFolders { get; }
        IEnumerable<IFileExplorerCommand> Commands { get; }

        /// <summary>
        /// The selected TreeViewItem item in the FileExplorerView. 
        /// Setting this property doesn't affect the TreeView, as it is a one way bind to an auto-property.
        /// </summary>
        IFolderItemViewModel SelectedItem { get; set; }
    }
}
