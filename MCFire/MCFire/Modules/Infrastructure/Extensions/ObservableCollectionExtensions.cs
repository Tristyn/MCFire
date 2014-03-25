using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    }
}