using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Metro.Commands;
using MCFire.Modules.WorldExplorer.Models;
using MCFire.Modules.WorldExplorer.Services;

namespace MCFire.Modules.WorldExplorer.ViewModels
{
    [Export]
    public class WorldExplorerViewModel : Tool
    {
        #region Properties

        [ImportingConstructor]
        public WorldExplorerViewModel(WorldExplorerService service)
        {
            DisplayName = "World Explorer";

            Children = service.Children.Link<WorldItemViewModel, WorldBrowserItem, BindableCollection<WorldItemViewModel>>(
                model => new WorldItemViewModel { Model = model },
                (model, viewModel) => viewModel.Model == model);
        }

        #endregion

        #region Properties

        public BindableCollection<WorldItemViewModel> Children { get; private set; }

        public WorldItemViewModel SelectedItem { get; set; }

        [ImportMany]
        public IEnumerable<IWindowCommand> Commands { get; private set; }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        #endregion
    }
}
