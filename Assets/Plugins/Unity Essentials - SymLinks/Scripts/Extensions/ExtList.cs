using System.Collections.Generic;

namespace UnityEssentials.symbolicLinks.extensions
{
    /// <summary>
    /// extension of list
    /// </summary>
    public static class ExtList
    {
        /// <summary>
        /// return true if a given list contain a given index.
        /// if an item is found, index is the index of the item in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">list</param>
        /// <param name="item">item to find in the list</param>
        /// <param name="index">-1 if nothing found, else, it's the index of the item in the list</param>
        /// <returns></returns>
        public static bool ContainIndex<T>(this List<T> collection, T item, ref int index)
        {
            index = -1;
            for (int i = 0; i < collection.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(collection[i], item))
                {
                    index = i;
                    return (true);
                }
            }
            return (false);
        }

        /// <summary>
        /// Add an item only if it now exist in the list
        /// </summary>
        /// <typeparam name="T">type of item in the list</typeparam>
        /// <param name="list">list to add</param>
        /// <param name="item">item to add if no exit in the list</param>
        /// <returns>return true if we added the item</returns>
        public static bool AddIfNotContain<T>(this List<T> list, T item)
        {
            if (item == null)
            {
                return (false);
            }

            if (!list.Contains(item))
            {
                list.Add(item);
                return (true);
            }
            return (false);
        }
    }
}
