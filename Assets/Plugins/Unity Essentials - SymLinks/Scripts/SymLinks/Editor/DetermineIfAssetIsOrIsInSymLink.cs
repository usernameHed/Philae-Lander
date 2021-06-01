using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials.symbolicLinks.extensions;

namespace UnityEssentials.symbolicLinks
{
    /// <summary>
    /// Determine if a file is a symlink folder, or if the file is inside a symlink
    /// </summary>
    public static class DetermineIfAssetIsOrIsInSymLink
    {
        private const FileAttributes FOLDER_SYMLINK_ATTRIBS = FileAttributes.Directory | FileAttributes.ReparsePoint;
        private const FileAttributes FILE_SYMLINK_ATTRIBS = FileAttributes.Directory & FileAttributes.Archive;

        /// <summary>
        /// Determine if a file is a symlink folder, or if the file is inside a symlink
        /// </summary>
        /// <param name="path">path of tile</param>
        /// <param name="attribs">attribute of file, get it from a path with File.GetAttributes(path);</param>
        /// <param name="allSymLinksAssetPathSaved">list of symlinks folder saved</param>
        /// <returns>true if this file is a symlink, or if the file is inside a symlink</returns>
        public static bool IsAttributeAFileInsideASymLink(string path, FileAttributes attribs, ref List<string> allSymLinksAssetPathSaved)
        {
            return (attribs & FILE_SYMLINK_ATTRIBS) == FILE_SYMLINK_ATTRIBS && path.ContainIsPaths(allSymLinksAssetPathSaved);
        }

        /// <summary>
        /// is this attribute associated with symlink folder ?
        /// </summary>
        /// <param name="attribs">attribute of file, get it from a path with File.GetAttributes(path);</param>
        /// <returns>true if this attribute is associated with symlink folder</returns>
        public static bool IsAttributeASymLink(FileAttributes attribs)
        {
            return (attribs & FOLDER_SYMLINK_ATTRIBS) == FOLDER_SYMLINK_ATTRIBS;
        }

        /// <summary>
        /// from a given path, loop thought all the parent directory,
        /// if a given parent (or itself), si a SymLink Folder, add it to the 
        /// given list, and return
        /// </summary>
        /// <param name="path">current direcory to test</param>
        /// <param name="allSymLinksAssetPathSaved">list of path of directory saved</param>
        public static void UpdateSymLinksParent(string path, ref List<string> allSymLinksAssetPathSaved)
        {
            while (!string.IsNullOrEmpty(path))
            {
                FileAttributes attribs = File.GetAttributes(path);
                if (IsAttributeASymLink(attribs))
                {
                    allSymLinksAssetPathSaved.AddIfNotContain(path);
                    return;
                }
                path = ExtPaths.GetDirectoryFromCompletPath(path);
            }
        }
    }
}
