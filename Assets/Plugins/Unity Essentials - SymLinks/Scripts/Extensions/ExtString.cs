using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEssentials.symbolicLinks.extensions
{
    /// <summary>
    /// extension of strings
    /// </summary>
    public static class ExtString
    {
        /// <summary>
        /// from a given string, like: RaceTrack 102
        /// extract the number 102 (int)
        /// </summary>
        /// <param name="input"></param>
        /// <returns>number in the end of string</returns>
        public static int ExtractIntFromEndOfString(string input)
        {
            var stack = new Stack<char>();
            for (var i = input.Length - 1; i >= 0; i--)
            {
                if (!char.IsNumber(input[i]))
                {
                    break;
                }

                stack.Push(input[i]);
            }

            string result = new string(stack.ToArray());
            return (ExtString.ToInt(result));
        }

        /// <summary>
        /// Converts a string to an int
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="defaultValue">default value if could not convert</param>
        private static int ToInt(string value, int defaultValue = 0)
        {
            // exit if null
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            // convert
            int rVal;
            return int.TryParse(value, out rVal) ? rVal : defaultValue;
        }
    }
}