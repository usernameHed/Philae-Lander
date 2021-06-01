using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.editor
{
    [CustomEditor(typeof(BranchBetweenSplines), true)]
    public class BranchEditor : BaseEditor<BranchBetweenSplines>
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Spline Type/Branch Between 2 splines", false, 10)]
        public static BranchBetweenSplines CreateBranch()
        {
            GameObject splineGameObject = new GameObject("BRANCH");
            BranchBetweenSplines branch = splineGameObject.AddComponent<BranchBetweenSplines>();
            Undo.RegisterCreatedObjectUndo(splineGameObject, "Create Branch");
            
            Selection.activeGameObject = splineGameObject;
            SceneView.lastActiveSceneView.MoveToView(splineGameObject.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(splineGameObject);

            BranchEditor branchEditor = (BranchEditor)CreateEditor(branch, typeof(BranchEditor));
            //branchEditor.ConstructBranch();
            return (branch);
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(BranchBetweenSplines))]
        static void DrawGizmos(BranchBetweenSplines branch, GizmoType selectionType)
        {
            if (Application.isPlaying)
            {
                return;
            }
            //ExtDrawGuizmos.DebugWireSphere(branch.GetPosA());
            //ExtDrawGuizmos.DebugWireSphere(branch.GetPosB());
            //Debug.DrawLine(branch.GetPosA(), branch.GetPosB());
            branch.ConstructSpline();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((BranchBetweenSplines)target), typeof(BranchBetweenSplines), false);
            GUI.enabled = true;

            // Ordinary properties
            DrawRemainingPropertiesInInspector();
        }
    }
}
