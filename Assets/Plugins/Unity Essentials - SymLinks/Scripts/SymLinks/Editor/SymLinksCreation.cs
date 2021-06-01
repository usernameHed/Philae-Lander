using UnityEditor;
using System.IO;
using UnityEssentials.symbolicLinks.extensions.editor;
using UnityEssentials.symbolicLinks.extensions;

namespace UnityEssentials.symbolicLinks
{
    /// <summary>
    /// An editor utility for easily creating symlinks in your project.
    /// 
    /// Adds a Menu item under `Assets/Create/Folder(Symlink)`, and
    /// draws a small indicator in the Project view for folders that are
    /// symlinks.
    /// </summary>
    public static class SymLinksCreation
    {
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;
        private const string PATH_JONCTION_FUNCTIONS = "Assets/Create/UnityEssentials/SymLink/";

        /// <summary>
        /// add a folder link at the current selection in the project view
        /// </summary>
        [MenuItem(PATH_JONCTION_FUNCTIONS + "Add new external folder link", false, 20)]
        private static void AddJonction()
		{
            AddLinkByType("/C mklink /J");
        }

        private static void AddLinkByType(string linkType)
        {
            string targetPath = GetSelectedPathOrFallback();

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            string sourceFolderPath = "";
            string sourceFolderName = "";
            ExtWindowComands.OpenFolderPanel(ref sourceFolderPath, ref sourceFolderName);

            targetPath = targetPath + "/" + sourceFolderName;

            if (Directory.Exists(targetPath))
            {
                return;
            }

            string commandeLine = linkType + " \"" + targetPath + "\" \"" + sourceFolderPath + "\"";
            ExtWindowComands.Execute(commandeLine);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            ExtSymLinks.ResetSymLinksDatas();
        }

        [MenuItem(PATH_JONCTION_FUNCTIONS + "Add new external folder link", true)]
        private static bool CheckAddJonction()
        {
            string targetPath = GetSelectedPathOrFallback();

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            if (IsSymLinkFolder(targetPath))
            {
                return (false);
            }
            string pathLinkFound = "";
            if (IsFolderHasParentSymLink(targetPath, ref pathLinkFound))
            {
                return (false);
            }
            return (true);
        }

        /// <summary>
        /// Restore a lost link
        /// WARNING: it will override identical files
        /// </summary>
        [MenuItem(PATH_JONCTION_FUNCTIONS + "Relink this local folder with an external one", false, 20)]
        private static void RestoreJonction()
        {
            string targetPath = GetSelectedPathOrFallback();
            string directoryName = Path.GetFileName(targetPath);

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            string sourceFolderPath = "";
            string sourceFolderName = "";
            ExtWindowComands.OpenFolderPanel(ref sourceFolderPath, ref sourceFolderName);

            if (string.IsNullOrEmpty(sourceFolderName))
            {
                return;
            }

            if (directoryName != sourceFolderName)
            {
                throw new System.Exception("source and target have different names. This function is meant to re-link broken symlink!");
            }

            int choice = EditorUtility.DisplayDialogComplex("Restore SymLink",
                "This action will link the current local folder with an external one with the same name, " +
                "and will merge them together. " +
                    " (this action is meant to restore a previously broken link) " +
                    "When merging folders, if 2 files have the same names, " +
                    "you have to choose between keep the local file present in unity, " +
                    "or keep the file present on the external folder. " + 
                    "In Both case, if files with the same name have differents settings, you will override one and keep the other. " +
                    "You cannot undo this action.",
                "Merge and keep local files", "cancel procedure", "Merge and override with new files");

            if (choice == 1)
            {
                return;
            }

            try
            {
                //Place the Asset Database in a state where
                //importing is suspended for most APIs
                AssetDatabase.StartAssetEditing();

                string newPathCreated = "";
                ExtFileEditor.DuplicateDirectory(targetPath, ref newPathCreated);
                ExtFileEditor.DeleteDirectory(targetPath);
                string commandeLine = "/C mklink /J \"" + targetPath + "\" \"" + sourceFolderPath + "\"";
                ExtWindowComands.Execute(commandeLine);
                ExtFileEditor.CopyAll(new DirectoryInfo(newPathCreated), new DirectoryInfo(targetPath), choice == 2);
                ExtFileEditor.DeleteDirectory(newPathCreated);
            }
            finally
            {
                //By adding a call to StopAssetEditing inside
                //a "finally" block, we ensure the AssetDatabase
                //state will be reset when leaving this function
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            ExtSymLinks.ResetSymLinksDatas();
        }

        [MenuItem(PATH_JONCTION_FUNCTIONS + "Relink this local folder with an external one", true)]
        private static bool CheckRestoreJonction()
        {
            string targetPath = GetSelectedPathOrFallback();

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            return (!IsSymLinkFolder(targetPath));
        }


        [MenuItem(PATH_JONCTION_FUNCTIONS + "Remove link", false, 20)]
        private static void RemoveJonction()
        {
            string targetPath = GetSelectedPathOrFallback();
            string directoryName = Path.GetFileName(targetPath);

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            string commandeLine = "/C rmdir \"" + ExtPaths.ReformatPathForWindow(targetPath) + "\"";

            bool unLink = EditorUtility.DisplayDialog(
                "Remove SymLink",
                "Do you want to Remove the link ? This action will duplicate the current extern folder to the current place. " +
                "You cannot undo this action.",
                "Remove Link",
                "Cancel");

            if (unLink)
            {
                try
                {
                    //Place the Asset Database in a state where
                    //importing is suspended for most APIs
                    AssetDatabase.StartAssetEditing();

                    string newPathCreated = "";
                    ExtFileEditor.DuplicateDirectory(targetPath, ref newPathCreated);
                    ExtWindowComands.Execute(commandeLine);
                    ExtFileEditor.RenameDirectory(newPathCreated, directoryName, false);
                }
                finally
                {
                    //By adding a call to StopAssetEditing inside
                    //a "finally" block, we ensure the AssetDatabase
                    //state will be reset when leaving this function
                    AssetDatabase.StopAssetEditing();
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            ExtSymLinks.ResetSymLinksDatas();
        }

        [MenuItem(PATH_JONCTION_FUNCTIONS + "Remove link", true)]
        private static bool CheckRemoveJonction()
        {
            string targetPath = GetSelectedPathOrFallback();

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FileAttributes.Directory) != FileAttributes.Directory)
            {
                targetPath = Path.GetDirectoryName(targetPath);
            }

            return (IsSymLinkFolder(targetPath));
        }

        #region UseFull Function

        public static bool IsFolderHasParentSymLink(string pathFolder, ref string pathSymLinkFound)
        {
            pathSymLinkFound = "";
            
            while (!string.IsNullOrEmpty(pathFolder))
            {
                string directoryName = Path.GetDirectoryName(pathFolder);

                if (IsSymLinkFolder(directoryName))
                {
                    pathSymLinkFound = directoryName;
                    return (true);
                }
                pathFolder = directoryName;
            }

            return (false);
        }

        public static bool IsSymLinkFolder(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
            {
                return (false);
            }

            FileAttributes attribs = File.GetAttributes(targetPath);

            if ((attribs & FOLDER_SYMLINK_ATTRIBS) != FOLDER_SYMLINK_ATTRIBS)
            {
                return (false);
            }
            else
            {
                return (true);
            }
        }

        /// <summary>
        /// return the path of the current selected folder (or the current folder of the asset selected)
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return (ExtPaths.ReformatPathForUnity(path));
        }
        #endregion
    }
}
