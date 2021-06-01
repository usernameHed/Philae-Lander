using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;
using UnityEssentials.Spline.PointsOnSplineExtensions.Editor;
using UnityEssentials.Spline.PointsOnSplineExtensions;

namespace UnityEssentials.Spline.editor
{
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : BaseEditor<Spline>
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Spline Type/Spline", false, 10)]
        private static void CreateSpline()
        {
            GameObject splineObj = new GameObject("Spline");
            Spline spline = splineObj.AddComponent<Spline>();
            
            Undo.RegisterCreatedObjectUndo(splineObj, "Create Spline");
            Selection.activeGameObject = splineObj;
            SceneView.lastActiveSceneView.MoveToView(splineObj.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(splineObj);

            Editor splineEditorGeneric = CreateEditor(spline, typeof(SplineEditor));
            SplineEditor splineEditor = (SplineEditor)splineEditorGeneric;
            splineEditor.ConstructSpline();
        }

        public void ConstructSpline()
        {
            if (_target == null)
            {
                return;
            }
            SetupWaypointList();
        }

        private void AddDefaultPoints()
        {
            InsertWaypointAtIndex(_waypointList.index);
            _waypointList.index = 0;
            InsertWaypointAtIndex(_waypointList.index);
            _waypointList.index = 1;
            InsertWaypointAtIndex(_waypointList.index);
            _waypointList.index = 2;
        }

        public static string kPreferTangentSelectionKey = "SplineEditor.PreferTangentSelection";
        public static bool PreferTangentSelection
        {
            get { return EditorPrefs.GetBool(kPreferTangentSelectionKey, false); }
            set
            {
                if (value != PreferTangentSelection)
                    EditorPrefs.SetBool(kPreferTangentSelectionKey, value);
            }
        }
        private ReorderableList _waypointList;
        static bool mWaypointsExpanded;
        bool mPreferTangentSelection;
        private List<PointsOnSplineExtension> _stickerOnSpline = null;

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.Waypoints));
            //excluded.Add("Appearances");
            return excluded;
        }

        void OnEnable()
        {
            _waypointList = null;
            mPreferTangentSelection = PreferTangentSelection;
            Tools.hidden = _target.ReadOnly;
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

            if (_waypointList == null)
                SetupWaypointList();
            if (_waypointList.index >= _waypointList.count)
                _waypointList.index = _waypointList.count - 1;

            // Ordinary properties
            DrawRemainingPropertiesInInspector();

            // Path length
            EditorGUILayout.LabelField("Path Length", _target.PathLength.ToString());

            GUILayout.Label(new GUIContent("Selected Waypoint:"));
            EditorGUILayout.BeginVertical(GUI.skin.box);
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 3 + 10);
            if (_waypointList.index >= 0)
            {
                DrawWaypointEditor(rect, _waypointList.index);
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (_target.Waypoints.Length > 0)
                {
                    EditorGUI.HelpBox(rect,
                        "Click on a waypoint in the scene view\nor in the Path Details list",
                        MessageType.Info);
                }
                else if (GUI.Button(rect, new GUIContent("Add a waypoint to the path")))
                {
                    InsertWaypointAtIndex(_waypointList.index);
                    _waypointList.index = 0;
                }
            }
            EditorGUILayout.EndVertical();

            if (mPreferTangentSelection != EditorGUILayout.Toggle(
                    new GUIContent("Prefer Tangent Drag",
                        "When editing the path, if waypoint position and tangent coincide, dragging will apply preferentially to the tangent"),
                    mPreferTangentSelection))
            {
                PreferTangentSelection = mPreferTangentSelection = !mPreferTangentSelection;
            }

            mWaypointsExpanded = EditorGUILayout.Foldout(mWaypointsExpanded, "Path Details", true);
            if (mWaypointsExpanded)
            {
                EditorGUI.BeginChangeCheck();
                _waypointList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }

            if (_target.ReadOnly == _waypointList.draggable)
            {
                _waypointList.displayAdd = !_target.ReadOnly;
                _waypointList.displayRemove = !_target.ReadOnly;
                _waypointList.draggable = !_target.ReadOnly;
                Tools.hidden = _target.ReadOnly;
            }
        }

        void SetupWaypointList()
        {
            _waypointList = new ReorderableList(
                    serializedObject, FindProperty(x => x.Waypoints),
                    true, true, true, true);
            _waypointList.elementHeight *= 3;

            _waypointList.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Waypoints");
                };

            _waypointList.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    DrawWaypointEditor(rect, index);
                };

            _waypointList.onAddCallback = (ReorderableList l) =>
                {
                    InsertWaypointAtIndex(l.index);
                };

            if (_waypointList.count == 0)
            {
                AddDefaultPoints();
            }
        }

        void DrawWaypointEditor(Rect rect, int index)
        {
            EditorGUI.BeginDisabledGroup(_target.ReadOnly);

            // Needed for accessing string names of fields
            Spline.Waypoint def = new Spline.Waypoint();

            Vector2 numberDimension = GUI.skin.button.CalcSize(new GUIContent("999"));
            Vector2 labelDimension = GUI.skin.label.CalcSize(new GUIContent("Position"));
            Vector2 addButtonDimension = new Vector2(labelDimension.y + 5, labelDimension.y + 1);
            float vSpace = 2;
            float hSpace = 3;

            SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += vSpace / 2;

            Rect r = new Rect(rect.position, numberDimension);
            Color color = GUI.color;
            // GUI.color = Target.m_Appearance.pathColor;
            if (GUI.Button(r, new GUIContent(index.ToString(), "Go to the waypoint in the scene view")))
            {
                if (SceneView.lastActiveSceneView != null)
                {
                    _waypointList.index = index;
                    SceneView.lastActiveSceneView.LookAt(_target.EvaluatePosition(index));
                }
            }
            GUI.color = color;

            r = new Rect(rect.position, labelDimension);
            r.x += hSpace + numberDimension.x;
            EditorGUI.LabelField(r, "Position");
            r.x += hSpace + r.width;
            r.width = rect.width - (numberDimension.x + hSpace + r.width + hSpace + addButtonDimension.x + hSpace);
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.Position), GUIContent.none);
            r.x += r.width + hSpace;
            r.size = addButtonDimension;
            GUIContent buttonContent = EditorGUIUtility.IconContent("d_RectTransform Icon");
            buttonContent.tooltip = "Set to scene-view camera position";
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            if (GUI.Button(r, buttonContent, style) && SceneView.lastActiveSceneView != null)
            {
                Undo.RecordObject(_target, "Set waypoint");
                Spline.Waypoint wp = _target.Waypoints[index];
                Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
                wp.Position = _target.transform.InverseTransformPoint(pos);
                _target.Waypoints[index] = wp;
            }

            r = new Rect(rect.position, labelDimension);
            r.y += numberDimension.y + vSpace;
            r.x += hSpace + numberDimension.x; r.width = labelDimension.x;
            EditorGUI.LabelField(r, "Tangent");
            r.x += hSpace + r.width;
            r.width = rect.width - (numberDimension.x + hSpace + r.width + hSpace + addButtonDimension.x + hSpace);
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.Tangent), GUIContent.none);
            r.x += r.width + hSpace;
            r.size = addButtonDimension;
            buttonContent = EditorGUIUtility.IconContent("ol minus@2x");
            buttonContent.tooltip = "Remove this waypoint";
            if (GUI.Button(r, buttonContent, style))
            {
                Undo.RecordObject(_target, "Delete waypoint");
                var list = new List<Spline.Waypoint>(_target.Waypoints);
                list.RemoveAt(index);
                _target.Waypoints = list.ToArray();
                if (index == _target.Waypoints.Length)
                    _waypointList.index = index - 1;
            }

            r = new Rect(rect.position, labelDimension);
            r.y += 2 * (numberDimension.y + vSpace);
            r.x += hSpace + numberDimension.x; r.width = labelDimension.x;
            EditorGUI.LabelField(r, "Roll");
            r.x += hSpace + labelDimension.x;
            r.width = rect.width
                - (numberDimension.x + hSpace)
                - (labelDimension.x + hSpace)
                - (addButtonDimension.x + hSpace);
            r.width /= 3;
            EditorGUI.MultiPropertyField(r, new GUIContent[] { new GUIContent(" ") },
                element.FindPropertyRelative(() => def.Roll));

            r.x = rect.x + rect.width - addButtonDimension.x;
            r.size = addButtonDimension;
            buttonContent = EditorGUIUtility.IconContent("ol plus@2x");
            buttonContent.tooltip = "Add a new waypoint after this one";
            if (GUI.Button(r, buttonContent, style))
            {
                _waypointList.index = index;
                InsertWaypointAtIndex(index);
            }

            EditorGUI.EndDisabledGroup();
        }

        void InsertWaypointAtIndex(int indexA)
        {
            Vector3 pos = Vector3.forward;
            Vector3 tangent = Vector3.right;
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
                    // Extrapolate the end
                    if (!_target.Waypoints[indexA].Tangent.AlmostZero())
                        tangent = _target.Waypoints[indexA].Tangent;
                    pos = _target.Waypoints[indexA].Position + tangent;
                    roll = _target.Waypoints[indexA].Roll;
                }
                else
                {
                    // Interpolate
                    pos = _target.transform.InverseTransformPoint(
                            _target.EvaluatePosition(0.5f + indexA));
                    tangent = _target.transform.InverseTransformDirection(
                            _target.EvaluateTangent(0.5f + indexA).normalized);
                    roll = Mathf.Lerp(
                            _target.Waypoints[indexA].Roll, _target.Waypoints[indexB].Roll, 0.5f);
                }
            }
            UpdateAllPointListerBeforeChangingSpline(_target);
            Undo.RecordObject(_target, "Add waypoint");
            var wp = new Spline.Waypoint();
            wp.Position = pos;
            wp.Tangent = tangent;
            wp.Roll = roll;
            var list = new List<Spline.Waypoint>(_target.Waypoints);
            list.Insert(indexA + 1, wp);
            _target.Waypoints = list.ToArray();
            _target.InvalidateDistanceCache();
            _waypointList.index = indexA + 1; // select it
            UpdateAllPointLister(_target);
        }

        void OnSceneGUI()
        {
            if (_waypointList == null)
                SetupWaypointList();

            if (Tools.current == Tool.Move)
            {
                EditorGUI.BeginDisabledGroup(_target.ReadOnly);
                Color colorOld = Handles.color;
                var localToWorld = _target.transform.localToWorldMatrix;
                var localRotation = _target.transform.rotation;
                for (int i = 0; i < _target.Waypoints.Length; ++i)
                {
                    DrawSelectionHandle(i, localToWorld);
                    if (_waypointList.index == i)
                    {
                        // Waypoint is selected
                        if (PreferTangentSelection)
                        {
                            DrawPositionControl(i, localToWorld, localRotation);
                            DrawTangentControl(i, localToWorld, localRotation);
                        }
                        else
                        {
                            DrawTangentControl(i, localToWorld, localRotation);
                            DrawPositionControl(i, localToWorld, localRotation);
                        }
                    }
                }
                Handles.color = colorOld;
                EditorGUI.EndDisabledGroup();
            }

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

        void DrawTangentControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
        {
            Spline.Waypoint wp = _target.Waypoints[i];
            Vector3 hPos = localToWorld.MultiplyPoint(wp.Position + wp.Tangent);

            Handles.color = Color.yellow;
            Handles.DrawLine(localToWorld.MultiplyPoint(wp.Position), hPos);

            EditorGUI.BeginChangeCheck();

            Quaternion rotation;
            if (Tools.pivotRotation != PivotRotation.Local)
            {
                rotation = Quaternion.identity;
            }
            else
            {
                rotation = _target.EvaluateOrientationAtUnit(i, SplineBase.PositionUnits.PathUnits);
            }

            float size = HandleUtility.GetHandleSize(hPos) * 0.1f;
            Handles.SphereHandleCap(0, hPos, rotation, size, EventType.Repaint);
            Vector3 newPos = Handles.PositionHandle(hPos, rotation);
            newPos = _target.AttemptToApplyGrid(newPos);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateAllPointListerBeforeChangingSpline(_target);
                Undo.RecordObject(target, "Change Waypoint Tangent");
                newPos = Matrix4x4.Inverse(localToWorld).MultiplyPoint(newPos);
                wp.Tangent = newPos - wp.Position;
                _target.Waypoints[i] = wp;
                _target.InvalidateDistanceCache();
                InspectorUtility.RepaintGameView(_target);
                UpdateAllPointLister(_target);
            }
        }

        void DrawPositionControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
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
            if (EditorGUI.EndChangeCheck())
            {
                pos = _target.AttemptToApplyGrid(pos);
                UpdateAllPointListerBeforeChangingSpline(_target);
                Undo.RecordObject(target, "Move Waypoint");
                wp.Position = Matrix4x4.Inverse(localToWorld).MultiplyPoint(pos);;
                _target.Waypoints[i] = wp;
                _target.InvalidateDistanceCache();
                InspectorUtility.RepaintGameView(_target);
                UpdateAllPointLister(_target);
            }
        }

        public static void DrawPathGizmo(SplineBase path, Color pathColor)
        {
            // Draw the path
            Color colorOld = Gizmos.color;
            Gizmos.color = pathColor;
            float step = 1f / path.Resolution;
            Vector3 lastPos = path.EvaluatePosition(path.MinPos);
            Vector3 lastW = (path.EvaluateOrientation(path.MinPos)
                             * Vector3.right) * path.Appearances.Width / 2;
            for (float t = path.MinPos + step; t <= path.MaxPos + step / 2; t += step)
            {
                Vector3 p = path.EvaluatePosition(t);
                Quaternion q = path.EvaluateOrientation(t);
                Vector3 w = (q * Vector3.right) * path.Appearances.Width / 2;
                Vector3 w2 = w * 1.2f;
                Vector3 p0 = p - w2;
                Vector3 p1 = p + w2;
                Gizmos.DrawLine(p0, p1);
                Gizmos.DrawLine(lastPos - lastW, p - w);
                Gizmos.DrawLine(lastPos + lastW, p + w);
#if false
                // Show the normals, for debugging
                Gizmos.color = Color.red;
                Vector3 y = (q * Vector3.up) * path.m_Appearance.width / 2;
                Gizmos.DrawLine(p, p + y);
                Gizmos.color = pathColor;
#endif
                lastPos = p;
                lastW = w;
            }
            Gizmos.color = colorOld;
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(Spline))]
        static void DrawGizmos(Spline path, GizmoType selectionType)
        {
            DrawPathGizmo(path,
                (Selection.activeGameObject == path.gameObject)
                ? path.Appearances.PathColor : path.Appearances.InactivePathColor);

            DrawGuizmosUnselected(path);
        }

        protected static void DrawGuizmosUnselected(Spline path)
        {
            if (path.gameObject == Selection.activeGameObject)
            {
                return;
            }
            Matrix4x4 localToWorld = path.transform.localToWorldMatrix;

            for (int i = 0; i < path.Waypoints.Length; ++i)
            {
                Spline.Waypoint wp = path.Waypoints[i];
                Vector3 pos = localToWorld.MultiplyPoint(wp.Position);
                float size = HandleUtility.GetHandleSize(pos) * 0.1f;

                Color colorOld = Gizmos.color;
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(pos, size);
                Gizmos.color = colorOld;
            }
        }

        public void UpdateAllPointListerBeforeChangingSpline(SplineBase spline)
        {
            SetupAllSplineSticker(spline);
            for (int i = 0; i < _stickerOnSpline.Count; i++)
            {
                PointsOnSplineExtension lister = _stickerOnSpline[i];
                if (lister == null || !lister.AnchorWhenChingingSpline)
                {
                    continue;
                }
                AnchorPointsOfListerPreCalculation(lister);
            }
        }

        public void AnchorPointsOfListerPreCalculation(PointsOnSplineExtension lister)
        {
            if (lister is ActionPoints)
            {
                ActionPointsEditor listerEditor = (ActionPointsEditor)CreateEditor((ActionPoints)lister, typeof(ActionPointsEditor));
                listerEditor.UpdatePreviousPosition();
                DestroyImmediate(listerEditor);
            }
            else if (lister is ComponentPoints)
            {
                ComponentPointsEditor listerEditor = (ComponentPointsEditor)CreateEditor((ComponentPoints)lister, typeof(ComponentPointsEditor));
                listerEditor.UpdatePreviousPosition();
                DestroyImmediate(listerEditor);
            }
            else if (lister is BasicPoints)
            {
                BasicPointsEditor listerEditor = (BasicPointsEditor)CreateEditor((BasicPoints)lister, typeof(BasicPointsEditor));
                listerEditor.UpdatePreviousPosition();
                DestroyImmediate(listerEditor);
            }
            else if (lister is VectorPoints)
            {
                VectorPointsEditor listerEditor = (VectorPointsEditor)CreateEditor((VectorPoints)lister, typeof(VectorPointsEditor));
                listerEditor.UpdatePreviousPosition();
                DestroyImmediate(listerEditor);
            }
            else if (lister is QuaternionPoints)
            {
                QuaternionPointsEditor listerEditor = (QuaternionPointsEditor)CreateEditor((QuaternionPoints)lister, typeof(QuaternionPointsEditor));
                listerEditor.UpdatePreviousPosition();
                DestroyImmediate(listerEditor);
            }
        }

        public void UpdateAllPointLister(SplineBase spline)
        {
            SetupAllSplineSticker(spline);
            for (int i = 0; i < _stickerOnSpline.Count; i++)
            {
                PointsOnSplineExtension lister = _stickerOnSpline[i];
                if (lister == null || !lister.AnchorWhenChingingSpline)
                {
                    continue;
                }
                AnchorPointsOfLister(lister);
            }
        }


        /// <summary>
        /// must be called when we change the spline
        /// </summary>
        /// <param name="lister"></param>
        public void AnchorPointsOfLister(PointsOnSplineExtension lister)
        {
            if (lister is ActionPoints)
            {
                ActionPointsEditor listerEditor = (ActionPointsEditor)CreateEditor((ActionPoints)lister, typeof(ActionPointsEditor));
                listerEditor.RecalculatePath();
                DestroyImmediate(listerEditor);
            }
            else if (lister is ComponentPoints)
            {
                ComponentPointsEditor listerEditor = (ComponentPointsEditor)CreateEditor((ComponentPoints)lister, typeof(ComponentPointsEditor));
                listerEditor.RecalculatePath();
                DestroyImmediate(listerEditor);
            }
            else if (lister is BasicPoints)
            {
                BasicPointsEditor listerEditor = (BasicPointsEditor)CreateEditor((BasicPoints)lister, typeof(BasicPointsEditor));
                listerEditor.RecalculatePath();
                DestroyImmediate(listerEditor);
            }
            else if (lister is VectorPoints)
            {
                VectorPointsEditor listerEditor = (VectorPointsEditor)CreateEditor((VectorPoints)lister, typeof(VectorPointsEditor));
                listerEditor.RecalculatePath();
                DestroyImmediate(listerEditor);
            }
            else if (lister is QuaternionPoints)
            {
                QuaternionPointsEditor listerEditor = (QuaternionPointsEditor)CreateEditor((QuaternionPoints)lister, typeof(QuaternionPointsEditor));
                listerEditor.RecalculatePath();
                DestroyImmediate(listerEditor);
            }
        }

        private void SetupAllSplineSticker(SplineBase spline)
        {
            if (_stickerOnSpline != null)
            {
                return;
            }

            PointsOnSplineExtension[] allPoints = FindObjectsOfType<PointsOnSplineExtension>();
            _stickerOnSpline = new List<PointsOnSplineExtension>(allPoints.Length);
            for (int i = 0; i < allPoints.Length; i++)
            {
                if (allPoints[i].SplineBase == spline)
                {
                    _stickerOnSpline.Add(allPoints[i]);
                }
            }
        }
    }
}
