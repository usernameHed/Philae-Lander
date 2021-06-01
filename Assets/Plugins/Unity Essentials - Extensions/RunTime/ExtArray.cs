using System;
using System.Collections.Generic;

namespace UnityEssentials.Extensions
{
    public static class ExtArray
    {
        /// <summary>
        /// do not use, it's slow ! only in editor
        /// </summary>
        /// <typeparam name="T">type inside the array</typeparam>
        /// <param name="array">array to add</param>
        /// <param name="itemToAdd">item to add</param>
        /// <returns>return the new array</returns>
        public static T[] Add<T>(T[] array, T itemToAdd)
        {
            T[] newArray = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            newArray[array.Length] = itemToAdd;
            return (newArray);
        }

        /// <summary>
        /// return true if the index is inside the list (and the list is not null)
        /// </summary>
        /// <typeparam name="T">type of list</typeparam>
        /// <param name="collection">list</param>
        /// <param name="index">index</param>
        /// <returns>true if valid</returns>
        public static bool IsValidIndex<T>(this T[] collection, int index)
        {
            return (collection != null && index >= 0 && index < collection.Length);
        }

        /// <summary>
        /// from a givne percent, from 0 to 1,
        /// get the index corresponding
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">list</param>
        /// <param name="percent">from 0 to 1</param>
        /// <returns>index found from percent</returns>
        public static int GetIndexFromPercent<T>(this T[] collection, float percent)
        {
            float remapped = ExtMathf.Remap(percent, 0, 1, 0, collection.Length);
            int index = (int)remapped;
            return (index);
        }

        public static U[] ArrayCombine<U>(U[] array, U[] array2)
        {
            U[] total = new U[array.Length + array2.Length];
            int i = 0;
            for (i = 0; i < array.Length; i++)
            {
                total[i] = array[i];
            }
            for (int k = 0; k < array2.Length; k++)
            {
                total[i + k] = array2[k];
            }
            return (total);
        }

        public static bool Contain<U>(this U[] array, U item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<U>.Default.Equals(array[i], item))
                {
                    return (true);
                }
            }
            return (false);
        }
    }
}