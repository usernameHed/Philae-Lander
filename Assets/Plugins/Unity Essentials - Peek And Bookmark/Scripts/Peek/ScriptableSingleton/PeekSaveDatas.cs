using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEssentials.Peek.ScriptableObjectSingleton;
using UnityEssentials.Peek.ToolbarExtender;

namespace UnityEssentials.Peek
{
    ///<summary>
    /// 
    ///</summary>
    public class PeekSaveDatas : SingletonScriptableObject<PeekSaveDatas>
    {
        [Serializable]
        public struct CustomObject
        {
            public UnityEngine.Object Asset;
            public bool IsGameObject;
            public int InstanceId;
            public string ScenePath;
            public string GameObjectName;

            public UnityEngine.Object Get()
            {
                if (!IsGameObject)
                {
                    return (Asset);
                }
                GameObject gameObject = EditorUtility.InstanceIDToObject(InstanceId) as GameObject;
                return (gameObject);
            }
        }

        [SerializeField] private List<CustomObject> _selectedObjects = new List<CustomObject>(1000); //public List<UnityEngine.Object> SelectedObjects { get { return (_selectedObjects); } }
        [SerializeField] private List<CustomObject> _selectedObjectsWithoutDuplicate = new List<CustomObject>(100); //public List<UnityEngine.Object> SelectedObjectsWithoutDuplicate { get { return (_selectedObjectsWithoutDuplicate); } }
        [SerializeField] private List<CustomObject> _pinnedObjects = new List<CustomObject>(100); //public List<UnityEngine.Object> PinnedObjects { get { return (_pinnedObjects); } }

        [SerializeField] private List<CustomObject> _pinnedObjectsInScenes = new List<CustomObject>(100);//   public List<UnityEngine.Object> PinnedObjectsInScenes { get { return (_pinnedObjectsInScenes); } }
        [SerializeField] private List<string> _pinnedObjectsNameInScenes = new List<string>(100);//                       public List<string> PinnedObjectsNameInScenes { get { return (_pinnedObjectsNameInScenes); } }
        [SerializeField] private List<CustomObject> _pinnedObjectsScenesLink = new List<CustomObject>(100);// public List<UnityEngine.Object> PinnedObjectsScenesLink { get { return (_pinnedObjectsScenesLink); } }
        [SerializeField] private int _currentIndex; //public int CurrentIndex { get { return (_currentIndex); } }
        [SerializeField] private CustomObject _lastSelectedObject; //public UnityEngine.Object LastSelectedObject { get { return (_lastSelectedObject); } }
        
        public bool GetLastObjectOfThisType<T>(ref UnityEngine.Object found)
        {
            found = null;

            for (int i = _selectedObjects.Count - 1; i >= 0; i--)
            {
                if (_selectedObjects[i].Get() == null)
                {
                    continue;
                }
                if (typeof(T).IsAssignableFrom(_selectedObjects[i].GetType()))
                {
                    found = _selectedObjects[i].Get();
                    return (true);
                }
            }
            return (false);
        }   
    }
}