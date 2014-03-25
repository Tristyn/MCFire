using System.Collections.Specialized;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using MCFire.Modules.Files.Events;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure.Extensions;

namespace MCFire.Modules.Files.ViewModels
{
    [Export]
    public class FolderItemViewModel : PropertyChangedBase
    {
        #region Fields

        IFolderItem _model;
        BindableCollection<FolderItemViewModel> _children;

        #endregion

        #region Methods

        void UpdateExists(object sender, FolderItemExistsChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => DoesntExist);
        }

        void UpdateName(object sender, FolderItemNameChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => ItemName);
        }

        #endregion

        #region Properties

        public IFolderItem Model
        {
            get { return _model; }
            set
            {
                if (value == _model || value == null) return;
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
                NotifyOfPropertyChange(() => ItemName);
                NotifyOfPropertyChange(() => DoesntExist);
                NotifyOfPropertyChange(() => IsFolder);
                NotifyOfPropertyChange(() => Children);
            }
        }

        public string ItemName
        {
            get { return _model.Name; }
            set
            {
                if (Equals(value, _model.Name)) return;
                _model.Name = value;
                NotifyOfPropertyChange(() => ItemName);
            }
        }

        public bool DoesntExist { get { return !_model.Exists; } }

        public bool IsFolder { get { return _model is IFolder; } }

        public BindableCollection<FolderItemViewModel> Children
        {
            get
            {
                var folder = _model as IFolder;
                //if (_children == null)
                //    _children = new BindableCollection<FolderItemViewModel>();
                //if (folder == null) return _children;
                if (folder == null)
                {
                    // return empty collection, initialize if necessary
                    return _children ?? (_children = new BindableCollection<FolderItemViewModel>());
                }

                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_children == null)
                    _children =
                        folder.Children.Link<FolderItemViewModel, IFolderItem, BindableCollection<FolderItemViewModel>>(
                            model => new FolderItemViewModel { Model = model },
                            (model, viewModel) => viewModel.Model == model);

                return _children;
            }
        }

        #endregion
    }
}
