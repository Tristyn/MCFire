namespace MCFire.Client.Gui.Modules.Explorer.ViewModels
{
    public class InstallationViewModel
    {
        public InstallationViewModel()
        {
            Worlds = new BindableCollection<WorldViewModel>();
        }

        Installation _model;

        public Installation Model
        {
            get { return _model; }
            set
            {
                _model = value;
                value.Worlds.TetherExisting(
                    Worlds, world => new WorldViewModel { Model = world }, 
                    (model, viewModel) => viewModel.Model == model);
            }
        }

        public string Title { get { return Model.Title; } }
        public string Path { get { return Model.Path; } }
        public BindableCollection<WorldViewModel> Worlds { get; set; }
    }
}
