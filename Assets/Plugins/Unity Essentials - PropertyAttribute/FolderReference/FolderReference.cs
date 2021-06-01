using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.Folder
{
    /// <summary>
    /// Represent a folder reference
    /// </summary>
    [Serializable]
    public class FolderReference
    {
        /// <summary>
        /// strong link to file
        /// </summary>
        public string Guid;
        /// <summary>
        /// Assets/path/
        /// </summary>
        public string RealPath;
        /// <summary>
        /// Asset/path
        /// </summary>
        public string RealPathWithoutFinalSlash;

        public FolderReference(string defaultPath = "Asset")
        {
            RealPath = defaultPath + "/";
            RealPathWithoutFinalSlash = defaultPath;
        }
    }
}