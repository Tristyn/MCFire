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
        [ImportMany]
        public IEnumerable<IWindowCommand> Commands { get; private set; }
        readonly object _lock = new Object();

        [ImportingConstructor]
        public WorldExplorerViewModel(FolderService folderService)
        {
            DisplayName = "World Explorer";
            Children = new BindableCollection<WorldItemViewModel>();
            Model = new WorldExplorerModel(folderService.RootFolders);
            Model.Children.CollectionChanged += HandleWorldBrowserItems;
            HandleWorldBrowserItems(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Model.Children, 0));
        }

        private void HandleWorldBrowserItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.Handle<WorldItemViewModel, WorldBrowserItem>(
                Children,
                model => new WorldItemViewModel { Model = model },
                (model, viewModel) => viewModel.Model == model);
        }

        public WorldExplorerModel Model { get; private set; }
        public BindableCollection<WorldItemViewModel> Children { get; private set; }

        public WorldItemViewModel SelectedItem { get; set; }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }
    }
}
