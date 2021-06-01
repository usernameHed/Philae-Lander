using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.Folder.Editor
{
    /// <summary>
    /// 
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityEssentials.PropertyAttribute.Folder.FolderReference))]
    public class FolderReferenceDrawer : PropertyDrawer
    {
        private bool _initialized;
        private SerializedProperty _guid;
        private SerializedProperty _realPath;
        private SerializedProperty _realPathWithoutFinalSlash;
        private Object _obj;

        private void Init(SerializedProperty property)
        {
            _initialized = true;
            _guid = property.FindPropertyRelative(nameof(FolderReference.Guid));
            _realPath = property.FindPropertyRelative(nameof(FolderReference.RealPath));
            _realPathWithoutFinalSlash = property.FindPropertyRelative(nameof(FolderReference.RealPathWithoutFinalSlash));

            //if guid is null, but path isn't, try to set the guid
            if (string.IsNullOrEmpty(_guid.stringValue) && !string.IsNullOrEmpty(_realPath.stringValue))
            {
                _guid.stringValue = AssetDatabase.AssetPathToGUID(_realPathWithoutFinalSlash.stringValue);
                property.serializedObject.ApplyModifiedProperties();
            }
            _obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(_guid.stringValue));
            Undo.undoRedoPerformed -= UndoPerformed;
            Undo.undoRedoPerformed += UndoPerformed;
        }

        private void UndoPerformed()
        {
            _initialized = false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            if (!_initialized)
            {
                Init(property);
            }
            GUIContent guiContent = EditorGUIUtility.ObjectContent(_obj, typeof(DefaultAsset));

            Rect r = EditorGUI.PrefixLabel(position, label);

            Rect textFieldRect = r;
            textFieldRect.width -= 40f;

            GUIStyle textFieldStyle = new GUIStyle("TextField")
            {
                imagePosition = _obj ? ImagePosition.ImageLeft : ImagePosition.TextOnly
            };

            if (GUI.Button(textFieldRect, guiContent, textFieldStyle) && _obj)
            {
                EditorGUIUtility.PingObject(_obj);
            }

            if (textFieldRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    DragAndDrop.visualMode = Directory.Exists(path) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    if (Directory.Exists(path))
                    {
                        _obj = reference;
                        _guid.stringValue = AssetDatabase.AssetPathToGUID(path);
                        _realPath.stringValue = path + "/";
                        _realPathWithoutFinalSlash.stringValue = path;
                        property.serializedObject.ApplyModifiedProperties();
                        _initialized = false;
                    }
                    Event.current.Use();
                }
            }

            Rect objectFieldRect = r;
            objectFieldRect.x = textFieldRect.xMax + 1f;
            objectFieldRect.width = 20f;

            if (GUI.Button(objectFieldRect, "x", GUI.skin.GetStyle("Button")))
            {
                _guid.stringValue = "";
                _realPath.stringValue = "";
                _realPathWithoutFinalSlash.stringValue = "";
                property.serializedObject.ApplyModifiedProperties();
                _initialized = false;
            }
            objectFieldRect.x += 20f;
            objectFieldRect.width = 17f;

            if (GUI.Button(objectFieldRect, "", GUI.skin.GetStyle("IN ObjectField")))
            {
                string currentFolder = string.IsNullOrEmpty(_guid.stringValue) ? "Assets" : AssetDatabase.GUIDToAssetPath(_guid.stringValue);
                string path = EditorUtility.OpenFolderPanel("Select a folder", currentFolder, "");
                if (path.Contains(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    _obj = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));
                    _guid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_obj));
                    _realPath.stringValue = AssetDatabase.GUIDToAssetPath(_guid.stringValue) + "/";
                    _realPathWithoutFinalSlash.stringValue = AssetDatabase.GUIDToAssetPath(_guid.stringValue);
                    property.serializedObject.ApplyModifiedProperties();
                    _initialized = false;
                }
                else if (!string.IsNullOrEmpty(path))
                {
                    Debug.LogError("The path must be in the Assets folder");
                }
            }
        }
    }
}