using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Modules.Files.EventArgs;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.ViewModels
{
    [Export]
    public class FolderItemViewModel : PropertyChangedBase, IFolderItemViewModel
    {
        #region Fields

        IFolderItem _model;
        BindableCollection<IFolderItemViewModel> _children;

        #endregion

        #region Methods

        void UpdateExists(object sender, FolderItemExistsChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => DoesntExist);
        }

        void UpdateName(object sender, FolderItemNameChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => Name);
        }

        #endregion

        #region Properties

        public IFolderItem Model
        {
            get { return _model; }
            set
            {
                if (value == _model) return;
                if (_model != null)
                {
                    _model.ExistsChanged -= UpdateExists;
                    _model.NameChanged -= UpdateName;
                }

                _model = value;
                _model.ExistsChanged += UpdateExists;
                _model.NameChanged += UpdateName;

                // reset children
                _children = null;
                NotifyOfPropertyChange(() => Model);
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => DoesntExist);
                NotifyOfPropertyChange(() => IsFolder);
                NotifyOfPropertyChange(() => Children);
            }
        }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                if (Equals(value, _model.Name)) return;
                _model.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool DoesntExist { get { return !_model.Exists; } }

        public bool IsFolder { get { return _model is IFolder; } }

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
                    _children.Add(new FolderItemViewModel { Model = childFolder });
                }
                return _children;
            }
        }

        #endregion
    }
}
