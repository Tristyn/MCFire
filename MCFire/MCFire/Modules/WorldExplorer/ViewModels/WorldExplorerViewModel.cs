using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Files.Services;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Metro.Commands;
using MCFire.Modules.WorldExplorer.Models;

namespace MCFire.Modules.WorldExplorer.ViewModels
{
    [Export]
    public class WorldExplorerViewModel : Tool
    {
        #region Fields

        readonly object _lock = new Object();

        #endregion

        #region Properties

        [ImportingConstructor]
        public WorldExplorerViewModel(FolderService folderService)
        {
            DisplayName = "World Explorer";
            Children = new BindableCollection<WorldItemViewModel>();
            Model = new WorldExplorerModel(folderService.RootFolders);
            Model.Children.CollectionChanged += HandleWorldBrowserItems;
            HandleWorldBrowserItems(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Model.Children, 0));
        }

        #endregion

        #region Methods

        private void HandleWorldBrowserItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.Handle<WorldItemViewModel, WorldBrowserItem>(
                Children,
                model => new WorldItemViewModel { Model = model },
                (model, viewModel) => viewModel.Model == model);
        }

        #endregion

        #region Properties

        public WorldExplorerModel Model { get; private set; }

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
