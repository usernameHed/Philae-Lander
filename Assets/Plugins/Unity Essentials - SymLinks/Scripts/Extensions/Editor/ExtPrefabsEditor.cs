using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEssentials.symbolicLinks.extensions.editor
{
    public static class ExtPrefabsEditor
    {
        /// <summary>
        /// is this GameObject a prefabs ?
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPrefab(GameObject obj)
        {
#if UNITY_2018_3_OR_NEWER
            GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
#else
            GameObject prefab = PrefabUtility.GetPrefabParent(obj) as GameObject;
#endif
            return prefab != null && prefab == obj;
        }

        public static bool IsPrefab(GameObject obj, ref GameObject prefab)
        {
#if UNITY_2018_3_OR_NEWER
            prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
            return prefab != null && prefab == obj;
#else
            prefab = PrefabUtility.FindPrefabRoot(obj) as GameObject;
            return (prefab != null);
#endif
        }
    }
}