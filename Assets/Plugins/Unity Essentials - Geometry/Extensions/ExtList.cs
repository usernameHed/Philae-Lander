using System;
using System.Collections.Generic;

namespace UnityEssentials.Geometry.extensions
{
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
        /// move an item in a list, from oldIndex to newIndex
        /// </summary>
        public static List<T> Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            if (list == null)
                return (null);
            if (list.Count == 0)
                return (list);
            if (oldIndex >= list.Count || oldIndex < 0)
                return (list);
            if (newIndex >= list.Count || newIndex < 0)
                return (list);

            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
            return (list);
        }
    }
}