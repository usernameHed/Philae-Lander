using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;
using UnityEssentials.Spline.Extensions.Editor.EventEditor;
using UnityEssentials.Spline.Extensions;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(PointsOnSplineExtension))]
    public abstract class PointsOnSplineExtensionEditor : BaseEditor<PointsOnSplineExtension>
    {
        protected ReorderableList _waypointList;
        private static PointsOnSplineExtension _currentPriority = default;
        private SplineBase.PositionUnits _previousPositionUnit;
        private FrequencyChrono _timerBeforeSort = new FrequencyChrono();

        protected const float _space = 3;
        private int _renamingIndex = -1;

        protected abstract SerializedProperty SerializedWaypoints { get; }
        protected abstract void DrawInSceneViewSelected(int index);

        public static GameObject CreatePointsLister(string nameGameObject = "Points Lister")
        {
            GameObject whereToPut;
            if (Selection.activeGameObject != null)
            {
                SplineBase spline = Selection.activeGameObject.GetComponent<SplineBase>();
                if (spline != null)
                {
                    whereToPut = new GameObject(nameGameObject);
                    whereToPut.transform.SetParent(spline.transform);
                    whereToPut.transform.localPosition = Vector3.zero;
                    Undo.RegisterCreatedObjectUndo(whereToPut, "Create " + nameGameObject);
                }
                else
                {
                    whereToPut = Selection.activeGameObject;
                }
            }
            else
            {
                whereToPut = new GameObject(nameGameObject);
                whereToPut.transform.localPosition = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(whereToPut, "Create " + nameGameObject);
            }
            return (whereToPut);
        }

        #region Init
        protected virtual void OnEnable()
        {
            _waypointList = null;

            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), _target.transform);
            this.ApplyModification();

            _previousPositionUnit = _target.PositionUnits;

            if (_target.GetComponent<SplineBase>() == null)
            {
                Tools.hidden = true;
            }
        }

        public void Construct(Color color)
        {
            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            if (_target.SplineBase != null)
            {
                serializedObject.GetPropertie("_colorWayPoints").colorValue = color;
                float pathLenght = _target.SplineBase.PathLength;

                Ray cameraRay = new Ray(SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.forward);
                float closestPointFromCamera = _target.SplineBase.FindClosestPointFromRay(cameraRay, 100, 1, _target.PositionUnits);

                int index = AddPoint(closestPointFromCamera);
                index = AddPoint(closestPointFromCamera + pathLenght / 100 * 5);
                BubbleSort(false);
            }
        }

        protected virtual void OnDisable()
        {
            if (_timerBeforeSort.IsRunning() || _timerBeforeSort.IsFinished())
            {
                BubbleSort(false);
            }
            Tools.hidden = false;
        }

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add("Waypoints");
            excluded.Add("_spline");
            excluded.Add("_positionUnits");
            return excluded;
        }
        #endregion

        #region Inspector
        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((PointsOnSplineExtension)target), typeof(PointsOnSplineExtension), false);
            GUI.enabled = true;

            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            if (_waypointList.index >= _waypointList.count)
            {
                SelectNewWayPoint(_waypointList.count - 1, true);
            }

            UpdatePathUnits();
            DisplayDescription();
            DisplayWaypoints();
            DisplayMainSplineSettings();
            DisplayOtherSettings();
            AttemptToDeletePoint();

            if (_timerBeforeSort.IsFinished())
            {
                BubbleSort(true);
            }
        }

        

        protected void SetupWaypointList()
        {
            _waypointList = new ReorderableList(
                    serializedObject, SerializedWaypoints,
                    true, true, true, true);

            _waypointList.drawHeaderCallback = (Rect rect) =>
            { EditorGUI.LabelField(rect, "Waypoints"); };

            _waypointList.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                { DrawWaypointEditor(rect, index); };

            _waypointList.onAddCallback = (ReorderableList l) =>
            { InsertPoint(l.index); };

            _waypointList.onRemoveCallback = (ReorderableList l) =>
            { RemovePoint(l.index); };

            _waypointList.draggable = false;

            BubbleSort(false);

            SelectNewWayPoint(GetClosestPointFromMouse(), false);
        }

        protected virtual void DrawWaypointEditor(Rect rect, int index)
        {
            rect.width -= _space;
            rect.y += 1;

            SerializedProperty waypoint = _waypointList.serializedProperty.GetArrayElementAtIndex(index);

            if (index == _renamingIndex)
            {
                rect = DisplayRenaming(index, rect, waypoint);
            }
            else
            {
                rect = DisplayIndex(index, rect, waypoint);
            }
            rect = DisplayContent(index, rect, 90);
            rect = DisplayDist(index, rect, 30, waypoint);
            rect = DrawTargetFocus(index, rect);
        }

        protected virtual Rect DisplayRenaming(int index, Rect rect, SerializedProperty waypoint)
        {
            if (Event.current.keyCode == KeyCode.Escape || Event.current.keyCode == KeyCode.Return)
            {
                _renamingIndex = -1;
            }

            string description = waypoint.FindPropertyRelative(nameof(PointsOnSplineExtension.Waypoint.Description)).stringValue;

            GUIContent nameLabel = new GUIContent(string.IsNullOrWhiteSpace(description) ? "999" : description + "999");
            Vector2 numberDimension = GUI.skin.label.CalcSize(nameLabel);

            Rect r = new Rect(rect.position, numberDimension);

            EditorGUI.BeginChangeCheck();
            description = EditorGUI.TextField(r, description);
            if (EditorGUI.EndChangeCheck())
            {
                waypoint.FindPropertyRelative(nameof(PointsOnSplineExtension.Waypoint.Description)).stringValue = description;
                this.ApplyModification();
            }

            rect.position = new Vector2(rect.position.x + numberDimension.x + _space, rect.position.y);
            rect.width -= numberDimension.x - _space;
            return (rect);
        }

        protected virtual Rect DisplayIndex(int index, Rect rect, SerializedProperty waypoint)
        {
            string description = waypoint.FindPropertyRelative(nameof(PointsOnSplineExtension.Waypoint.Description)).stringValue;
            int realIndex = waypoint.FindPropertyRelative("_index").intValue;

            GUIContent indexLabel = new GUIContent(string.IsNullOrWhiteSpace(description) ? "999" : description + "999");
            Vector2 numberDimension = GUI.skin.label.CalcSize(indexLabel);
            Rect r = new Rect(rect.position, numberDimension);

            description = string.IsNullOrWhiteSpace(description) ? realIndex.ToString() : description;
            if (GUI.Button(r, new GUIContent(description, "Go to the waypoint in the scene view")))
            {
                if (Event.current.button == 0)
                {
                    if (SceneView.lastActiveSceneView != null)
                    {
                        SelectNewWayPoint(index, true);
                        Vector3 pos = _target.GetPositionFromPoint(index);
                        SceneView.lastActiveSceneView.LookAt(pos);
                        //InspectorUtility.RepaintGameView(target);
                    }
                }
                else if (Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Rename"), false, RenameCallBack, index);
                    menu.ShowAsContext();
                    Event.current.Use();
                }

            }
            rect.position = new Vector2(rect.position.x + numberDimension.x + _space, rect.position.y);
            rect.width -= numberDimension.x - _space;
            return (rect);
        }

        public void RenameCallBack(object indexObject)
        {
            int index = (int)indexObject;
            _renamingIndex = index;
        }

        protected virtual Rect DisplayContent(int index, Rect rect, float rightMargin)
        {
            Rect r = new Rect(rect);
            r.width -= rightMargin;
            r.height -= _space;

            GUIStyle style = new GUIStyle("Box");
            style.normal.background = ExtTexture.MakeTex(600, 1, Color.red);
            GUI.Box(r, "", style);

            rect.position = new Vector2(rect.position.x + rect.width - rightMargin + _space, rect.position.y);
            rect.width = rightMargin;
            return (rect);
        }

        protected virtual Rect DisplayDist(int index, Rect rect, float rightMargin, SerializedProperty waypoint)
        {
            GUIContent distanceLabel = new GUIContent("Dist");
            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 1.5f;

            Rect r = new Rect(rect);
            r.width = rect.width - rightMargin;

            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = floatFieldWidth;
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(r, waypoint.FindPropertyRelative(nameof(PointsOnSplineExtension.Waypoint.PathPosition)), distanceLabel);
                if (EditorGUI.EndChangeCheck())
                {
                    _timerBeforeSort.StartChrono(1f, false);
                }
            }
            EditorGUIUtility.labelWidth = oldWidth;

            rect.position = new Vector2(rect.position.x + r.width + _space, rect.position.y);
            rect.width = rightMargin - _space;
            return (rect);
        }

        protected virtual Rect DrawTargetFocus(int index, Rect rect)
        {
            GUIContent setButtonContent = EditorGUIUtility.IconContent("d_RectTransform Icon");
            setButtonContent.tooltip = "Set position to the closest point on spline from the TargetToTrack";

            if (GUI.Button(rect, setButtonContent, GUI.skin.label) && SceneView.lastActiveSceneView != null)
            {
                ActionTargetFocus(index);
            }
            return (rect);
        }

        protected virtual void ActionTargetFocus(int index)
        {
            Undo.RecordObject(_target, "Set waypoint");
            PointsOnSplineExtension.Waypoint wp = _target.GetWayPoint(index);
            Vector3 pos = _target.TargetPosition;
            wp.PathPosition = _target.ConvertWorldPositionToPathPosition(pos);
            _target.SetWayPoint(wp, index);
            _timerBeforeSort.StartChrono(0.1f, false);
        }

        /// <summary>
        /// display description text
        /// </summary>
        private void DisplayDescription()
        {
            Color old = GUI.backgroundColor;
            GUI.backgroundColor = serializedObject.FindProperty("_colorWayPoints").colorValue;
            SerializedProperty description = serializedObject.FindProperty("_description");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(description);
            if (EditorGUI.EndChangeCheck())
            {
                this.ApplyModification();
            }
            GUI.backgroundColor = old;
        }

        /// <summary>
        /// display waypoints reorderable list
        /// </summary>
        private void DisplayWaypoints()
        {
            EditorGUI.BeginDisabledGroup(!_target.IsSplineLinked);
            {
                // Waypoints
                EditorGUI.BeginChangeCheck();
                _waypointList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                {
                    this.ApplyModification();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DisplayMainSplineSettings()
        {
            SerializedProperty spline = serializedObject.FindProperty("_spline");
            SerializedProperty units = serializedObject.FindProperty("_positionUnits");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spline);
            EditorGUILayout.PropertyField(units);
            if (EditorGUI.EndChangeCheck())
            {
                this.ApplyModification();
            }
        }

        /// <summary>
        /// display all other settings
        /// </summary>
        private void DisplayOtherSettings()
        {
            EditorGUI.BeginChangeCheck();
            bool foldout = EditorGUILayout.Foldout(this.GetPropertie("_foldout").boolValue, "Editor Settings");
            if (EditorGUI.EndChangeCheck())
            {
                this.GetPropertie("_foldout").boolValue = foldout;
                this.ApplyModification();
            }
            
            if (foldout)
            {
                // Ordinary properties
                DrawRemainingPropertiesInInspector();
            }
        }
        #endregion

        #region Misc
        protected virtual int InsertPoint(int index)
        {
            if (_waypointList.count == 0 || serializedObject.GetPropertie("_addPointClosestToCamera").boolValue)
            {
                return (AddCloseToCamera());
            }
            _waypointList.serializedProperty.InsertAtIndex(_waypointList.index == -1 ? index : _waypointList.index);
            this.ApplyModification();
            SelectNewWayPoint(index + 1, true);

            RepairIndexWaypoints();
            ApplyPointsListerReference(_waypointList.index);
            this.ApplyModification();
            return (_waypointList.index);
        }

        /// <summary>
        /// add point closest to camera, reorder list, and return the current index
        /// </summary>
        /// <returns>current point added</returns>
        protected int AddCloseToCamera()
        {
            Ray cameraRay = new Ray(SceneView.lastActiveSceneView.camera.transform.position, SceneView.lastActiveSceneView.camera.transform.forward);
            float closestPointFromCamera = _target.SplineBase.FindClosestPointFromRay(cameraRay, 100, 1, _target.PositionUnits);
            int index = AddPoint(closestPointFromCamera);
            BubbleSort(true);

            SelectNewWayPoint(_waypointList.index, true);
            return (_waypointList.index);
        }

        /// <summary>
        /// add a point at the given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>index of point added (always at the end)</returns>
        protected virtual int AddPoint(float position)
        {
            _waypointList.serializedProperty.arraySize++;
            SelectNewWayPoint(_waypointList.serializedProperty.arraySize - 1, false);

            _waypointList.serializedProperty.GetArrayElementAtIndex(_waypointList.index).GetPropertie("_index").intValue = _waypointList.index;
            ApplyPointsListerReference(_waypointList.index);

            SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(_waypointList.index);
            SerializedProperty pathPosition = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition));
            pathPosition.floatValue = position;
            this.ApplyModification();
            return (_waypointList.index);
        }

        protected virtual void RemovePoint(int index)
        {
            DeletePointInChunk(index);
            _waypointList.serializedProperty.RemoveAt(index);
            RepairIndexWaypoints();
            this.ApplyModification();
        }

        protected virtual void SelectNewWayPoint(int index, bool additionnalPing)
        {
            _waypointList.index = index;
            _currentPriority = _target;
            SaveIndexSelected(index);
        }

        private void SaveIndexSelected(int index)
        {
            SerializedProperty lastIndex = serializedObject.FindProperty("_lastIndexSelected");
            lastIndex.intValue = index;
        }

        private int GetClosestPointFromMouse()
        {
            if (EditorWindow.mouseOverWindow == null)
            {
                return (serializedObject.FindProperty("_lastIndexSelected").intValue);
            }

            if (EditorWindow.mouseOverWindow.GetType() == typeof(SceneView))
            {
                Vector3[] pointPosition = new Vector3[_target.WaypointsCount];
                for (int i = 0; i < _target.WaypointsCount; ++i)
                {
                    Vector3 pos = _target.GetPositionFromPoint(i);
                    //ExtDrawGuizmos.DebugWireSphere(pos, 1f, 1f);
                    pointPosition[i] = pos;
                }
                Ray mouse = ExtSceneView.CalculateMousePosition(SceneView.lastActiveSceneView);
                //Debug.DrawRay(mouse.origin, mouse.direction * 1000, Color.green, 10f);

                int closest = GetClosestPointToRay(mouse, out float minDist, pointPosition);
                //ExtDrawGuizmos.DebugWireSphere(_target.GetPositionFromPoint(closest), Color.green, 1f, 1f);

                SaveIndexSelected(closest);
                this.ApplyModification();
                return (closest);
            }
            return serializedObject.FindProperty("_lastIndexSelected").intValue;
        }

        /// <summary>
        /// from a set of points, return the closest one to the ray
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ray"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static int GetClosestPointToRay(Ray ray, out float minDist, params Vector3[] points)
        {
            minDist = 0;
            if (points.Length == 0)
            {
                return (-1);
            }
            Vector3 p1 = ray.origin;
            Vector3 p2 = ray.GetPoint(1);
            minDist = DistancePointToLine3D(points[0], p1, p2);
            int index = 0;
            for (int i = 1; i < points.Length; i++)
            {
                float dist = DistancePointToLine3D(points[i], p1, p2);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
            return (index);
        }

        public static float DistancePointToLine3D(Vector3 point, Vector3 lineP1, Vector3 lineP2)
        {
            return (Vector3.Cross(lineP1 - lineP2, point - lineP1).magnitude);
        }

        private void UpdatePathUnits()
        {
            if (_previousPositionUnit != _target.PositionUnits)
            {
                SplineBase spline = _target.SplineBase;
                for (int i = 0; i < _target.WaypointsCount; ++i)
                {
                    SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(i);
                    SerializedProperty pathPosition = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition));
                    float currentPathPosition = pathPosition.floatValue;
                    float newPos = spline.ConvertPathUnit(currentPathPosition, _previousPositionUnit, _target.PositionUnits);
                    pathPosition.floatValue = newPos;
                }
                this.ApplyModification();
            }
            _previousPositionUnit = _target.PositionUnits;
        }

        /// <summary>
        /// bubble sort optimize algorythme
        /// </summary>
        /// <returns>haschanged</returns>
        protected virtual bool BubbleSort(bool changeIndexAtEnd)
        {
            float currentPath = 0;
            if (_waypointList.index != -1)
            {
                currentPath = _waypointList.serializedProperty.GetArrayElementAtIndex(_waypointList.index).GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition)).floatValue;
            }
            else
            {
                changeIndexAtEnd = false;
            }

            bool changed = false;
            for (int i = _waypointList.count - 1; i >= 1; i--)
            {
                bool sorted = true;
                for (int j = 0; j <= i - 1; j++)
                {
                    if (_waypointList.serializedProperty.GetArrayElementAtIndex(j + 1).GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition)).floatValue
                        < _waypointList.serializedProperty.GetArrayElementAtIndex(j).GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition)).floatValue)
                    {
                        _waypointList.Move(j + 1, j);
                        sorted = false;
                        changed = true;
                    }
                }
                if (sorted)
                {
                    break;
                }
            }
            if (changed)
            {
                if (changeIndexAtEnd)
                {
                    for (int i = 0; i < _target.WaypointsCount; i++)
                    {
                        float indexPath = _waypointList.serializedProperty.GetArrayElementAtIndex(i).GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition)).floatValue;
                        if (indexPath == currentPath)
                        {
                            _waypointList.index = i;
                            break;
                        }
                    }
                }
                RepairIndexWaypoints();
                this.ApplyModification();
            }

            _timerBeforeSort.Reset();
            return (changed);
        }

        private void ApplyPointsListerReference(int index)
        {
            _waypointList.serializedProperty.GetArrayElementAtIndex(index).GetPropertie("_pointsListerReference").objectReferenceValue = _target;
        }

        protected virtual void RepairIndexWaypoints()
        {
            ReorderChunks();
            for (int i = 0; i < _waypointList.count; i++)
            {
                _waypointList.serializedProperty.GetArrayElementAtIndex(i).GetPropertie("_index").intValue = i;
            }
        }

        #region Chunk Related
        private void ReorderChunks()
        {
            ChunkLinker[] chunks = _target.GetComponentsInChildren<ChunkLinker>();
            for (int i = 0; i < chunks.Length; i++)
            {
                ChunkLinkerEditor pointsEditor = (ChunkLinkerEditor)CreateEditor((ChunkLinker)chunks[i], typeof(ChunkLinkerEditor));
                pointsEditor.RepairChunkIndex(_waypointList);
            }
        }

        private void DeletePointInChunk(int index)
        {
            ChunkLinker[] chunks = _target.GetComponentsInChildren<ChunkLinker>();
            for (int i = 0; i < chunks.Length; i++)
            {
                ChunkLinkerEditor pointsEditor = (ChunkLinkerEditor)CreateEditor((ChunkLinker)chunks[i], typeof(ChunkLinkerEditor));
                pointsEditor.RemoveChunkUsingOldPoints(index);
            }
        }
        #endregion

        private void AttemptToDeletePoint()
        {
            if (Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.KeyDown && _waypointList.index == -1)
            {
                Event.current.Use();
                return;
            }

            if (Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.KeyUp && _waypointList.index != -1)
            {
                DeletePoint(_waypointList.index);
            }
        }

        private void DeletePoint(int index)
        {
            _waypointList.DeleteAt(index);
        }

        public virtual void UpdatePreviousPosition()
        {
            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            for (int i = 0; i < _target.WaypointsCount; i++)
            {
                SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(i);
                SerializedProperty previous = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PreviousPosition));
                previous.vector3Value = _target.GetPositionFromPoint(i);
            }
            this.ApplyModification();
        }

        public virtual void RecalculatePath()
        {
            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            for (int i = 0; i < _waypointList.count; i++)
            {
                SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(i);
                SerializedProperty previous = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PreviousPosition));

                float closestPositionOnSpline = _target.SplineBase.FindClosestPoint(previous.vector3Value, 0, -1, 10);
                float currentUnits = _target.SplineBase.FromPathNativeUnits(closestPositionOnSpline, _target.PositionUnits);

                SerializedProperty pathPosition = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition));
                pathPosition.floatValue = currentUnits;
            }
            this.ApplyModification();
        }
        #endregion

        #region SceneView

        protected virtual void OnSceneGUI()
        {
            if (_target == null || !_target.ShowWayPoints)
            {
                return;
            }
            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            if (Tools.current == Tool.Move)
            {
                AttemptToDeletePoint();

                Color colorOld = Handles.color;
                var localToWorld = _target.transform.localToWorldMatrix;
                for (int i = 0; i < _target.WaypointsCount; ++i)
                {
                    DrawSelectionHandle(i);
                    DrawInSceneViewSelected(i);
                    if (_waypointList.index == i)
                    {
                        DrawPositionControl(i, localToWorld, _target.transform.rotation); // Waypoint is selected
                    }
                }
                Handles.color = colorOld;
            }

            bool isOtherPointOnSplineActive = _currentPriority != null && _currentPriority != _target;
            if (_waypointList.index != -1 && (isOtherPointOnSplineActive || Event.current.keyCode == KeyCode.Escape))
            {
                _waypointList.index = -1;
            }

            if (Event.current.keyCode == KeyCode.F && _waypointList.index != -1 && Event.current.type == EventType.KeyUp)
            {
                SceneView.lastActiveSceneView.LookAt(_target.GetPositionFromPoint(_waypointList.index));
            }
        }

        private void DrawPositionControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
        {
            PointsOnSplineExtension.Waypoint wp = _target.GetWayPoint(i);
            Vector3 pos = _target.GetPositionFromPoint(i);
            EditorGUI.BeginChangeCheck();

            Handles.color = _target.ColorWayPoint;

            Quaternion rotation;
            if (Tools.pivotRotation != PivotRotation.Local)
            {
                rotation = Quaternion.identity;
            }
            else
            {
                rotation = _target.GetRotationFromPoint(i);
            }

            float size = HandleUtility.GetHandleSize(pos) * 0.1f;
            if (Event.current.button == 1 || Event.current.alt)
            {
                Handles.CircleHandleCap(0, pos, rotation, size * 3, EventType.Repaint);
                return;
            }

            CustomHandleMove.DragHandleResult handleResult;
            Vector3 newPos = CustomHandleMove.DragHandle(pos, size * 3, _target.ColorWayPoint, out handleResult, rotation, 0);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move Waypoint");
                float oldPos = wp.PathPosition;
                wp.PathPosition = _target.SplineBase.AttemptToApplyGrid(_target.ConvertWorldPositionToPathPosition(newPos));
                _target.SetWayPoint(wp, i);
                OnMovePoint(i, oldPos, wp.PathPosition);
            }

            switch (handleResult)
            {
                case CustomHandleMove.DragHandleResult.OnLeftRelease:
                    BubbleSort(true);
                    break;
            }
        }

        protected virtual void OnMovePoint(int index, float oldPos, float newPos)
        {
            //InspectorUtility.RepaintGameView(_target);
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
            | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(PointsOnSplineExtension))]
        static void DrawGizmos(PointsOnSplineExtension points, GizmoType selectionType)
        {
            if (points.gameObject == Selection.activeGameObject)
            {
                return;
            }
            if (!points.ShowWayPointsWhenUnselected)
            {
                return;
            }
            for (int i = 0; i < points.WaypointsCount; ++i)
            {
                DrawSelectionHandleUnselected(points, i);
            }
        }

        private void DrawSelectionHandle(int i)
        {
            float pathPosition = _target.GetWayPoint(i).PathPosition;
            Vector3 pos = _target.GetPositionFromPoint(i);

            float size = HandleUtility.GetHandleSize(pos) * 0.2f;
            Handles.color = _target.ColorWayPoint;
            if (Handles.Button(pos, Quaternion.identity, size, size, Handles.SphereHandleCap)
                && _waypointList != null && _waypointList.index != i)
            {
                SelectNewWayPoint(i, true);
                //InspectorUtility.RepaintGameView(target);
            }

            Vector2 labelSize;
            string description;
            Vector2 labelPos = HandleUtility.WorldToGUIPoint(pos);
            if (serializedObject.GetPropertie("_showFullNameOnSceneView").boolValue)
            {
                GUIContent nameLabel = new GUIContent(string.IsNullOrWhiteSpace(_target.GetWayPoint(i).Description) ? "99" : _target.GetWayPoint(i).Description + "999");
                labelPos.y += string.IsNullOrWhiteSpace(_target.GetWayPoint(i).Description) ? 0 : -10;
                labelSize = GUI.skin.label.CalcSize(nameLabel);
                description = string.IsNullOrWhiteSpace(_target.GetWayPoint(i).Description) ? i.ToString() : _target.GetWayPoint(i).Description;
            }
            else
            {
                labelSize = new Vector2(EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight);
                description = _target.GetWayPoint(i).Index.ToString();
            }

            string tooltip = "Waypoint " + (string.IsNullOrWhiteSpace(_target.GetWayPoint(i).Description) ? _target.GetWayPoint(i).Index.ToString() : _target.GetWayPoint(i).Description);

            Handles.BeginGUI();
            
            labelPos.y -= labelSize.y / 2;
            labelPos.x -= labelSize.x / 2 - 2;
            GUILayout.BeginArea(new Rect(labelPos, labelSize));
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 14;

                GUILayout.Label(new GUIContent(description, tooltip), style);
            }
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private static void DrawSelectionHandleUnselected(PointsOnSplineExtension target, int i)
        {
            Vector3 pos = target.GetPositionFromPoint(i);
            float size = HandleUtility.GetHandleSize(pos) * 0.07f;
            //ExtDrawGuizmos.DebugSphere(pos, target.ColorWayPoint, size);
            Color colorOld = Gizmos.color;
            Gizmos.color = target.ColorWayPoint;
            Gizmos.DrawSphere(pos, size);
            Gizmos.color = colorOld;
        }

        /// <summary>
        /// display string in sceneView from 3d position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="toDisplay"></param>
        public static void DisplayStringInSceneViewFrom3dPosition(Vector3 position, string toDisplay, Color color, int fontSize = 20)
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = color;
            textStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle textStyleThickness = new GUIStyle(textStyle);
            textStyleThickness.normal.textColor = Color.black;

            Handles.Label(position + new Vector3(0.05f, -0.05f, 0), toDisplay, textStyleThickness);
            Handles.Label(position, toDisplay, textStyle);
        }
        #endregion

    }
}
