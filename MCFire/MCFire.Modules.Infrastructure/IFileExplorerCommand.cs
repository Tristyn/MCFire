using MCFire.Modules.Infrastructure.ViewModels;

namespace MCFire.Modules.Infrastructure
{
    public interface IFileExplorerCommand
    {
        IFileExplorerViewModel FileExplorer { set; }
    }
}
