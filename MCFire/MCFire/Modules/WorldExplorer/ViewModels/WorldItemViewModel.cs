using System.Collections.Specialized;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.WorldExplorer.Models;

namespace MCFire.Modules.WorldExplorer.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class WorldItemViewModel : PropertyChangedBase
    {
        WorldBrowserItem _model;
        private BindableCollection<WorldItemViewModel> _children;

        private void HandleWorldBrowserItem(object s, NotifyCollectionChangedEventArgs e)
        {
            e.Handle<WorldItemViewModel, WorldBrowserItem>(_children, model => new WorldItemViewModel { Model = model }, (model, viewModel) => viewModel.Model == model);
        }

        public string Title
        {
            get { return _model.Title; }
        }

        public WorldBrowserItem Model
        {
            get { return _model; }
            set
            {
                _model = value;
                _model.Children.CollectionChanged += HandleWorldBrowserItem;
                HandleWorldBrowserItem(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _model.Children, 0));
            }
        }

        public BindableCollection<WorldItemViewModel> Children
        {
            get
            {
                if (_children != null)
                    return _children;
                _children = new BindableCollection<WorldItemViewModel>();
                Model.Children.CollectionChanged += HandleWorldBrowserItem;
                HandleWorldBrowserItem(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Model.Children, 0));
                return _children;
            }
        }
    }
}
