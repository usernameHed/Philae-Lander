using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEssentials.SceneWorkflow.extension
{
    public static class ExtPaths
    {
        /// <summary>
        /// take a path of a file, increment his number in his name, test if it exist, and return the new path
        /// 
        /// use: string newFilePath = ExtString.RenameIncrementalFile(AssetDatabase.GetAssetPath(assetObject));
        /// if you want to have only the name after, do:
        /// string only name = Path.GetFileName(newFilePath);
        /// 
        /// return the renamed file asset like:
        /// Asset/Folder/myFileAsset.asset      ->  Asset/Folder/myFileAsset 1.asset
        /// Asset/Folder/myFileAsset 0.asset    ->  Asset/Folder/myFileAsset 1.asset
        /// Asset/Folder/myFileAsset 58.asset    ->  Asset/Folder/myFileAsset 59.asset
        /// Asset/Folder/myFileAsset58.asset    ->  Asset/Folder/myFileAsset 59.asset
        /// asset 24.asset
        /// </summary>
        /// <param name="pathFileToRename">path of the file</param>
        /// <param name="index">new index calculated</param>
        /// <returns></returns>
        public static string RenameIncrementalFile(string pathFileToRename, out int index, bool setZeroIfNothingFound)
        {
            string nameAsset = Path.GetFileNameWithoutExtension(pathFileToRename);
            int numberPrevious = ExtString.ExtractIntFromEndOfString(nameAsset);

            string nameWithoutNumber = nameAsset.Replace(numberPrevious.ToString(), "");
            nameWithoutNumber = nameWithoutNumber.TrimEnd(' ');

            string dirName = Path.GetDirectoryName(pathFileToRename).Replace('\\', '/') + '/';

            string newName = "";
            string finalRenamedPath = "";
            index = numberPrevious;

            do
            {
                if (index == 0)
                {
                    if (setZeroIfNothingFound)
                    {
                        newName = nameWithoutNumber + " " + (index);
                    }
                    else
                    {
                        newName = nameWithoutNumber;
                    }
                }
                else
                {
                    newName = nameWithoutNumber + " " + (index);
                }
                finalRenamedPath = dirName + newName + Path.GetExtension(pathFileToRename);
                index++;
            }
            while (File.Exists(finalRenamedPath) || Directory.Exists(finalRenamedPath));
            return (finalRenamedPath);
        }

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
        /// from Assets/some/path/assetName.asset
        /// to: Assets/some/path/
        /// </summary>
        /// <returns></returns>
        public static string GetDirectoryFromCompletPath(string completPath)
        {
            string nameFile = Path.GetDirectoryName(completPath);
            return (ReformatPathForUnity(nameFile));
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
            string formattedPath = path.Replace('\\', '/');
            formattedPath = formattedPath.Replace('|', characterReplacer);
            return (formattedPath);
        }

        /// <summary>
        /// change a path from
        /// Assets/path/of/file
        /// to
        /// Assets\path\of\file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReformatPathForWindow(string path)
        {
            string formattedPath = path.Replace('/', '\\');
            formattedPath = formattedPath.Replace('|', '-');
            return (formattedPath);
        }

        /// <summary>
        /// is subPath is contained in a list of given path
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static bool ContainSubStringInList(this List<string> paths, string subPath)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                if (paths[i].Contains(subPath))
                {
                    return (true);
                }
            }
            return (false);
        }

        /// <summary>
        /// is subPath is containted in any of the list of paths ?
        /// </summary>
        /// <param name="subPath"></param>
        /// <param name="paths"></param>
        /// <returns>true if subPath exist in at least one of the list of paths</returns>
        public static bool ContainIsPaths(this string subPath, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                if (subPath.Contains(paths[i]))
                {
                    return (true);
                }
            }
            return (false);
        }
    }
}