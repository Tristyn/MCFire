using MCFire.Modules.Files.ViewModels;
using MCFire.Modules.Infrastructure.Interfaces;

namespace MCFire.Modules.Files.Commands
{
    public interface IFileExplorerCommand : ICommand
    {
        IFileExplorerViewModel FileExplorer { set; }
    }
}
