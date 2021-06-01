using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.symbolicLinks.extensions
{
    /// <summary>
    /// extension of file
    /// </summary>
    public static class ExtFile
    {
        public static bool IsFileExist(string path)
        {
            return (File.Exists(path));
        }

        /// <summary>
        /// return true if the directory exist
        /// </summary>
        /// <param name="pathDirectory"></param>
        /// <returns>true if directory exist</returns>
        public static bool IsDirectiryExist(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return (false);
            }
            string path = Path.GetDirectoryName(folder);
            if (string.IsNullOrEmpty(path))
            {
                return (false);
            }
            bool directoryExist = Directory.Exists(folder);
            return (directoryExist);
        }

        /// <summary>
        /// Creates a directory at <paramref name="folder"/> if it doesn't exist
        /// </summary>
        /// <param name="folder">true if we created a new directory</param>
        public static bool CreateDirectoryIfNotExist(this string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return (false);
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                return (true);
            }
            return (false);
        }
    }
}