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
        #region Properties

        [ImportingConstructor]
        public WorldExplorerViewModel(WorldExplorerService service)
        {
            DisplayName = "World Explorer";

            Children = service.Installations.Link<WorldItemViewModel, Installation, BindableCollection<WorldItemViewModel>>(
                model => new WorldItemViewModel { Model = model },
                (model, viewModel) => viewModel.Model == model);
        }

        #endregion

        #region Properties

        public BindableCollection<WorldItemViewModel> Children { get; private set; }

        public WorldItemViewModel SelectedItem { get; set; }

        [ImportMany]
        public IEnumerable<IWorldExplorerCommand> Commands { get; private set; }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        #endregion
    }
}
