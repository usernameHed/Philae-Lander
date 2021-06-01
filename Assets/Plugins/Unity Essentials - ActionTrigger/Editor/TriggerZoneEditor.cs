using UnityEngine;
using UnityEditor;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.ActionTrigger.Extensions;
using UnityEssentials.CrossSceneReference;
using System.Collections.Generic;
using UnityEssentials.SceneWorkflow.extension;

namespace UnityEssentials.ActionTrigger.editor
{
	///<summary>
	/// 
	///</summary>
	[CustomEditor(typeof(TriggerZone), true)]
	public class TriggerZoneEditor : BaseEditor<TriggerZone>
	{
        private static bool _needToUpdateActionList = false;
        private static FrequencyCoolDown _coolDownUpdateList = new FrequencyCoolDown();
        private static List<GuidComponent> _listActions = new List<GuidComponent>(10);
        private static List<GuidReference> _referenceToAdd = new List<GuidReference>(10);

        private static bool _hierarchyChangedSetupped = false;

        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TriggerZone)target), typeof(TriggerZone), false);
            GUI.enabled = true;

            bool automaticlySetListFromSceneActions = _target.AutomaticlySetListFromSceneActions;

            EditorGUI.BeginChangeCheck();
            DrawRemainingPropertiesInInspector();
                
            if (EditorGUI.EndChangeCheck() && _target.AutomaticlySetListFromSceneActions != automaticlySetListFromSceneActions)
            {
                _coolDownUpdateList.Reset();
                _needToUpdateActionList = true;
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
            | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(TriggerZone))]
        static void DrawGizmos(TriggerZone points, GizmoType selectionType)
        {
            if (!_hierarchyChangedSetupped)
            {
                _hierarchyChangedSetupped = true;
                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
            }
            
            UpdateLinks(points);
            ShowLinks(points);
        }

        private static void UpdateLinks(TriggerZone zone)
        {
            if (Application.isPlaying)
            {
                return;
            }
            if (!zone.AutomaticlySetListFromSceneActions)
            {
                return;
            }
            if (!_needToUpdateActionList)
            {
                return;
            }
            if (_coolDownUpdateList.IsRunning())
            {
                return;
            }
            //setup list of all action in the scene

            _listActions.Clear();
            GameObject[] roots = zone.gameObject.scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                GuidComponent[] guidComponents = roots[i].GetComponentsInChildren<GuidComponent>();
                _listActions.Append(guidComponents);
            }

            if (_listActions.Count == zone.ActionCount)
            {
                return;
            }

            _referenceToAdd.Clear();
            for (int i = 0; i < _listActions.Count; i++)
            {
                GuidReference guid = new GuidReference(_listActions[i]);
                _referenceToAdd.Add(guid);
            }

            if (_referenceToAdd.Count > 0)
            {
                TriggerZoneEditor editor = (TriggerZoneEditor)CreateEditor((TriggerZone)zone) as TriggerZoneEditor;
                editor.AddGuidReferences(_referenceToAdd);
                DestroyImmediate(editor);
                _coolDownUpdateList.StartCoolDown(5);
            }
            else
            {
                _coolDownUpdateList.StartCoolDown(2);
            }            
            _needToUpdateActionList = false;
        }

        private static void ShowLinks(TriggerZone points)
        {
            if (!points.ShowLinksToActions)
            {
                return;
            }
            for (int i = 0; i < points.ActionCount; i++)
            {
                GameObject reference = points.GetActionByIndex(i);
                if (reference != null)
                {
                    float sizeArrow = 0.2f;
                    if (Vector3.SqrMagnitude(points.transform.position - reference.transform.position) > sizeArrow * sizeArrow)
                    {
                        ExtDrawGuizmos.DebugArrowConstant(points.transform.position, -(points.transform.position - reference.transform.position) * 0.8f, new Color(1, 1, 1, 0.1f), sizeArrow);
                    }
                }
            }
        }

        private static void OnHierarchyChanged()
        {
            _needToUpdateActionList = true;
        }

        public void AddGuidReferences(List<GuidReference> refs)
        {
            SerializedProperty actionProperty = serializedObject.FindProperty("_actions");
            actionProperty.arraySize = refs.Count;
            serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < refs.Count; i++)
            {
                _target.SetActionByIndex(refs[i], i);
            }
            EditorUtility.SetDirty(_target);
        }
    }
}