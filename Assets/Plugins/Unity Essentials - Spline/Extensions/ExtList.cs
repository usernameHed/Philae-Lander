using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions
{
    /// <summary>
    /// Rotation extensions
    /// </summary>
    public static class ExtList
    {
        /// <summary>
        /// transform an array into a list
        /// </summary>
        public static List<T> ToList<T>(this T[] array)
        {
            if (array == null)
            {
                return (new List<T>());
            }

            List<T> newList = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                newList.Add(array[i]);
            }
            return (newList);
        }

        /// <summary>
        /// from a current list, append a second list at the end of the first list
        /// </summary>
        /// <typeparam name="T">type of content in the lists</typeparam>
        /// <param name="currentList">list where we append stuffs</param>
        /// <param name="listToAppends">list to append to the other</param>
        public static void Append<T>(this IList<T> currentList, IList<T> listToAppends)
        {
            if (listToAppends == null)
            {
                return;
            }
            for (int i = 0; i < listToAppends.Count; i++)
            {
                currentList.Add(listToAppends[i]);
            }
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