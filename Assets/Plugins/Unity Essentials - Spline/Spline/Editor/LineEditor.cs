using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.editor
{
    [CustomEditor(typeof(SimpleLine), true)]
    public class LineEditor : BaseEditor<SimpleLine>
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Spline Type/Simple Line", false, 10)]
        public static SimpleLine CreateLine()
        {
            GameObject splineGameObject = new GameObject("Line");
            SimpleLine line = splineGameObject.AddComponent<SimpleLine>();

            Undo.RegisterCreatedObjectUndo(splineGameObject, "Create Line");
            Selection.activeGameObject = splineGameObject;
            SceneView.lastActiveSceneView.MoveToView(splineGameObject.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(splineGameObject);

            Editor splineEditorGeneric = CreateEditor(line, typeof(LineEditor));
            LineEditor splineEditor = (LineEditor)splineEditorGeneric;
            splineEditor.ConstructLine();
            return (line);
        }

        public static SimpleLine CreateLineOnGameObject(GameObject splineGameObject)
        {
            SimpleLine line = splineGameObject.AddComponent<SimpleLine>();

            Undo.RegisterCompleteObjectUndo(splineGameObject, "Create Line");
            Selection.activeGameObject = splineGameObject;
            SceneView.lastActiveSceneView.MoveToView(splineGameObject.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(splineGameObject);

            Editor splineEditorGeneric = CreateEditor(line, typeof(LineEditor));
            LineEditor splineEditor = (LineEditor)splineEditorGeneric;
            splineEditor.ConstructLine();
            return (line);
        }

        public void ConstructLine()
        {
            if (_target == null)
            {
                return;
            }
            SetupWaypointList();
        }

        private ReorderableList _waypointList;
        private float _currentRoll;
        private bool _isRotatingWp;

        void OnEnable()
        {
            _waypointList = null;
            if (_target != null)
            {
                Tools.hidden = _target.ReadOnly;
            }
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Spline)target), typeof(Spline), false);
            GUI.enabled = true;

            if (_waypointList == null || _waypointList.count != 2)
            {
                SetupWaypointList();
            }

            if (_waypointList.index >= _waypointList.count)
            {
                _waypointList.index = _waypointList.count - 1;
            }

            // Ordinary properties
            DrawRemainingPropertiesInInspector();

            // Path length
            EditorGUILayout.LabelField("Path Length", _target.PathLength.ToString());

            // Waypoints
            EditorGUI.BeginChangeCheck();
            _waypointList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            if (Tools.hidden != _target.ReadOnly)
            {
                Tools.hidden = _target.ReadOnly;
            }
        }

        protected override void DrawRemainingPropertiesInInspector()
        {
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, GetExcludedPropertiesInInspector().ToArray());
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.Waypoints));
            excluded.Add("m_Looped");
            excluded.Add("Resolution");
            return excluded;
        }

        void SetupWaypointList()
        {
            _waypointList = new ReorderableList(
                    serializedObject, FindProperty(x => x.Waypoints),
                    true, true, true, true);

            _waypointList.drawHeaderCallback = (Rect rect) =>
            { EditorGUI.LabelField(rect, "Waypoints"); };

            _waypointList.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                { DrawWaypointEditor(rect, index); };

            _waypointList.onAddCallback = (ReorderableList l) =>
            { InsertWaypointAtIndex(l.index); };

            
            if (_waypointList.count == 0)
            {
                InsertWaypointAtIndex(_waypointList.index);
                _waypointList.index = 0;
                InsertWaypointAtIndex(_waypointList.index);
                _waypointList.index = 1;

                _target.Resolution = 1;
                _target.Appearances.Width = 0;
            }
            _waypointList.displayAdd = false;
            _waypointList.displayRemove = false;
            _waypointList.draggable = false;
        }

        void DrawWaypointEditor(Rect rect, int index)
        {
            EditorGUI.BeginDisabledGroup(_target.ReadOnly);

            // Needed for accessing string names of fields
            SplineSmooth.Waypoint def = new SplineSmooth.Waypoint();
            SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(index);

            float hSpace = 3;
            rect.width -= hSpace; rect.y += 1;
            Vector2 numberDimension = GUI.skin.label.CalcSize(new GUIContent("999"));
            Rect r = new Rect(rect.position, numberDimension);
            if (GUI.Button(r, new GUIContent(index.ToString(), "Go to the waypoint in the scene view")))
            {
                if (SceneView.lastActiveSceneView != null)
                {
                    _waypointList.index = index;
                    SceneView.lastActiveSceneView.LookAt(_target.EvaluatePosition(index));
                }
            }

            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2f;
            r.x += r.width + hSpace; r.width = rect.width - (r.width + hSpace) - (r.height + hSpace);
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.Position), GUIContent.none);
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = oldWidth;

            r.x += r.width + hSpace; r.height += 1; r.width = r.height;
            GUIContent setButtonContent = EditorGUIUtility.IconContent("d_RectTransform Icon");
            setButtonContent.tooltip = "Set to scene-view camera position";
            if (GUI.Button(r, setButtonContent, GUI.skin.label) && SceneView.lastActiveSceneView != null)
            {
                Undo.RecordObject(_target, "Set waypoint");
                Spline.Waypoint wp = _target.Waypoints[index];
                Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
                wp.Position = _target.transform.InverseTransformPoint(pos);
                _target.Waypoints[index] = wp;
            }

            EditorGUI.EndDisabledGroup();
        }

        void InsertWaypointAtIndex(int indexA)
        {
            Vector3 pos = Vector3.right;
            float roll = 0;

            // Get new values from the current indexA (if any)
            int numWaypoints = _target.Waypoints.Length;
            if (indexA < 0)
                indexA = numWaypoints - 1;
            if (indexA >= 0)
            {
                int indexB = indexA + 1;
                if (_target.m_Looped && indexB >= numWaypoints)
                    indexB = 0;
                if (indexB >= numWaypoints)
                {
                    Vector3 delta = Vector3.right;
                    if (indexA > 0)
                        delta = _target.Waypoints[indexA].Position - _target.Waypoints[indexA - 1].Position;
                    pos = _target.Waypoints[indexA].Position + delta;
                    roll = _target.Waypoints[indexA].Roll;
                }
                else
                {
                    // Interpolate
                    pos = _target.transform.InverseTransformPoint(_target.EvaluatePosition(0.5f + indexA));
                    roll = Mathf.Lerp(_target.Waypoints[indexA].Roll, _target.Waypoints[indexB].Roll, 0.5f);
                }
            }
            Undo.RecordObject(_target, "Add waypoint");
            var wp = new Spline.Waypoint();
            wp.Position = pos;
            wp.Roll = roll;
            var list = new List<Spline.Waypoint>(_target.Waypoints);
            list.Insert(indexA + 1, wp);
            _target.Waypoints = list.ToArray();
            _target.InvalidateDistanceCache();
            InspectorUtility.RepaintGameView(_target);
            _waypointList.index = indexA + 1; // select it
        }

        private void OnSceneGUI()
        {
            if (_target == null)
            {
                return;
            }

            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            if (Tools.current == Tool.Move)
            {
                Color colorOld = Handles.color;
                var localToWorld = _target.transform.localToWorldMatrix;
                for (int i = 0; i < _target.Waypoints.Length; ++i)
                {
                    DrawSelectionHandle(i, localToWorld);
                    if (_waypointList.index == i)
                        DrawPositionControl(i, localToWorld, _target.transform.rotation); // Waypoint is selected
                }
                Handles.color = colorOld;
            }
            /*
            else if (Tools.current == Tool.Rotate)
            {
                Color colorOld = Handles.color;
                var localToWorld = Target.transform.localToWorldMatrix;
                for (int i = 0; i < Target.Waypoints.Length; ++i)
                {
                    DrawSelectionHandle(i, localToWorld);
                    if (mWaypointList.index == i)
                        DrawRotationControl(i, localToWorld, Target.transform.rotation); // Waypoint is selected
                }
                Handles.color = colorOld;
            }
            */
            if (Event.current.keyCode == KeyCode.F && _waypointList.index != -1 && Event.current.type == EventType.KeyUp)
            {
                SceneView.lastActiveSceneView.LookAt(_target.EvaluatePosition(_waypointList.index));
            }
        }

        void DrawSelectionHandle(int i, Matrix4x4 localToWorld)
        {
            if (Event.current.button != 1)
            {
                Vector3 pos = localToWorld.MultiplyPoint(_target.Waypoints[i].Position);
                float size = HandleUtility.GetHandleSize(pos) * 0.2f;
                Handles.color = Color.white;
                if (Handles.Button(pos, Quaternion.identity, size, size, Handles.SphereHandleCap)
                    && _waypointList.index != i)
                {
                    _waypointList.index = i;
                    InspectorUtility.RepaintGameView(_target);
                }
                // Label it
                Handles.BeginGUI();
                Vector2 labelSize = new Vector2(
                        EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight);
                Vector2 labelPos = HandleUtility.WorldToGUIPoint(pos);
                labelPos.y -= labelSize.y / 2;
                labelPos.x -= labelSize.x / 2;
                GUILayout.BeginArea(new Rect(labelPos, labelSize));
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(new GUIContent(i.ToString(), "Waypoint " + i), style);
                GUILayout.EndArea();
                Handles.EndGUI();
            }
        }

        private void DrawPositionControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
        {
            Spline.Waypoint wp = _target.Waypoints[i];
            Vector3 pos = localToWorld.MultiplyPoint(wp.Position);
            EditorGUI.BeginChangeCheck();
            Handles.color = _target.Appearances.PathColor;

            Quaternion rotation;
            if (Tools.pivotRotation != PivotRotation.Local)
            {
                rotation = Quaternion.identity;
            }
            else
            {
                rotation = _target.EvaluateOrientationAtUnit(i, SplineBase.PositionUnits.PathUnits);
            }

            float size = HandleUtility.GetHandleSize(pos) * 0.1f;
            Handles.SphereHandleCap(0, pos, rotation, size, EventType.Repaint);
            pos = Handles.PositionHandle(pos, rotation);
            pos = _target.AttemptToApplyGrid(pos);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move Waypoint");
                wp.Position = Matrix4x4.Inverse(localToWorld).MultiplyPoint(pos);
                _target.Waypoints[i] = wp;
                _target.InvalidateDistanceCache();
                InspectorUtility.RepaintGameView(_target);
            }
        }

        private void DrawRotationControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
        {
            Spline.Waypoint wp = _target.Waypoints[i];
            Vector3 pos = localToWorld.MultiplyPoint(wp.Position);
            EditorGUI.BeginChangeCheck();
            Handles.color = Color.yellow;

            Quaternion rotation;
            if (Tools.pivotRotation != PivotRotation.Local)
            {
                rotation = Quaternion.identity;
            }
            else
            {
                rotation = _target.EvaluateOrientationAtUnit(i, SplineBase.PositionUnits.PathUnits);
            }

            float size = HandleUtility.GetHandleSize(pos) * 0.1f;
            Handles.SphereHandleCap(0, pos, rotation, size, EventType.Repaint);

            if (!_isRotatingWp)
            {
                _currentRoll = wp.Roll == 0 ? 1 : wp.Roll;
            }

            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            using (new Handles.DrawingScope(matrix))
            {
                Quaternion newRotation = Handles.Disc(Quaternion.identity, Vector3.zero, new Vector3(0, 0, _currentRoll), 1, true, 0);
                if (EditorGUI.EndChangeCheck())
                {
                    _isRotatingWp = true;
                    Undo.RecordObject(target, "Rotate Waypoint");

                    // Needed for accessing string names of fields
                    SplineSmooth.Waypoint def = new SplineSmooth.Waypoint();
                    SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(i);
                    SerializedProperty rollProperty = element.FindPropertyRelative(() => def.Roll);
                    _currentRoll = newRotation.eulerAngles.z;
                    rollProperty.floatValue = _currentRoll;
                    serializedObject.ApplyModifiedProperties();
                }
            }


        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(SplineSmooth))]
        static void DrawGizmos(SplineSmooth path, GizmoType selectionType)
        {
            SplineEditor.DrawPathGizmo(path,
                (Selection.activeGameObject == path.gameObject)
                ? path.Appearances.PathColor : path.Appearances.InactivePathColor);
            DrawGuizmosUnselected(path);
        }

        protected static void DrawGuizmosUnselected(SplineSmooth path)
        {
            if (path.gameObject == Selection.activeGameObject)
            {
                return;
            }
            Matrix4x4 localToWorld = path.transform.localToWorldMatrix;

            for (int i = 0; i < path.Waypoints.Length; ++i)
            {
                SplineSmooth.Waypoint wp = path.Waypoints[i];
                Vector3 pos = localToWorld.MultiplyPoint(wp.Position);
                float size = HandleUtility.GetHandleSize(pos) * 0.05f;

                Color colorOld = Gizmos.color;
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(pos, size);
                Gizmos.color = colorOld;
            }
        }
    }
}
