using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Explorer.Services;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.Explorer.ViewModels
{
    [Export]
    public class WorldExplorerViewModel : Tool
    {
        public override double PreferredWidth
        {
            get { return 250; }
        }

        public WorldExplorerViewModel()
        {
            DisplayName = "World Explorer";
        }

        [Import]
        WorldExplorerService Service
        {
            set
            {
                Installs = value.Installations.Link<InstallationViewModel, Installation, BindableCollection<InstallationViewModel>>(
                    model => new InstallationViewModel { Model = model },
                    (model, viewModel) => viewModel.Model == model);
            }
        }

        public BindableCollection<InstallationViewModel> Installs { get; set; }

        public WorldItemViewModel SelectedItem { get; set; }

        [ImportMany]
        public IEnumerable<IWorldExplorerCommand> Commands { get; private set; }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }
    }
}
