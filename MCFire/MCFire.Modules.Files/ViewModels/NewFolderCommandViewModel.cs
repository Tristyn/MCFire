using System.ComponentModel.Composition;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.ViewModels;

namespace MCFire.Modules.Files.ViewModels
{
    [Export(typeof(IFileExplorerCommand))]
    public class NewFolderCommandViewModel : IFileExplorerCommand
    {
        public IFileExplorerViewModel FileExplorer { set; private get; }

        public void AddFolder()
        {
            FileExplorer.NewFolder();
        }
    }
}
