using Caliburn.Micro;

namespace MCFire.Modules.Infrastructure.ViewModels
{
    public interface IFileExplorerViewModel
    {
        void NewFolder();
        BindableCollection<IFolderItemViewModel> RootFolders { get; }
    }
}
