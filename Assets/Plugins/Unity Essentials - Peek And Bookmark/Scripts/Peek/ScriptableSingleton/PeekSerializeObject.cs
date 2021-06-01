using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEssentials.Peek.Extensions.Editor;
using UnityEssentials.Peek.Options;
using UnityEssentials.Peek.ScriptableObjectSingleton;
using UnityEssentials.Peek.ToolbarExtender;

namespace UnityEssentials.Peek
{
    ///<summary>
    /// Default paths for icons
    ///</summary>
    [InitializeOnLoad]
    public static class PeekSerializeObject
    {
        private static EditorChrono _waitBeforeSave = new EditorChrono();

        public static SerializedObject _peekSave;
        public static SerializedObject PeekSave
        {
            get
            {
                if (_peekSave == null)
                {
                    _peekSave = new SerializedObject(PeekSaveDatas.Instance);
                }
                return (_peekSave);
            }
        }

        #region save
        static PeekSerializeObject()
        {
            EditorApplication.update += AttemptToSave;
            EditorApplication.quitting += AttemptToSaveOnQuit;
        }

        private static void AttemptToSaveOnQuit()
        {
            if (_waitBeforeSave.IsRunning() || _waitBeforeSave.IsFinished())
            {
                PeekSave.ApplyModifiedPropertiesWithoutUndo();
                _waitBeforeSave.Reset();
            }
        }

