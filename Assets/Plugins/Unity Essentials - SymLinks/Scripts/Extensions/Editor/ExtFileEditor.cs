using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.symbolicLinks.extensions.editor
{
    public static class ExtFileEditor
    {
        public static void CreateEntirePathIfNotExist(string path)
        {
            string[] directories = path.Split('/');
            string finalPath = "";
            for (int i = 0; i < directories.Length - 1; i++)
            {
                finalPath += directories[i] + "/";
                CreateDirectoryIfNotExist(finalPath);
            }
        }

        /// <summary>
        /// Creates a directory at <paramref name="folder"/> if it doesn't exist
        /// </summary>
        /// <param name="folder">true if we created a new directory</param>
        public static bool CreateDirectoryIfNotExist(this string folder, bool refreshProject = false)
        {
            bool directoryCreated = ExtFile.CreateDirectoryIfNotExist(folder);
            if (directoryCreated && refreshProject)
            {
                AssetDatabase.Refresh();
            }
            return (directoryCreated);
        }

        /// <summary>
        /// duplicate a directory at the same location, with an incremental name
        /// </summary>
        /// <param name="pathDirectoryToDuplicate"></param>
        /// <returns></returns>
        public static bool DuplicateDirectory(string pathDirectoryToDuplicate, ref string newPathDirectory)
        {
            int index = 0;
            newPathDirectory = ExtPaths.RenameIncrementalFile(pathDirectoryToDuplicate, ref index, false);
            return (ExtFileEditor.DuplicateDirectory(pathDirectoryToDuplicate, newPathDirectory));
        }

        /// <summary>
        /// duplicate a directory
        /// from Assets/to/duplicate
        /// to Assets/new/location
        /// </summary>
        /// <param name="pathDirectoryToDuplicate">directory path to duplicate</param>
        /// <param name="newPathDirectory">new direcotry path</param>
        /// <param name="incrementDirectoryNameIfNeeded">do we increment the new duplicated name of the folder if folder already exist ?</param>
        /// <returns>true if succed, false if not duplicated</returns>
        public static bool DuplicateDirectory(string pathDirectoryToDuplicate, string newPathDirectory, bool incrementDirectoryNameIfNeeded = true, bool refreshDataBase = true)
        {
            if (ExtFile.IsDirectiryExist(newPathDirectory))
            {
                if (incrementDirectoryNameIfNeeded)
                {
                    int index = 0;
                    newPathDirectory = ExtPaths.RenameIncrementalFile(newPathDirectory, ref index, false);
                }
                else
                {
                    return (false);
                }
            }
            FileUtil.CopyFileOrDirectory(pathDirectoryToDuplicate, newPathDirectory);

            if (refreshDataBase)
            {
                AssetDatabase.Refresh();
            }
            return (true);
        }

        public static bool DeleteDirectory(string pathOfDirectory)
        {
            return (FileUtil.DeleteFileOrDirectory(pathOfDirectory));
        }

        /// <summary>
        /// rename an asset, and return the name
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string RenameDirectory(string oldPath, string newName, bool refreshAsset)
        {
            if (ExtFile.IsFileExist(oldPath))
            {
                throw new System.Exception("a directory must be given, not a file");
            }

            string pathWhitoutName = Path.GetDirectoryName(oldPath);
            pathWhitoutName = ExtPaths.ReformatPathForUnity(pathWhitoutName);
            string newWantedName = ExtPaths.ReformatPathForUnity(newName);

            FileUtil.MoveFileOrDirectory(oldPath, pathWhitoutName + "/" + newWantedName);
            if (refreshAsset)
            {
                AssetDatabase.Refresh();
            }
            string newPath = pathWhitoutName + "/" + newWantedName;
            return (newPath);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, bool overrideFile)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                string destinationPath = Path.Combine(target.ToString(), fi.Name);
                destinationPath = ExtPaths.ReformatPathForWindow(destinationPath);
                if (ExtFile.IsFileExist(destinationPath) && overrideFile)
                {
                    continue;
                }
                fi.CopyTo(destinationPath, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, overrideFile);
            }
        }
    }
}