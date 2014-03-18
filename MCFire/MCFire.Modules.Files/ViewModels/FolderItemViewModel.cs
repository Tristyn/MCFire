using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Modules.Infrastructure;
using MCFire.Modules.Infrastructure.ViewModels;

namespace MCFire.Modules.Files.ViewModels
{
    [Export]
    public class FolderItemViewModel : PropertyChangedBase, IFolderItemViewModel
    {
        IFolderItem _model;
        BindableCollection<IFolderItemViewModel> _children;

        public IFolderItem Model
        {
            get { return _model; }
            set
            {
                if (value == Model) return;
                _model = value;

                // reset children
                _children = null;
                NotifyOfPropertyChange(() => Model);
                NotifyOfPropertyChange(() => Children);
            }
        }

        public string Name
        {
            get { return _model.Name; }
            // TODO: setting _model.Name currently throws NotImplementedException 
            //set
            //{
            //    if (Equals(value, _model.Name)) return;
            //    _model.Name = value;
            //    NotifyOfPropertyChange(() => Name);
            //}
        }

        public BindableCollection<IFolderItemViewModel> Children
        {
            get
            {
                var folder = _model as IFolder;
                if (_children == null)
                    _children = new BindableCollection<IFolderItemViewModel>();
                if (folder == null) return _children;
                foreach (var childFolder in folder.Children)
                {
                    _children.Add(new FolderItemViewModel { _model = childFolder });
                }
                return _children;
            }
        }
    }
}