        private static void AttemptToSave()
        {
            if (_waitBeforeSave.IsFinished())
            {
                PeekSave.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        public static void Save(float saveInXSeconds = 0)
        {
            if (saveInXSeconds > 0)
            {
                _waitBeforeSave.StartChrono(saveInXSeconds);
                return;
            }
            PeekSave.ApplyModifiedPropertiesWithoutUndo();
        }
        #endregion


        #region proterty extension for CustomObject
        public static void SetCustomObject(this SerializedProperty customObject, UnityEngine.Object target)
        {
            if (target is GameObject gameObject)
            {
                customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.IsGameObject)).boolValue = true;
                customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.InstanceId)).intValue = gameObject.GetInstanceID();
                customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.ScenePath)).stringValue = gameObject.scene.name;
                customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.GameObjectName)).stringValue = gameObject.name;
            }
            else
            {
                customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.IsGameObject)).boolValue = false;
                customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.Asset)).objectReferenceValue = target;
            }
        }
        public static UnityEngine.Object GetCustomObject(this SerializedProperty customObject)
        {
            bool isGameObject = customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.IsGameObject)).boolValue;
            if (isGameObject)
            {
                int instanceId = customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.InstanceId)).intValue;
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                return (gameObject);
            }
            else
            {
                return (customObject.GetPropertie(nameof(PeekSaveDatas.CustomObject.Asset)).objectReferenceValue);
            }
        }

        public static void RemoveObject(this SerializedProperty array, UnityEngine.Object toRemove)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).GetCustomObject() == toRemove)
                {
                    array.GetArrayElementAtIndex(i).SetPropertyValue(null);
                    array.DeleteArrayElementAtIndex(i);
                    return;
                }
            }
        }
        public static void RemoveAllObject(this SerializedProperty array, UnityEngine.Object toRemove)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = array.arraySize - 1; i >= 0; i--)
            {
                if (array.GetArrayElementAtIndex(i).GetCustomObject() == toRemove)
                {
                    array.GetArrayElementAtIndex(i).SetPropertyValue(null);
                    array.DeleteArrayElementAtIndex(i);
                }
            }
        }

        public static void AddObject(this SerializedProperty array, UnityEngine.Object toAdd)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).SetCustomObject(toAdd);
        }

        public static void AddString(this SerializedProperty array, string toAdd)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).stringValue = toAdd;
        }

        public static List<UnityEngine.Object> ToObjectList(this SerializedProperty list)
        {
            List<UnityEngine.Object> listObject = new List<UnityEngine.Object>(list.arraySize);
            for (int i = 0; i < list.arraySize; i++)
            {
                listObject.Add(list.GetArrayElementAtIndex(i).GetCustomObject());
            }
            return (listObject);
        }

        public static bool AddObjectIfNoContain(this SerializedProperty array, UnityEngine.Object toAdd)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).GetCustomObject() == toAdd)
                {
                    return (false);
                }
            }
            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).SetCustomObject(toAdd);
            return (true);
        }

        public static bool ContainsObject(this SerializedProperty array, UnityEngine.Object item)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).GetCustomObject() == item)
                {
                    return (true);
                }
            }
            return (false);
        }

        public static bool ContainsString(this SerializedProperty array, string item)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (!array.isArray)
                throw new ArgumentException("Property is not an array");

            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex(i).stringValue == item)
                {
                    return (true);
                }
            }
            return (false);
        }
        #endregion

        #region edit lists
        public static void SetCurrentIndex(int index)
        {
            PeekSave.GetPropertie("_currentIndex").intValue = index;
        }
        public static void AddToIndex(int add)
        {
            PeekSave.GetPropertie("_currentIndex").intValue += add;
            PeekSave.GetPropertie("_currentIndex").intValue  = Mathf.Clamp(CurrentIndex, 0, SelectedObjectsCount - 1);
        }


        public static void ChangeLastSelectedObject(UnityEngine.Object newSelected)
        {
            PeekSave.GetPropertie("_lastSelectedObject").SetCustomObject(newSelected);
        }


        public static void RemoveSelectedWithoutDuplicate(UnityEngine.Object toRemove)
        {
            SelectedObjectsWithoutDuplicateList.RemoveObject(toRemove);
        }
        public static void RemoveSelectedWithoutDuplicateAt(int index)
        {
            SelectedObjectsWithoutDuplicateList.RemoveAt(index);
        }

        public static void Reset()
        {
            ChangeLastSelectedObject(null);
            SetCurrentIndex(-1);

            SelectedObjectsList.ClearArray();
            SelectedObjectsWithoutDuplicateList.ClearArray();
            PinnedObjectsList.ClearArray();
            PinnedObjectsInScenesList.ClearArray();
            PinnedObjectsNameInScenesList.ClearArray();
            PinnedObjectsScenesLinkList.ClearArray();

            Save();
        }

        public static void KeepLinkAndNamesUpToDate()
        {
            if (PinnedObjectsInScenesList == null || PinnedObjectsNameInScenesList == null || PinnedObjectsScenesLinkList == null)
            {
                return;
            }
            for (int i = 0; i < PinnedObjectsInScenesCount; i++)
            {
                if (PinnedObjectsInScenesIndex(i) == null)
                {
                    continue;
                }
                PinnedObjectsNameInScenesList.GetArrayElementAtIndex(i).stringValue = PinnedObjectsInScenesIndex(i).name;
            }
            Save(30);
        }

        /// <summary>
        /// trigger when scene is deleted, or on peekLogic init / update
        /// </summary>
        public static void DeleteBookMarkedGameObjectOfLostScene()
        {
            for (int i = PinnedObjectsScenesLinkCount - 1; i >= 0; i--)
            {
                if (PinnedObjectsScenesLinkIndex(i) == null && PinnedObjectsInScenesIndex(i) == null)
                {
                    PinnedObjectsScenesLinkList.RemoveAt(i);
                    PinnedObjectsInScenesList.RemoveAt(i);
                    PinnedObjectsNameInScenesList.RemoveAt(i);
                }
            }
            Save();
        }

        public static void MoveToBookMark(int index)
        {
            SelectedObjectsWithoutDuplicateList.AddObject(PinnedObjectsInScenesIndex(index));

            PinnedObjectsScenesLinkList.RemoveAt(index);
            PinnedObjectsInScenesList.RemoveAt(index);
            PinnedObjectsNameInScenesList.RemoveAt(index);

            Save(30);
        }

        public static void RemoveBookMarkedGameObjectItem(int index)
        {
            if (LastSelectedObject == PinnedObjectsInScenesIndex(index))
            {
                PeekSerializeObject.ChangeLastSelectedObject(null);
            }
            SelectedObjectsList.RemoveAllObject(PinnedObjectsInScenesIndex(index));
            SelectedObjectsWithoutDuplicateList.RemoveObject(PinnedObjectsInScenesIndex(index));

            PinnedObjectsInScenesList.RemoveAt(index);
            PinnedObjectsNameInScenesList.RemoveAt(index);
            PinnedObjectsScenesLinkList.RemoveAt(index);

            Save();
        }

        public static void RemoveBookMarkedItem(int index)
        {
            if (LastSelectedObject == PinnedObjectsInScenesIndex(index))
            {
                PeekSerializeObject.ChangeLastSelectedObject(null);
            }
            SelectedObjectsList.RemoveAllObject(PinnedObjectsIndex(index));
            SelectedObjectsWithoutDuplicateList.RemoveObject(PinnedObjectsIndex(index));
            PinnedObjectsList.RemoveAt(index);

            Save();
        }

        public static void RemoveAssetFromBookMark(int index)
        {
            SelectedObjectsWithoutDuplicateList.AddObject(PinnedObjectsIndex(index));
            PinnedObjectsList.RemoveAt(index);

            Save();
        }

        public static void RemoveItemInSelection(int index)
        {
            if (LastSelectedObject == SelectedObjectsWithoutDuplicateIndex(index))
            {
                ChangeLastSelectedObject(null);
            }
            SelectedObjectsList.RemoveAllObject(SelectedObjectsWithoutDuplicateIndex(index));
            SelectedObjectsWithoutDuplicateList.RemoveAt(index);

            Save();
        }

        public static void ClearBookMarkGameObjects()
        {
            if (PinnedObjectsInScenesList.ContainsObject(LastSelectedObject))
            {
                PeekSerializeObject.ChangeLastSelectedObject(null);
            
            }

            PinnedObjectsInScenesList.ClearArray();
            PinnedObjectsNameInScenesList.ClearArray();
            PinnedObjectsScenesLinkList.ClearArray();
            Save();
        }

        public static void ClearBookMarkAsset()
        {
            if (PinnedObjectsList.ContainsObject(LastSelectedObject))
            {
                PeekSerializeObject.ChangeLastSelectedObject(null);
            }
            PinnedObjectsList.ClearArray();
            Save();
        }
        public static void ClearSelectedList()
        {
            if (SelectedObjectsWithoutDuplicateList.ContainsObject(LastSelectedObject))
            {
                PeekSerializeObject.ChangeLastSelectedObject(null);
            }
            SelectedObjectsWithoutDuplicateList.ClearArray();
            Save();
        }

        public static void AttemptToRemoveNull()
        {
            bool hasChanged1 = false;
            bool hasChanged2 = false;
            CleanNullFromList(SelectedObjectsList, ref hasChanged1);
            CleanNullFromList(SelectedObjectsWithoutDuplicateList, ref hasChanged2);

            if (hasChanged1 || hasChanged2)
            {
                Save();
            }
        }

        public static bool AddNewSelection(UnityEngine.Object currentSelectedObject)
        {
            if (currentSelectedObject == null)
            {
                return (false);
            }
            if (currentSelectedObject.GetType().ToString() == "Search.Pro.InspectorRecentSO")
            {
                return (false);
            }
            ChangeLastSelectedObject(currentSelectedObject);
            SelectedObjectsList.AddObject(LastSelectedObject);
            SetCurrentIndex(SelectedObjectsCount - 1);

            SelectedObjectsWithoutDuplicateList.RemoveObject(currentSelectedObject);
            if (!PinnedObjectsList.ContainsObject(currentSelectedObject)
                && !PinnedObjectsInScenesList.ContainsObject(currentSelectedObject))
            {
                SelectedObjectsWithoutDuplicateList.AddObject(currentSelectedObject);
            }
            ShrunkListIfNeeded();
            return (true);
        }

        public static bool ShrunkListIfNeeded()
        {
            bool hasChanged = false;

            if (SelectedObjectsCount >= UnityEssentialsPreferences.GetMaxSelectedObjectStored())
            {
                hasChanged = true;
                SelectedObjectsList.RemoveAt(0);
            }
            while (SelectedObjectsWithoutDuplicateCount >= UnityEssentialsPreferences.GetMaxObjectSHown())
            {
                hasChanged = true;
                SelectedObjectsWithoutDuplicateList.RemoveAt(0);
            }
            while (PinnedObjectsCount >= UnityEssentialsPreferences.GetMaxPinnedObject())
            {
                hasChanged = true;
                PinnedObjectsList.RemoveAt(0);
            }
            while (PinnedObjectsInScenesCount >= UnityEssentialsPreferences.GetMaxPinnedObject())
            {
                hasChanged = true;
                PinnedObjectsInScenesList.RemoveAt(0);
                PinnedObjectsNameInScenesList.RemoveAt(0);
                PinnedObjectsScenesLinkList.RemoveAt(0);
            }
            return (hasChanged);
        }

        public static void AddNewBookMark(UnityEngine.Object toSelect)
        {
            GameObject toSelectGameObject = toSelect as GameObject;
            if (toSelectGameObject != null && toSelectGameObject.scene.IsValid())
            {
                if (PinnedObjectsInScenesList.AddObjectIfNoContain(toSelect))
                {
                    PinnedObjectsNameInScenesList.AddString(toSelectGameObject.name);
                    PinnedObjectsScenesLinkList.AddObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(toSelectGameObject.scene.path));
                }
            }
            else
            {
                PinnedObjectsList.AddObjectIfNoContain(toSelect);
            }
        }

        
        /// <summary>
        /// Clean  null item (do not remove items, remove only the list)
        /// </summary>
        /// <param name="listToClean"></param>
        /// <returns>true if list changed</returns>
        public static void CleanNullFromList(SerializedProperty listToClean, ref bool hasChanged)
        {
            hasChanged = false;
            if (listToClean == null)
            {
                return;
            }
            for (int i = listToClean.arraySize - 1; i >= 0; i--)
            {
                if (ExtObjectEditor.IsTruelyNull(listToClean.GetArrayElementAtIndex(i).GetCustomObject()))
                {
                    listToClean.RemoveAt(i);
                    hasChanged = true;
                }
            }
        }

        public static bool IsThereNullCustomObjectInList(this SerializedProperty list)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                if (ExtObjectEditor.IsTruelyNull(list.GetArrayElementAtIndex(i).GetCustomObject()))
                {
                    return (true);
                }
            }
            return (false);
        }
        #endregion

        #region getter
        public static UnityEngine.Object LastSelectedObject { get { return (PeekSave.GetPropertie("_lastSelectedObject").GetCustomObject()); } }
        public static int CurrentIndex { get { return (PeekSave.GetPropertie("_currentIndex").intValue); } }
        public static SerializedProperty SelectedObjectsList { get { return (PeekSave.GetPropertie("_selectedObjects")); } }
        public static SerializedProperty SelectedObjectsWithoutDuplicateList { get { return (PeekSave.GetPropertie("_selectedObjectsWithoutDuplicate")); } }
        public static SerializedProperty PinnedObjectsList { get { return (PeekSave.GetPropertie("_pinnedObjects")); } }
        public static SerializedProperty PinnedObjectsInScenesList { get { return (PeekSave.GetPropertie("_pinnedObjectsInScenes")); } }
        public static SerializedProperty PinnedObjectsNameInScenesList { get { return (PeekSave.GetPropertie("_pinnedObjectsNameInScenes")); } }
        public static SerializedProperty PinnedObjectsScenesLinkList { get { return (PeekSave.GetPropertie("_pinnedObjectsScenesLink")); } }



        public static int SelectedObjectsCount { get { return (SelectedObjectsList.arraySize); } }
        public static int SelectedObjectsWithoutDuplicateCount { get { return (SelectedObjectsWithoutDuplicateList.arraySize);            }        }
        public static int PinnedObjectsCount { get { return (PinnedObjectsList.arraySize); } }
        public static int PinnedObjectsInScenesCount { get { return (PinnedObjectsInScenesList.arraySize); } }
        public static int PinnedObjectsNameInScenesCount { get { return (PinnedObjectsNameInScenesList.arraySize); } }
        public static int PinnedObjectsScenesLinkCount { get { return (PinnedObjectsScenesLinkList.arraySize); } }




        public static UnityEngine.Object SelectedObjectsIndex(int index)
        {
            return (SelectedObjectsList.GetArrayElementAtIndex(index).GetCustomObject());
        }
        public static UnityEngine.Object SelectedObjectsWithoutDuplicateIndex(int index)
        {
            return (SelectedObjectsWithoutDuplicateList.GetArrayElementAtIndex(index).GetCustomObject());
        }
        public static UnityEngine.Object PinnedObjectsIndex(int index)
        {
            return (PinnedObjectsList.GetArrayElementAtIndex(index).GetCustomObject());
        }
        public static UnityEngine.Object PinnedObjectsInScenesIndex(int index)
        {
            return (PinnedObjectsInScenesList.GetArrayElementAtIndex(index).GetCustomObject());
        }
        public static string PinnedObjectsNameInScenesIndex(int index)
        {
            return (PinnedObjectsNameInScenesList.GetArrayElementAtIndex(index).stringValue);
        }
        public static UnityEngine.Object PinnedObjectsScenesLinkIndex(int index)
        {
            return (PinnedObjectsScenesLinkList.GetArrayElementAtIndex(index).GetCustomObject());
        }

        #endregion
    }
}