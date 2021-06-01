using UnityEditor;
using UnityEngine;
using UnityEssentials.SceneWorkflow.extension;

namespace UnityEssentials.CrossSceneReference
{
    [CustomEditor(typeof(GuidComponent), true)]
    public class GuidComponentEditor : PartialEditor
    {
        private GuidComponent guidComp;

        private const string GUID = "serializedGuid";
        private string[] _propertiesToOverride = new string[] { GUID };
        public override string[] PropertiesToOverride => _propertiesToOverride;

        private void OnEnable()
        {
            guidComp = (GuidComponent)target;
        }

        public override void OnInspectorGUI()
        {
            DrawGUI(serializedObject);
        }

        public override void DrawSpecificProperty(SerializedProperty property, string propertyName)
        {
            switch (propertyName)
            {
                case GUID:
                    EditorGUILayout.LabelField("Guid:", guidComp.GetGuid().ToString());
                    break;
            }
        }
    }
}