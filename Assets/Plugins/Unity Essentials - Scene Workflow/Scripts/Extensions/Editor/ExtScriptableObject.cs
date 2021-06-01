using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityEssentials.SceneWorkflow.extensions.editor
{
    public static class ExtScriptableObject
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return (asset);
        }

        /// <summary>
        /// think to do AssetDatabase.Refresh(); after that
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        public static void Save<T>(this T asset) where T : UnityEngine.Object
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        //	Create an ScriptableObject, given a good path & name included
        /// </summary>
        public static T CreateAsset<T>(string pathToCreate) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = pathToCreate;
            if (path == "")
            {
                path = "Assets";
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(pathToCreate);

            Debug.Log("name: " + assetPathAndName);
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            return (asset);
        }

        /// <summary>
        //	From a given U asset, create an T asset (this asset T inherite from U)
        // then fill T with datas of U
        /// </summary>
        public static T DuplicateChildToParentWithInheritance<T, U>(U otherAsset, string infoAsset, string directoryChild) where T : ScriptableObject where U : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(otherAsset);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(otherAsset)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + directoryChild + "/" + otherAsset.name + infoAsset + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return (asset);
        }
    }

}