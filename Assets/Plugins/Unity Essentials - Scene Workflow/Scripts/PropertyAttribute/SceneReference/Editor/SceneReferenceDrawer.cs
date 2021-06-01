using UnityEngine;
using UnityEditor;

namespace UnityEssentials.SceneWorkflow.PropertyAttribute.SceneReference
{
    /// <summary>
    /// Display a Scene Reference object in the editor.
    /// If scene is valid, provides basic buttons to interact with the scene's role in Build Settings.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : PropertyDrawer
    {
        // The exact name of the asset Object variable in the SceneReference object
        private const string sceneAssetPropertyString = "sceneAsset";
        // The exact name of  the scene Path variable in the SceneReference object
        private const string scenePathPropertyString = "scenePath";

        /// <summary>
        /// Drawing the 'SceneReference' property
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneAssetProperty = GetSceneAssetProperty(property);

            // Draw the main Object field
            label.tooltip = "The actual Scene Asset reference.\nOn serialize this is also stored as the asset's path.";

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            EditorGUI.BeginChangeCheck();
            UnityEngine.Object selectedObject = EditorGUI.ObjectField(position, label, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);

            ExtBuildSettings.BuildScene buildScene = ExtBuildSettings.GetBuildScene(selectedObject);

            if (EditorGUI.EndChangeCheck())
            {
                sceneAssetProperty.objectReferenceValue = selectedObject;

                // If no valid scene asset was selected, reset the stored path accordingly
                if (buildScene.scene == null)
                {
                    GetScenePathProperty(property).stringValue = string.Empty;
                }
            }
            EditorGUI.EndProperty();
        }

        static SerializedProperty GetSceneAssetProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(sceneAssetPropertyString);
        }

        static SerializedProperty GetScenePathProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(scenePathPropertyString);
        }
    }

}