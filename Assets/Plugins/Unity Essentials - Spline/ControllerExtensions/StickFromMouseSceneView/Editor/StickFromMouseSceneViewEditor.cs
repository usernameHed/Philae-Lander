using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.ControllerExtensions
{
    [CustomEditor(typeof(StickFromMouseSceneView))]
    public class StickFromMouseSceneViewEditor : BaseEditor<StickFromMouseSceneView>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((StickFromMouseSceneView)target), typeof(StickFromMouseSceneView), false);
            GUI.enabled = true;

            DrawRemainingPropertiesInInspector();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(StickFromMouseSceneView))]
        static void DrawGizmos(StickFromMouseSceneView stick, GizmoType selectionType)
        {
            StickUpdater.SetReference(stick);
        }
    }
}