using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Extensions
{
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

        public static int ContainSubStringInArray(string[] toTransform, string fileName)
        {
            for (int i = 0; i < toTransform.Length; i++)
            {
                if (fileName.Contains(toTransform[i]))
                    return (i);
            }
            return (-1);
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
        /// from a current list, append a second list at the end of the first list
        /// </summary>
        /// <typeparam name="T">type of content in the lists</typeparam>
        /// <param name="currentList">list where we append stuffs</param>
        /// <param name="listToAppends">list to append to the other</param>
        public static void AppendList<T>(this List<T> currentList, List<T> listToAppends)
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

        /// <summary>
        /// when we are looping in an array, determine if we are at the end loop of it
        /// </summary>
        /// <typeparam name="T">type of the array</typeparam>
        /// <param name="collection">current array</param>
        /// <param name="i">index we currently are</param>
        /// <returns>true if we are at the end loop</returns>
        public static bool AreWeAtLastLoop<T>(this List<T> collection, int i)
        {
            return (i + 1 >= collection.Count);
        }

        /// <summary>
        /// return true if the index is inside the list (and the list is not null)
        /// </summary>
        /// <typeparam name="T">type of list</typeparam>
        /// <param name="collection">list</param>
        /// <param name="index">index</param>
        /// <returns>true if valid</returns>
        public static bool IsValideIndex<T>(this List<T> collection, int index)
        {
            return (collection != null && index >= 0 && index < collection.Count);
        }

        public static bool ContainIndex<T>(this List<T> collection, T item, out int index)
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
        /// return the index of the element inside a list
        /// return -1 if not found in list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetFirstIndexOfElementInList<T>(List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                return (-1);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(list[i], item))
                {
                    return (i);
                }
            }
            return (-1);
        }

        /// <summary>
        /// Attempt to add each element, and return the sum
        /// </summary>
        /// <typeparam name="T">type of parametter, float, int, double...</typeparam>
        /// <param name="list">list of elemetns</param>
        /// <returns>sum of all the elements in the list</returns>
        public static float Sum(this List<float> list)
        {
            float sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }
            return (sum);
        }


        /// <summary>
        /// is this list contain a string (nice for enum test)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="listEnum"></param>
        /// <returns></returns>
        public static bool ListContain<T>(this List<T> listEnum, string tag)
        {
            for (int i = 0; i < listEnum.Count; i++)
            {
                if (String.Equals(listEnum[i].ToString(), tag))
                    return (true);
            }
            return (false);
        }

        /// <summary>
        /// Remove occurence of OtherListReference inside the ListToClean
        /// 
        /// from a listToClean:
        /// A, E, Z, T
        /// and a OtherListReference:
        /// R, A, F, C
        /// 
        /// return the listToClean like that:
        /// E, Z, T
        /// </summary>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="listToClean">list to clean</param>
        /// <param name="OtherListReference">other list</param>
        public static List<T> RemoveOccurenceFromOtherList<T>(List<T> listToClean, List<T> OtherListReference)
        {
            for (int i = listToClean.Count - 1; i >= 0; i--)
            {
                if (OtherListReference.Contains(listToClean[i]))
                {
                    listToClean.RemoveAt(i);
                }
            }
            return (listToClean);
        }

        /// <summary>
        /// return a list without doublon
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> RemoveRedundancy<T>(List<T> list)
        {
            List<T> newList = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (newList.Contains(list[i]))
                {
                    continue;
                }
                newList.Add(list[i]);
            }
            return (newList);
        }

        public static bool IsThereNullInList<T>(this List<T> listToClean)
        {
            for (int i = 0; i < listToClean.Count; i++)
            {
                if (listToClean[i] == null || listToClean[i].ToString() == "null")
                {
                    return (true);
                }
            }
            return (false);
        }

        public static List<T> GetCopyOf<T>(this List<T> listToCopy)
        {
            List<T> newList = new List<T>();
            for (int i = 0; i < listToCopy.Count; i++)
            {
                newList.Add(listToCopy[i]);
            }
            return (newList);
        }

        /// <summary>
        /// Add an item only if it now exist in the list
        /// </summary>
        /// <typeparam name="T">type of item in the list</typeparam>
        /// <param name="list">list to add</param>
        /// <param name="item">item to add if no exit in the list</param>
        /// <returns>return true if we added the item</returns>
        public static bool AddIfNotContain<T>(this List<T> list, List<T> items)
        {
            bool added = false;
            if (items == null)
            {
                return (added);
            }

            List<T> copyFirst = list.GetCopyOf();
            for (int i = 0; i < items.Count; i++)
            {
                if (copyFirst.Contains(items[i]))
                {
                    continue;
                }
                added = true;
                list.Add(items[i]);
            }
            return (added);
        }

        /// <summary>
        /// true return if in the list, there is a word that is substring of the fileName
        /// </summary>
        /// <param name="toTransform"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static int ContainSubStringInList(List<string> toTransform, string fileName)
        {
            for (int i = 0; i < toTransform.Count; i++)
            {
                if (fileName.Contains(toTransform[i]))
                    return (i);
            }
            return (-1);
        }



        /// <summary>
        /// return a list of all child of this transform (depth 1 only)
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static List<Transform> GetListFromChilds(this Transform parent)
        {
            List<Transform> allChild = new List<Transform>();
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                allChild.Add(parent.transform.GetChild(i));
            }
            return (allChild);
        }

        // Function which pushes all zeros  
        // to end of an array. 
        public static void PushAllValueToEnd(int[] arr, int valueToPush)
        {
            // Count of non-zero elements 
            int count = 0;
            int lenghtArray = arr.Length;

            // Traverse the array. If element encountered is 
            // non-zero, then replace the element  
            // at index â..countâ.. with this element 
            for (int i = 0; i < lenghtArray; i++)
            {
                if (arr[i] != valueToPush)
                {
                    // here count is incremented 
                    arr[count++] = arr[i];
                }
            }

            // Now all non-zero elements have been shifted to 
            // front and â..countâ.. is set as index of first 0. 
            // Make all elements 0 from count to end. 
            while (count < lenghtArray)
            {
                arr[count++] = 0;
            }
        }

        /// <summary>
        /// bubble sort optimize algorythme
        /// </summary>
        public static List<float> BubbleSort(this List<float> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                bool sorted = true;
                for (int j = 0; j <= i - 1; j++)
                {
                    if (list[j + 1] < list[j])
                    {
                        list.Move(j + 1, j);
                        sorted = false;
                    }
                }
                if (sorted)
                    break;
            }
            return (list);
        }

        /// <summary>
        /// Shuffle the list in place using the Fisher-Yates method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            list.Sort((a, b) => 1 - 2 * UnityEngine.Random.Range(0, 1));
        }

        /// <summary>
        /// Return a random item from the list.
        /// Sampling with replacement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return (default(T));
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T RandomItem<T>(this List<T> list, T elementToExcludeFromRandom)
        {
            return (RandomItem(list, new List<T>() { elementToExcludeFromRandom }));
        }

        /// <summary>
        /// Return a random item from the list.
        /// Sampling with replacement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this List<T> list, List<T> elementToExcludeFromRandom)
        {
            if (list.Count == 0)
            {
                return (default(T));
            }
            List<T> tmpList = CopyListWithoutSomeElements(list, elementToExcludeFromRandom);
            T choosenElement = RandomItem(tmpList);
            return choosenElement;
        }

        /// <summary>
        /// create a new list, a copy of the listDataToCopy, without the elementToExclude
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listDatasToCopy"></param>
        /// <param name="elementToExclude"></param>
        /// <returns></returns>
        public static List<T> CopyListWithoutSomeElements<T>(List<T> listDatasToCopy, List<T> elementToExclude)
        {
            List<T> newList = new List<T>();
            for (int i = 0; i < listDatasToCopy.Count; i++)
            {
                if (elementToExclude != null && elementToExclude.Contains(listDatasToCopy[i]))
                {
                    continue;
                }
                newList.Add(listDatasToCopy[i]);
            }
            return (newList);
        }

        /// <summary>
        /// Removes a random item from the list, returning that item.
        /// Sampling without replacement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = UnityEngine.Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Returns true if the array is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this T[] data)
        {
            return ((data == null) || (data.Length == 0));
        }

        /// <summary>
        /// Returns true if the list is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this List<T> data)
        {
            return ((data == null) || (data.Count == 0));
        }

        /// <summary>
        /// Returns true if the dictionary is null or empty
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> data)
        {
            return ((data == null) || (data.Count == 0));
        }

        /// <summary>
        /// deques an item, or returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public static T DequeueOrNull<T>(this Queue<T> q)
        {
            try
            {
                return (q.Count > 0) ? q.Dequeue() : default(T);
            }

            catch (Exception)
            {
                return default(T);
            }
        }

        public static List<T> Fill<T>(this List<T> list, int size, T element)
        {
            list = new List<T>(size);
            for (int i = 0; i < size; i++)
            {
                list.Add(element);
            }
            return (list);
        }
    }
}