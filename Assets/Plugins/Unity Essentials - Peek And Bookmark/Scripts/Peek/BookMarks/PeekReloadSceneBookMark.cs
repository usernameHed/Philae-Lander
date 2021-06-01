using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEssentials.Peek.Extensions;
using UnityEssentials.Peek.Extensions.Editor;

namespace UnityEssentials.Peek
{
    public class PeekReloadSceneBookMark
    {
        private List<Transform> _gameObjectsList = new List<Transform>(300);

        private string _sceneNameOfGameObjectToSelect;
        private string _gameObjectNametoSelect;
        private bool _mustSearchForGameObject;

        public void Init()
        {
            _mustSearchForGameObject = false;
        }

        public void AutomaticlySelectWhenFound(string toSelectName, string sceneName)
        {
            _gameObjectNametoSelect = toSelectName;
            _sceneNameOfGameObjectToSelect = sceneName;
            _mustSearchForGameObject = true;
        }

        /// <summary>
        /// Attempt to relink all gameObject found in current scene
        /// with bookMarks with by their [Scene name + name]
        /// </summary>
        /// <param name="scene"></param>
        public void ReloadSceneFromOneLoadedScene(Scene scene)
        {
            _gameObjectsList.Clear();
            AppendAllTransformInSceneToList(scene);
            RelinkNamesWithGameObjects(_gameObjectsList);
        }

        /// <summary>
        /// Append all Transform found in this scene into _gameObjectsList list
        /// </summary>
        /// <param name="scene"></param>
        public void AppendAllTransformInSceneToList(Scene scene)
        {
            if (!scene.IsValid())
            {
                return;
            }
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                Transform[] allChilds = roots[i].GetComponentsInChildren<Transform>(true);
                ExtList.Append<Transform>(_gameObjectsList, allChilds.ToList());
            }
        }

        /// <summary>
        /// Attempt to relink all gameObject
        /// with bookMarks with by their [Scene name + name]
        /// 
        /// Do a deep clean of list before!
        /// </summary>
        /// <param name="foundObjects"></param>
        public void RelinkNamesWithGameObjects(List<Transform> foundObjects)
        {
            if (foundObjects == null || PeekSaveDatas.Instance == null)
            {
                return;
            }
            CleanArraysFromDuplicateInNameAndObjectReferences(
                PeekSerializeObject.PeekSave.GetPropertie("_pinnedObjectsInScenes"),
                PeekSerializeObject.PeekSave.GetPropertie("_pinnedObjectsNameInScenes"),
                PeekSerializeObject.PeekSave.GetPropertie("_pinnedObjectsScenesLink"));

            for (int i = 0; i < PeekSerializeObject.PinnedObjectsInScenesCount; i++)
            {
                if (PeekSerializeObject.PinnedObjectsInScenesIndex(i) == null)
                {
                    for (int k = 0; k < foundObjects.Count; k++)
                    {
                        GameObject foundGameObject = foundObjects[k].gameObject;
                        if (string.Equals(
                            PeekSerializeObject.PinnedObjectsScenesLinkIndex(i).name + "/" + PeekSerializeObject.PinnedObjectsNameInScenesIndex(i),
                            foundGameObject.scene.name + "/" + foundGameObject.name))
                        {
                            PeekSerializeObject.PinnedObjectsInScenesList.GetArrayElementAtIndex(i).SetCustomObject(foundGameObject);
                            if (_mustSearchForGameObject
                                && string.Equals(foundGameObject.scene.name, _sceneNameOfGameObjectToSelect)
                                && string.Equals(foundGameObject.name, _gameObjectNametoSelect))
                            {
                                _mustSearchForGameObject = false;
                                PeekLogic.ForceSelection(foundGameObject);
                                SceneView.FrameLastActiveSceneView();
                            }
                        }
                    }
                }
            }
            PeekSerializeObject.Save(1f);
        }


