using Caliburn.Micro;

namespace MCFire.Modules.Infrastructure.ViewModels
{
    public interface IFolderItemViewModel
    {
        IFolderItem Model { get; set; }

        string Name { get; }

        BindableCollection<IFolderItemViewModel> Children { get; }
        bool DoesntExist { get; }
        bool IsFolder { get; }
    }
}