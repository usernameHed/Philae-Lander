using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor
{
    /// <summary>
    /// For a given key (like SCENE_VIEW_LISTER),
    /// we have a list of guid saved with JSON.
    /// 
    /// we can save a new asset into this list, or get it
    /// with the correct key
    /// </summary>
    public class ExtSaveAssetGUID
    {
        private const string KEY_OF_ADDITIONNAL_DATA = " KEY_OF_ADDITIONNAL_DATA";

        /// <summary>
        /// Container struct for the List
        /// </summary>
        [Serializable]
        public struct ListContainer
        {
            public List<string> List;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_dataList">Data list value</param>
            public ListContainer(List<string> dataList)
            {
                List = dataList;
            }
        }

        /// <summary>
        /// save asset guid in the list represented by the key.
        /// if there is no list yet, create it
        /// </summary>
        /// <param name="asset">asset to save</param>
        /// <param name="key">key that contain the list of assets</param>
        /// <param name="additionnalJsonData">any additional data associated with that asset (must be serializable to JSON)</param>
        /// <returns></returns>
        public static bool SaveAssetReference(UnityEngine.Object asset, string key, string additionnalJsonData)
        {
            ListContainer guidAssets;
            ListContainer guidAssetsAdditionnalData;
            if (EditorPrefs.HasKey(key))
            {
                guidAssets = JsonUtility.FromJson<ListContainer>(EditorPrefs.GetString(key));
                guidAssetsAdditionnalData = JsonUtility.FromJson<ListContainer>(EditorPrefs.GetString(key + KEY_OF_ADDITIONNAL_DATA));
            }
            else
            {
                guidAssets = new ListContainer(new List<string>(1));
                guidAssetsAdditionnalData = new ListContainer(new List<string>(1));
            }

            //if this object is already saved, just update the additionnalData
            string currentGuidAsset = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
            if (ContainIndex(guidAssets.List, currentGuidAsset, out int index))
            {
                guidAssetsAdditionnalData.List[index] = additionnalJsonData;
            }
            else
            {
                guidAssets.List.Add(currentGuidAsset);
                guidAssetsAdditionnalData.List.Add(additionnalJsonData);
            }

            string jsonData = JsonUtility.ToJson(guidAssets);
            string jsonAdditionalData = JsonUtility.ToJson(guidAssetsAdditionnalData);
            EditorPrefs.SetString(key, jsonData);
            EditorPrefs.SetString(key + KEY_OF_ADDITIONNAL_DATA, jsonAdditionalData);
            return (true);
        }


        public static bool ContainIndex<T>(List<T> collection, T item, out int index)
        {
            index = -1;
            for (int i = 0; i < collection.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(collection[i], item))
                {
                    index = i;
                    return (true);
                }
            }
            return (false);
        }

        public static List<UnityEngine.Object> GetAssets(string key, out List<string> additionalDatas)
        {
            additionalDatas = new List<string>(0);
            if (!EditorPrefs.HasKey(key))
            {
                return (new List<UnityEngine.Object>(0));
            }

            ListContainer guidAssets = JsonUtility.FromJson<ListContainer>(EditorPrefs.GetString(key));
            ListContainer additionalDatasContainer = JsonUtility.FromJson<ListContainer>(EditorPrefs.GetString(key + KEY_OF_ADDITIONNAL_DATA));
            List<UnityEngine.Object> savedAsset = new List<UnityEngine.Object>(guidAssets.List.Count);

            for (int i = 0; i < guidAssets.List.Count; i++)
            {
                savedAsset.Add(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guidAssets.List[i])));
            }
            additionalDatas = additionalDatasContainer.List;
            return (savedAsset);
        }

        public static UnityEngine.Object GetAsset(string key, out string additionalData)
        {
            List<UnityEngine.Object> saved = GetAssets(key, out List<string> additionalDatas);
            if (saved.Count == 0)
            {
                additionalData = "";
                return (null);
            }
            additionalData = additionalDatas[0];
            return (saved[0]);
        }

        public static T GetAsset<T>(string key, out string additionalData) where T : UnityEngine.Object
        {
            UnityEngine.Object saved = GetAsset(key, out additionalData);
            return (saved as T);
        }

        public static string GetAdditionalDataOfAsset(string key, UnityEngine.Object asset)
        {
            if (!EditorPrefs.HasKey(key))
            {
                return ("");
            }
            ListContainer guidAssets = JsonUtility.FromJson<ListContainer>(EditorPrefs.GetString(key));
            ListContainer additionalDatasContainer = JsonUtility.FromJson<ListContainer>(EditorPrefs.GetString(key + KEY_OF_ADDITIONNAL_DATA));
            for (int i = 0; i < guidAssets.List.Count; i++)
            {
                UnityEngine.Object objectSaved = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guidAssets.List[i]));
                if (objectSaved == null)
                {
                    continue;
                }
                if (objectSaved.GetInstanceID() == asset.GetInstanceID())
                {
                    return (additionalDatasContainer.List[i]);
                }
            }
            return ("");
        }

        public static void ResetKey(string key)
        {
            EditorPrefs.DeleteKey(key);
            EditorPrefs.DeleteKey(key + KEY_OF_ADDITIONNAL_DATA);
        }
    }
}