using MCFire.Modules.Files.ViewModels;

namespace MCFire.Modules.Files.Commands
{
    public interface IFileExplorerCommand
    {
        IFileExplorerViewModel FileExplorer { set; }
    }
}
