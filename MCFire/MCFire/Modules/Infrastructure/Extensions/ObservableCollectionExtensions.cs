using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MCFire.Modules.Infrastructure.Extensions
{
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

        /// <summary>
        /// Links the two collections such that when 
        /// the items of sourceColletion are changed, 
        /// the changes are mirrored in the targetCollection.
        /// </summary>
        /// <typeparam name="TTarget">The type of target collection</typeparam>
        /// <typeparam name="TSource">The type that the collection is binding to.</typeparam>
        /// <param name="sourceCollection">The collection to observe changes to.</param>
        /// <param name="targetFactory">The function to create a new TTarget using a TSource</param>
        /// <param name="comparer">The function to determine if the TTarget was created using the instance of TSource</param>
        public static ObservableCollection<TTarget> Link<TTarget, TSource>(this ObservableCollection<TSource> sourceCollection, Func<TSource, TTarget> targetFactory, Func<TSource, TTarget, bool> comparer)
        {
            var targetCollection = new ObservableCollection<TTarget>();
            sourceCollection.CollectionChanged += (s, e) => e.Handle(targetCollection, targetFactory, comparer);
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0).Handle(targetCollection, targetFactory, comparer);
            return targetCollection;
        }

        public static ObservableCollection<TTarget> Link<TTarget, TSource>(this ObservableCollection<TSource> sourceCollection)
            where TSource : TTarget
            where TTarget : class
        {
            var targetCollection = new ObservableCollection<TTarget>();
            sourceCollection.CollectionChanged += (s, e) => e.Handle(targetCollection);
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0).Handle(targetCollection);
            return targetCollection;
        }

        public static TTargetCollection Link<TTarget, TSource, TTargetCollection>(this ObservableCollection<TSource> sourceCollection)
            where TTargetCollection : ObservableCollection<TTarget>
            where TSource : TTarget
            where TTarget : class
        {
            var targetCollection = Activator.CreateInstance<TTargetCollection>();
            sourceCollection.CollectionChanged += (s, e) => e.Handle(targetCollection);
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0).Handle(targetCollection);
            return targetCollection;
        }

        public static TTargetCollection Link<TTarget, TSource, TTargetCollection>(this ObservableCollection<TSource> sourceCollection, Func<TSource, TTarget> targetFactory, Func<TSource, TTarget, bool> comparer)
            where TTargetCollection : ObservableCollection<TTarget>
        {
            var targetCollection = Activator.CreateInstance<TTargetCollection>();
            sourceCollection.CollectionChanged += (s, e) => e.Handle(targetCollection, targetFactory, comparer);
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0).Handle( targetCollection, targetFactory, comparer);
            return targetCollection;
        }

        public static void LinkExisting<TTarget, TSource>(this ObservableCollection<TSource> sourceCollection, ObservableCollection<TTarget> targetCollection)
            where TSource : TTarget
            where TTarget : class
        {
            sourceCollection.CollectionChanged += (s, e) => e.Handle(targetCollection);
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceCollection, 0).Handle(targetCollection);
        }
    }
}