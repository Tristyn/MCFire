using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Links the two collections such that when 
        /// the items of sourceColletion are changed, 
        /// the changes are mirrored in the targetCollection.
        /// </summary>
        /// <typeparam name="TTarget">The type of target collection</typeparam>
        /// <typeparam name="TSource">The type that the collection is binding to.</typeparam>
        /// <param name="targetCollection">The target collection.</param>
        /// <param name="sourceCollection">The collection to observe changes to.</param>
        public static void Link<TTarget, TSource>(this ObservableCollection<TTarget> targetCollection,
            ObservableCollection<TSource> sourceCollection)
            where TSource : TTarget
            where TTarget : class
        {
            sourceCollection.CollectionChanged += (s, e) => Handle(e, targetCollection);
            Handle(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0), targetCollection);
        }

        /// <summary>
        /// Links the two collections such that when 
        /// the items of sourceColletion are changed, 
        /// the changes are mirrored in the targetCollection.
        /// </summary>
        /// <typeparam name="TTarget">The type of target collection</typeparam>
        /// <typeparam name="TSource">The type that the collection is binding to.</typeparam>
        /// <param name="targetCollection">The target collection.</param>
        /// <param name="sourceCollection">The collection to observe changes to.</param>
        /// <param name="targetFactory">The function to create a new TTarget using a TSource</param>
        /// <param name="comparer">The function to determine if the TTarget was created using the instance of TSource</param>
        public static void Link<TTarget, TSource>(this ObservableCollection<TTarget> targetCollection, ObservableCollection<TSource> sourceCollection, Func<TSource, TTarget> targetFactory, Func<TSource, TTarget, bool> comparer)
        {
            sourceCollection.CollectionChanged += (s, e) => Handle(e, targetCollection, targetFactory, comparer);
            Handle(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0), targetCollection, targetFactory, comparer);
        }

        /// <summary>
        /// Extends ObservableCollection and makes 'Linked' ObservableCollections very simple to set up.
        /// </summary>
        /// <typeparam name="TTarget">The target type, usually a view model</typeparam>
        /// <param name="e">The NotifyCollectionChangedEventArgs</param>
        /// <param name="targetCollection">The target collection</param>
        static void Handle<TTarget>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection) where TTarget : class
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e, targetCollection);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e, targetCollection);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Replace(e, targetCollection);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Move(e, targetCollection);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException("Resetting collections not implemented.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Extends ObservableCollection and makes 'Linked' ObservableCollections very simple to set up.
        /// </summary>
        /// <typeparam name="TTarget">The target type, usually a view model</typeparam>
        /// <typeparam name="TSource">The source type, usually a model</typeparam>
        /// <param name="e">The NotifyCollectionChangedEventArgs</param>
        /// <param name="targetCollection">The target collection</param>
        /// <param name="targetFactory">The function to create a new TTarget using a TSource</param>
        /// <param name="comparer">The function to determine if the TTarget was created using the instance of TSource</param>
        public static void Handle<TTarget, TSource>([NotNull] this NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection, Func<TSource, TTarget> targetFactory, Func<TSource, TTarget, bool> comparer)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e, targetCollection, targetFactory);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Remove(e, targetCollection, comparer);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Replace(e, targetCollection, targetFactory);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Move(e, targetCollection);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException("Resetting collections not implemented.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Adds the items found in e.NewItems to the targetCollection with no transformation.
        /// </summary>
        static void Add<TTarget>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection)
        {
            var index = e.NewStartingIndex;
            if (index == -1)
                targetCollection.Insert(0, e.NewItems.Cast<TTarget>().First());
            foreach (var targetItem in e.NewItems.Cast<TTarget>())
            {
                targetCollection.Insert(index, targetItem);
                index++;
            }
        }

        /// <summary>
        /// Adds the items found in e.NewItems to the targetCollection, specifying a transformation from TSource to TTarget.
        /// </summary>
        static void Add<TTarget, TSource>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection, Func<TSource, TTarget> targetFactory)
        {
            var index = e.NewStartingIndex;
            if (index == -1)
                targetCollection.Insert(-1, e.NewItems.Cast<TSource>().Select(targetFactory).First());
            foreach (var targetItem in e.NewItems.Cast<TSource>().Select(targetFactory))
            {
                targetCollection.Insert(index, targetItem);
                index++;
            }
        }

        /// <summary>
        /// Removes the items found in e.OldItems from the targetCollection, using the default equality comparer ==
        /// </summary>
        static void Remove<T>(NotifyCollectionChangedEventArgs e, ObservableCollection<T> targetCollection) where T : class
        {
            targetCollection.RemoveForeach(from sourceItem in e.OldItems.Cast<T>()
                                           select targetCollection.First(targetItem => sourceItem == targetItem));
        }

        /// <summary>
        /// Removes the items found in e.OldItems from the targetCollection, specifying a custom equality comparer.
        /// </summary>
        static void Remove<TTarget, TSource>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection, Func<TSource, TTarget, bool> comparer)
        {
            targetCollection.RemoveForeach(from sourceItem in e.OldItems.Cast<TSource>()
                                           select targetCollection.First(targetItem => comparer(sourceItem, targetItem)));
        }

        /// <summary>
        /// Replaces the item in targetCollection at the same point where the change occured in the source collection with no transformation.
        /// </summary>
        static void Replace<T>(NotifyCollectionChangedEventArgs e, ObservableCollection<T> targetCollection)
        {
            targetCollection[e.NewStartingIndex] = e.NewItems.Cast<T>().First();
        }

        /// <summary>
        /// Replaces the item in targetCollection at the same point where the change occured in the source collection, 
        /// specifying a transformation from TSource to TTarget. 
        /// </summary>
        static void Replace<TTarget, TSource>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection, Func<TSource, TTarget> targetFactory)
        {
            targetCollection[e.NewStartingIndex] = targetFactory(e.NewItems.Cast<TSource>().First());
        }

        /// <summary>
        /// Moves the item in targetCollection using the indexes that occured in the source collection.
        /// </summary>
        static void Move<TTarget>(NotifyCollectionChangedEventArgs e, ObservableCollection<TTarget> targetCollection)
        {
            targetCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
        }
    }

    public static class ObservableCollectionExtensions
    {


        /// <summary>
        /// Iterates through range and calls Add() foreach item.
        /// This solves the issue of AddRange() saying that bindings should be reset.
        /// </summary>
        public static void AddForeach<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Iterates through range and calls Remove() foreach item.
        /// This solves the issue of RemoveRange() saying that bindings should be reset.
        /// </summary>
        public static void RemoveForeach<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                collection.Remove(item);
            }
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
