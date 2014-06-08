﻿namespace MCFire.Client.Gui.Modules.Explorer.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class WorldItemViewModel : PropertyChangedBase
    {
        WorldBrowserItem _model;

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
                //Children =
                //    Model.Children.Link<WorldItemViewModel, WorldBrowserItem, BindableCollection<WorldItemViewModel>>(
                //    model => new WorldItemViewModel { Model = model }, 
                //    (model, viewModel) => viewModel.Model == model);
            }
        }

        public BindableCollection<WorldItemViewModel> Children { get; private set; }
    }
}