        /// <summary>
        /// Deep clean of list, here an exemple: (see CleanTestFunction)
        /// list:
        ///     [NAME]  : [OBJ]
        /// 0   Yellow  : Object B      <- Process: Remove 3 & 7 (same object)
        /// 1   Yellow  : Object A      <- Process: Remove 2 (is null, not 4 because different Object)
        /// 2̶ ̶ ̶ ̶Y̶e̶l̶l̶o̶w̶ ̶ ̶:̶ ̶n̶u̶l̶l̶
        /// 3̶ ̶ ̶ ̶Y̶e̶l̶l̶o̶w̶ ̶ ̶:̶ ̶O̶b̶j̶e̶c̶t̶ ̶B̶
        /// 4   Yellow  : Object C      <- Process:
        /// 5   Red     : null          <- Process: Remove 6 (same name, both null)
        /// 6̶ ̶ ̶ ̶R̶e̶d̶ ̶ ̶ ̶ ̶ ̶:̶ ̶n̶u̶l̶l̶
        /// 7 ̶ ̶ ̶R̶e̶d̶ ̶ ̶ ̶ ̶ ̶:̶ ̶O̶b̶j̶e̶c̶t̶ ̶B̶
        /// 8̶ ̶ ̶ ̶G̶r̶e̶e̶n̶ ̶ ̶ ̶:̶ ̶n̶u̶l̶l̶ ̶ ̶ ̶ ̶ ̶     <- Process: remove itself 8 (same name, other has Object) break k loop
        /// 9   Green   : Object K      <- Process: 
        /// 10  Black   : Object Z      <- Process:
        /// </summary>
        public static void CleanArraysFromDuplicateInNameAndObjectReferences(SerializedProperty objectReferences, SerializedProperty pathObjects, SerializedProperty sceneLinkedReference)
        {
            if (objectReferences == null || pathObjects == null || objectReferences.arraySize != pathObjects.arraySize || objectReferences.arraySize == 0)
            {
                return;
            }

            int countArray = objectReferences.arraySize;
            for (int i = countArray - 1; i >= 0; i--)
            {
                if (i >= objectReferences.arraySize || i < 0)
                {
                    continue;
                }

                UnityEngine.Object current = objectReferences.GetArrayElementAtIndex(i).GetCustomObject();
                string currentName = pathObjects.GetArrayElementAtIndex(i).stringValue;

                if (string.IsNullOrEmpty(currentName))
                {
                    objectReferences.RemoveAt(i);
                    pathObjects.RemoveAt(i);
                    sceneLinkedReference.RemoveAt(i);
                    continue;
                }
                currentName = sceneLinkedReference.GetArrayElementAtIndex(i).GetCustomObject().name + "/" + currentName;

                for (int k = i - 1; k >= 0; k--)
                {
                    UnityEngine.Object toTest = objectReferences.GetArrayElementAtIndex(k).GetCustomObject();
                    string toTestName = sceneLinkedReference.GetArrayElementAtIndex(k).GetCustomObject().name + "/" + pathObjects.GetArrayElementAtIndex(k).stringValue;

                    if (Equals(current, toTest) && current != null && toTest != null)
                    {
                        objectReferences.RemoveAt(k);
                        pathObjects.RemoveAt(k);
                        sceneLinkedReference.RemoveAt(k);
                    } //1: Object A     2: Object A
                    else if (Equals(currentName, toTestName))                           //1: name A        2: name A
                    {
                        if (current != null && toTest == null)                                  //1: Object A      2: null
                        {
                            objectReferences.RemoveAt(k);
                            pathObjects.RemoveAt(k);
                            sceneLinkedReference.RemoveAt(k);
                            i--;
                        }
                        else if (current != null && toTest != null)                             //1: Object A       2: Object B
                        {
                            //nothing
                        }
                        else if (current == null && toTest != null)                             //1: null           2: ObjectA
                        {
                            objectReferences.RemoveAt(i);
                            pathObjects.RemoveAt(i);
                            sceneLinkedReference.RemoveAt(i);
                            break;
                        }
                        else if (current == null && toTest == null)                             //1: null           2: null
                        {
                            objectReferences.RemoveAt(k);
                            pathObjects.RemoveAt(k);
                            sceneLinkedReference.RemoveAt(k);
                            i--;
                        }
                    }
                }
            }
        }
        //end of class
    }
}