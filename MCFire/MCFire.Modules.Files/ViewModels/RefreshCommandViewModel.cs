using System.ComponentModel.Composition;
using System.Linq;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.ViewModels;

namespace MCFire.Modules.Files.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IFileExplorerCommand))]
    public class RefreshCommandViewModel : IFileExplorerCommand
    {
        public IFileExplorerViewModel FileExplorer { set; private get; }

        public void Refresh()
        {
            foreach (var folder in FileExplorer.RootFolders)
            {
                // TODO: ASYNC??!??! OH GOD
                folder.Model.Refresh();
            }
        }

        public bool CanRefresh()
        {
            return FileExplorer != null && FileExplorer.RootFolders.Any();
        }
    }
}
