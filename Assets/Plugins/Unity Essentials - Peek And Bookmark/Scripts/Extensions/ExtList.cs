using System.Collections.Generic;

namespace UnityEssentials.Peek.Extensions
{
    /// <summary>
    /// extension of list
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

        /// <summary>
        /// remove all occurence of an item in a list
        /// </summary>
        /// <typeparam name="T">type of items</typeparam>
        /// <param name="list">list where to remove items</param>
        /// <param name="item">item to remove multiple times</param>
        /// <param name="removeAllOccurence">true: remove multiple, else, remove once</param>
        /// <returns>true if removed something</returns>
        public static bool Remove<T>(this List<T> list, T item, bool removeAllOccurence)
        {
            if (!removeAllOccurence)
            {
                return (list.Remove(item));
            }
            bool removed = false;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] != null && list[i].Equals(item))
                {
                    list.RemoveAt(i);
                    removed = true;
                }
            }
            return (removed);
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
        /// Add in a list some new items
        /// </summary>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// return true if the 2 list have the same content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool AreListHaveSameContent<T>(List<T> first, List<T> second)
        {
            if (first.Count != second.Count)
            {
                return (false);
            }
            for (int i = 0; i < first.Count; i++)
            {
                if (!Equals(first[i], second[i]))
                {
                    return (false);
                }
            }
            return (true);
        }

        public static T[] CompactToArray<T>(List<T> listA, List<T> listB)
        {
            T[] array = new T[listA.Count + listB.Count];
            int currentIndex = 0;
            for (int i = 0; i < listA.Count; i++)
            {
                array[currentIndex] = listA[i];
                currentIndex++;
            }
            for (int i = 0; i < listB.Count; i++)
            {
                array[currentIndex] = listB[i];
                currentIndex++;
            }
            return (array);
        }

        /// <summary>
        /// Clean  null item (do not remove items, remove only the list)
        /// </summary>
        /// <param name="listToClean"></param>
        /// <returns>true if list changed</returns>
        public static List<T> CleanNullFromList<T>(this List<T> listToClean, ref bool hasChanged)
        {
            hasChanged = false;
            if (listToClean == null)
            {
                return (listToClean);
            }
            for (int i = listToClean.Count - 1; i >= 0; i--)
            {
                if (listToClean[i] == null || listToClean[i].Equals(null))
                {
                    listToClean.RemoveAt(i);
                    hasChanged = true;
                }
            }
            return (listToClean);
        }

        /// <summary>
        /// pick a random number in array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T PickRandom<T>(this List<T> collection)
        {
            return (collection[UnityEngine.Random.Range(0, collection.Count)]);
        }
    }
}
