using UnityEngine;

namespace UnityEssentials.Peek.Extensions
{
    /// <summary>
    /// extension of paths
    /// </summary>
    public static class ExtPaths
    {
        /// <summary>
        /// from an absolutePath, return the relative Path
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string ConvertAbsoluteToRelativePath(string absolutePath)
        {
            string gameDataFolder = Application.dataPath;
            absolutePath = ExtPaths.ReformatPathForUnity(absolutePath);

            if (absolutePath.StartsWith(gameDataFolder))
            {
                string relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                return (relativepath);
            }
            return (absolutePath);
        }

        /// <summary>
        /// change a path from
        /// Assets\path\of\file
        /// to
        /// Assets/path/of/file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReformatPathForUnity(string path, char characterReplacer = '-')
        {
            if (path == null)
            {
                return ("");
            }
            string formattedPath = path.Replace('\\', '/');
            formattedPath = formattedPath.Replace('|', characterReplacer);
            return (formattedPath);
        }
    }
}
