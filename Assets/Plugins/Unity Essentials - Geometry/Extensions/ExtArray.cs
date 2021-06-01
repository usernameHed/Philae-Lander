using System;
using System.Collections.Generic;

namespace UnityEssentials.Geometry.extensions
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
    }
}