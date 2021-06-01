using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.ActionTrigger.Extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtList
    {
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
