using System.Collections.Generic;
using System.IO;

namespace MCFire.Client.Gui.Modules.Explorer.ViewModels
{
    [Export]
    public class WorldExplorerViewModel : Tool
    {
        public WorldExplorerViewModel()
        {
            DisplayName = "World Explorer";
        }

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        public override void SaveState(BinaryWriter writer)
        {
            writer.Write(true);
        }

        public override void LoadState(BinaryReader reader)
        {
            reader.ReadBoolean();
        }

        [Import]
        WorldExplorerService Service
        {
            set
            {
                Installs = value.Installations.Tether<InstallationViewModel, Installation, BindableCollection<InstallationViewModel>>(
                    model => new InstallationViewModel { Model = model },
                    (model, viewModel) => viewModel.Model == model);
            }
        }

        public BindableCollection<InstallationViewModel> Installs { get; set; }

        public WorldItemViewModel SelectedItem { get; set; }

        [ImportMany]
        public IEnumerable<IWorldExplorerCommand> Commands { get; private set; }

        public override double PreferredWidth
        {
            get { return 250; }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }
    }
}
