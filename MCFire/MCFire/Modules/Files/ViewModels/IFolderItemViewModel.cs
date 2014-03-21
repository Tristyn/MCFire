using Caliburn.Micro;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.ViewModels
{
    public interface IFolderItemViewModel
    {
        IFolderItem Model { get; set; }

        string ItemName { get; }

        BindableCollection<IFolderItemViewModel> Children { get; }
        bool DoesntExist { get; }
        bool IsFolder { get; }
    }
}