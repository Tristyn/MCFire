using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class NotifyCollectionChangedExtensions
    {
        /// <summary>
        /// Extends BindableCollection and makes 'Linked' ObservableCollections very simple to set up.
        /// </summary>
        /// <typeparam name="TTarget">The target type, usually a view model</typeparam>
        /// <typeparam name="TSource">The source type, usually a model</typeparam>
        /// <param name="sourceArgs">The NotifyCollectionChangedEventArgs</param>
        /// <param name="targetCollection"></param>
        /// <param name="targetFactory"></param>
        /// <param name="comparer"></param>
        public static void Handle<TTarget, TSource>([NotNull] this NotifyCollectionChangedEventArgs sourceArgs, BindableCollection<TTarget> targetCollection, Func<TSource, TTarget> targetFactory, Func<TSource, TTarget, bool> comparer)
        {
            switch (sourceArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(sourceArgs, targetCollection, targetFactory);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(sourceArgs, targetCollection, comparer);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Replace(sourceArgs, targetCollection, targetFactory);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Move(sourceArgs, targetCollection);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Remove(sourceArgs, targetCollection, comparer);
                    Add(sourceArgs, targetCollection, targetFactory);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static void Add<TTarget, TSource>(NotifyCollectionChangedEventArgs e, BindableCollection<TTarget> targetCollection, Func<TSource, TTarget> targetFactory)
        {
            var index = e.NewStartingIndex;
            if(index==-1)
                targetCollection.Insert(-1, e.NewItems.Cast<TSource>().Select(targetFactory).First());
            foreach (var targetItem in e.NewItems.Cast<TSource>().Select(targetFactory))
            {
                targetCollection.Insert(index, targetItem);
                index++;
            }
        }

        static void Remove<TTarget, TSource>(NotifyCollectionChangedEventArgs e, BindableCollection<TTarget> targetCollection, Func<TSource, TTarget, bool> comparer)
        {
            targetCollection.RemoveRange(from sourceItem in e.OldItems.Cast<TSource>()
                                         select targetCollection.First(targetItem => comparer(sourceItem, targetItem)));
        }

        static void Replace<TTarget, TSource>(NotifyCollectionChangedEventArgs e, BindableCollection<TTarget> targetCollection, Func<TSource, TTarget> targetFactory)
        {
            targetCollection[e.NewStartingIndex] = targetFactory(e.NewItems.Cast<TSource>().First());
        }

        static void Move<TTarget>(NotifyCollectionChangedEventArgs e, BindableCollection<TTarget> targetCollection)
        {
            targetCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
        }
    }

    // This class extends ObservableCollection and makes 'Linked' ObservableCollections very simple to set up

    // consider the following situation.
    // In the Model class there is an List<SubModel> SubModels.
    // In the ViewModel class there is an ObservableCollection<SubViewModel> SubViewModels for binding to a view.
    // Whenever a subModel is added to SubModels, a SubViewModel must be created, given a reference to the SubModel, and added to SubViewModels.
    // Furthermore, the Model class interacts with other ViewModels aswell, and the ViewModel in this situation does not know when SubViewModels are added by outside sources.
    // Therefor we events to notify the ViewModel of changes: SubModelAdded, SubModelRemoved.

    // This kind of synchrony is very hard to achieve, and if it were implemented would be riddled with many edge cases.
    // For example, what if SubModels.Insert(5, new model()) was called? SubModelAdded would fire and a new SubViewModel instance would be created, but the new SubViewModel is at the back of the list while the SubModel is somewhere in the middle. The index value was lost.
    // Other calls to SubViewModels would require their own special events: AddRange, RemoveRange, Clear, Move and RemoveAt.

    // Wpf handles these issues with stride, leveraging ObservableCollection.CollectionChanged to synchronize view items to any ObservableCollection.
    // By changing the type of SubModels to an ObservableCollection, we can leverage CollectionChanged events too, but it requires excessive amounts of boilerplate code per implementation that cant be hidden well via inheritance.
    // The following code is required to link one ObservableCollection<SubModel> to an ObservableCollection<SubViewModel>

    class Model
    {
        public readonly BindableCollection<SubModel> SubModels = new BindableCollection<SubModel>();
    }

    class ViewModel
    {
        public readonly BindableCollection<SubViewModel> SubViewModels = new BindableCollection<SubViewModel>();

        public ViewModel(Model model)
        {
            model.SubModels.CollectionChanged += SubModelsChanged;
            // SubViewModels will get created automatically when a SubModel is added!
            // But the code is ugly, and types need to be changed for every implementation.
        }

        private void SubModelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var index = e.NewStartingIndex;
                    foreach (var viewModel in from SubModel newItem in e.NewItems select new SubViewModel { Model = newItem })
                    {
                        SubViewModels.Insert(index, viewModel);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        SubViewModels.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    SubViewModels[e.NewStartingIndex] =
                        (from SubModel item in e.NewItems
                         select new SubViewModel { Model = item }
                         ).First();
                    break;
                case NotifyCollectionChangedAction.Move:
                    SubViewModels.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    SubViewModels.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    class SubModel
    { // Simple data object
        public int Id { get; set; }
        public string Data { get; set; }
    }

    class SubViewModel : INotifyPropertyChanged
    { // sub view model
        public SubModel Model { get; set; }
        public int Id
        {
            get { return Model.Id; }
            set
            {
                Model.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Data
        {
            get { return Model.Data; }
            set
            {
                Model.Data = value;
                OnPropertyChanged("Data");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // using extensions

    class ViewModelUsingExtensions
    {
        public readonly BindableCollection<SubViewModel> SubViewModels = new BindableCollection<SubViewModel>();
        public ViewModelUsingExtensions(Model model)
        {
            
            model.SubModels.CollectionChanged += (s, e) => e.Handle<SubViewModel, SubModel>
                 (SubViewModels,
                 subModel => new SubViewModel { Model = subModel },
                 (subModel, viewModel) => viewModel.Model == subModel);
        }

    }
}
