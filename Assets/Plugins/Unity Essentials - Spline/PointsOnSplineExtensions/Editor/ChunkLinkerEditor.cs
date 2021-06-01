using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;
using System;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(ChunkLinker))]
    public class ChunkLinkerEditor : BaseEditor<ChunkLinker>
    {
        private ReorderableList _controllers;

        protected virtual void OnEnable()
        {
            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<PointsOnSplineExtension>(this.GetPropertie("_points"), _target.transform);
            this.ApplyModification();
        }

        #region reorder index
        public void RepairChunkIndex(ReorderableList list)
        {
            this.UpdateEditor();
            if (list == null || list.serializedProperty == null)
            {
                return;
            }

            List<int> oldOrder = new List<int>(list.count);
            List<int> newOrder = new List<int>(list.count);
            for (int i = 0; i < list.count; i++)
            {
                oldOrder.Add(list.serializedProperty.GetArrayElementAtIndex(i).GetPropertie("_index").intValue);
                newOrder.Add(i);

                if (oldOrder[i] != newOrder[i])
                {
                    //Debug.Log("change " + oldOrder[i] + " by " + newOrder[i]);
                }
            }

            SerializedProperty chunks = serializedObject.GetPropertie("_chunks");

            for (int i = 0; i < _target.ChunkCount; i++)
            {
                //Debug.Log("chunk " + i);
                SerializedProperty item = chunks.GetArrayElementAtIndex(i);
                AttemptToChangeChunk(oldOrder, newOrder, item);
            }

            this.ApplyModification();
        }

        private void AttemptToChangeChunk(List<int> oldOrder, List<int> newOrder, SerializedProperty item)
        {
            SerializedProperty A = item.GetPropertie(nameof(ChunkLinker.Chunk.A));
            SerializedProperty B = item.GetPropertie(nameof(ChunkLinker.Chunk.B));
            int a = A.intValue;
            int b = B.intValue;

            //Debug.Log("index A: " + a + ", B: " + b);

            for (int i = 0; i < oldOrder.Count; i++)
            {
                int previousIndex = oldOrder[i];
                int newIndex = newOrder[i];

                if (newIndex == previousIndex)
                {
                    continue;
                }
                if (a == previousIndex)
                {
                    A.intValue = newIndex;
                    //Debug.Log("Change " + a + " into " + newIndex);
                }
                if (b == previousIndex)
                {
                    B.intValue = newIndex;
                    //Debug.Log("Change " + b + " into " + newIndex);
                }
            }
        }

        public void RemoveChunkUsingOldPoints(int indexRemoved)
        {
            this.UpdateEditor();
            SerializedProperty chunks = serializedObject.GetPropertie("_chunks");

            for (int i = _target.ChunkCount - 1; i >= 0; i--)
            {
                Debug.Log("chunk " + i);
                SerializedProperty item = chunks.GetArrayElementAtIndex(i);

                SerializedProperty A = item.GetPropertie(nameof(ChunkLinker.Chunk.A));
                SerializedProperty B = item.GetPropertie(nameof(ChunkLinker.Chunk.B));
                int a = A.intValue;
                int b = B.intValue;
                if (a == indexRemoved || b == indexRemoved)
                {
                    chunks.RemoveAt(i);
                }
            }
            this.ApplyModification();
        }
        #endregion

        #region reorderable list
        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((ChunkLinker)target), typeof(ChunkLinker), false);
            GUI.enabled = true;

            if (_controllers == null)
            {
                SetupWaypointList();
            }

            // Points List
            EditorGUI.BeginChangeCheck();
            _controllers.DoLayoutList();
            EditorGUILayout.PropertyField(this.GetPropertie("_points"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            DisplayOtherSettings();
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

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add("_chunks");
            excluded.Add("_points");
            return excluded;
        }

        void SetupWaypointList()
        {
            _controllers = new ReorderableList(
                    serializedObject, this.GetPropertie("_chunks"),
                    true, true, true, true);

            _controllers.drawHeaderCallback = (Rect rect) =>
            { EditorGUI.LabelField(rect, "Chunks"); };

            _controllers.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                { DrawWaypointEditor(rect, index); };

            _controllers.draggable = true;

            _controllers.onAddCallback = (ReorderableList l) =>
            { InsertChunk(l.index); };

            _controllers.onRemoveCallback = (ReorderableList l) =>
            { RemoveChunk(l.index); };
        }

        private void DrawWaypointEditor(Rect rect, int index)
        {
            if (_target.Points == null)
            {
                return;
            }

            SerializedProperty element = _controllers.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty description = element.GetPropertie(nameof(ChunkLinker.Chunk.Description));
            SerializedProperty a = element.GetPropertie(nameof(ChunkLinker.Chunk.A));
            SerializedProperty b = element.GetPropertie(nameof(ChunkLinker.Chunk.B));
            SerializedProperty actionChunk = element.GetPropertie(nameof(ChunkLinker.Chunk.ActionChunk));
            SerializedProperty color = element.GetPropertie(nameof(ChunkLinker.Chunk.Color));

            int numberPoints = _target.Points.WaypointsCount;

            string aDescription = _target.Points.GetWayPoint(a.intValue).Description;
            GUIContent aIndexLabel = new GUIContent(string.IsNullOrWhiteSpace(aDescription) ? "999" : aDescription + "999");
            Vector2 aNumberDimension = GUI.skin.label.CalcSize(aIndexLabel);

            string bDescription = _target.Points.GetWayPoint(b.intValue).Description;
            GUIContent bIndexLabel = new GUIContent(string.IsNullOrWhiteSpace(bDescription) ? "999" : bDescription + "999");
            Vector2 bNumberDimension = GUI.skin.label.CalcSize(bIndexLabel);


            if (GUI.Button(AbsoluteRectLeft(rect, aNumberDimension.x, 0), _target.Points.GetWayPoint(a.intValue).GetDescription()))
            {
                ShowPopupMenu(a, numberPoints, b);
            }
            if (GUI.Button(AbsoluteRectLeft(rect, bNumberDimension.x, aNumberDimension.x), _target.Points.GetWayPoint(b.intValue).GetDescription()))
            {
                ShowPopupMenu(b, numberPoints, a);
            }

            EditorGUI.BeginChangeCheck();
            {
                Rect rectDescription = AbsluteMarginLeftRight(rect, aNumberDimension.x + bNumberDimension.x + 5, 200);
                if (rectDescription.width > 10)
                {
                    description.stringValue = EditorGUI.TextField(rectDescription, description.stringValue);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ApplyModification();
            }

            EditorGUI.PropertyField(AbsoluteRectRight(rect, 110, 70), actionChunk, new GUIContent("", ""));
            EditorGUI.PropertyField(AbsoluteRectRight(rect, 60, 0), color, new GUIContent("", ""));
        }

        private void ShowPopupMenu(SerializedProperty current, int numberPoints, SerializedProperty other)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < numberPoints; i++)
            {
                menu.AddItem(new GUIContent(_target.Points.GetWayPoint(i).GetDescription(), "Waypoints " + i), i == current.intValue, ApplyPropertyChange, new object[2] { current, i });
            }
            menu.AddItem(new GUIContent("<=>"), false, SwitchPropertyValues, new object[2] { current, other });
            menu.ShowAsContext();
        }

        private void ApplyPropertyChange(object propertyToChange)
        {
            object[] array = (object[])propertyToChange;
            SerializedProperty property = (SerializedProperty)array[0];
            int index = (int)array[1];
            property.intValue = index;
            this.ApplyModification();
        }

        private void SwitchPropertyValues(object propertyToChange)
        {
            object[] array = (object[])propertyToChange;
            SerializedProperty a = (SerializedProperty)array[0];
            SerializedProperty b = (SerializedProperty)array[1];
            int aTmp = a.intValue;
            a.intValue = b.intValue;
            b.intValue = aTmp;
            this.ApplyModification();
        }

        private Rect PercentRect(Rect fullRect, float sizePercent = 100, float marginLeftPercent = 0)
        {
            Rect rect = new Rect(fullRect);
            rect.x = fullRect.x + (fullRect.width / 100 * marginLeftPercent);
            rect.width = fullRect.width / 100 * sizePercent;
            return (rect);
        }
        private Rect AbsoluteRectLeft(Rect fullRect, float sizeAbsolute = 10, float marginLeftAbsolute = 0)
        {
            Rect rect = new Rect(fullRect);
            rect.x = fullRect.x + marginLeftAbsolute;
            rect.width = sizeAbsolute;
            return (rect);
        }
        private Rect AbsoluteRectRight(Rect fullRect, float sizeAbsolute = 10, float marginRightAbsolute = 0)
        {
            Rect rect = new Rect(fullRect);
            rect.x = fullRect.x + fullRect.width - sizeAbsolute - marginRightAbsolute;
            rect.width = sizeAbsolute;
            return (rect);
        }

        private Rect AbsluteMarginPercentSize(Rect fullRect, float sizePercent = 100, float marginLeftAbsolute = 0)
        {
            Rect rect = new Rect(fullRect);
            rect.x = fullRect.x + marginLeftAbsolute;
            rect.width = fullRect.width / 100 * sizePercent;
            return (rect);
        }

        private Rect AbsluteMarginLeftRight(Rect fullRect, float marginLeftAbsolute = 0, float marginRightAbsolute = 0)
        {
            Rect rect = new Rect(fullRect);
            rect.x = fullRect.x + marginLeftAbsolute;
            rect.width = fullRect.width - marginLeftAbsolute - marginRightAbsolute;
            return (rect);
        }


        private void InsertChunk(int indexToDuplicate)
        {
            int dupplicated = _controllers.serializedProperty.Dupplicate(indexToDuplicate, out bool firstItem);

            SerializedProperty element = _controllers.serializedProperty.GetArrayElementAtIndex(dupplicated);
            if (firstItem)
            {
                element.GetPropertie(nameof(ChunkLinker.Chunk.Description)).stringValue = "Description";
                element.GetPropertie(nameof(ChunkLinker.Chunk.A)).intValue = Mathf.Max(_target.Points.WaypointsCount - 2, 0);
                element.GetPropertie(nameof(ChunkLinker.Chunk.B)).intValue = Mathf.Max(_target.Points.WaypointsCount - 1, 0);
                element.GetPropertie(nameof(ChunkLinker.Chunk.Color)).colorValue = Color.cyan;
            }

            Component referenceToDuplicate = _target.GetReference(indexToDuplicate);
            bool canDupplicate = referenceToDuplicate != null && this.GetPropertie("_duplicateCurrentSelectedOnCreation").boolValue;

            GameObject referenceToLink;
            if (!canDupplicate)
            {
                referenceToLink = GeneratePoint(dupplicated);
            }
            else
            {
                Vector3 offsetFromPoint = referenceToDuplicate.transform.position;
                UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(referenceToDuplicate.gameObject);
                if (prefabRoot != null)
                {
                    referenceToLink = PrefabUtility.InstantiatePrefab(prefabRoot, referenceToDuplicate.transform.parent) as GameObject;
                    referenceToLink.transform.position = referenceToDuplicate.transform.position;
                    referenceToLink.transform.rotation = referenceToDuplicate.transform.rotation;
                }
                else
                {
                    referenceToLink = Instantiate(referenceToDuplicate.gameObject, referenceToDuplicate.transform.position, referenceToDuplicate.transform.rotation, referenceToDuplicate.transform.parent);
                }
                referenceToLink.transform.position = Vector3.zero;
            }

            Undo.RegisterCreatedObjectUndo(referenceToLink, "Duplicate item of waypoint");

            element.GetPropertie(nameof(ChunkLinker.Chunk.ActionChunk)).objectReferenceValue = referenceToLink;
            _controllers.index = dupplicated;
            this.ApplyModification();
        }

        protected void RemoveChunk(int indexToRemove)
        {
            Component referenceToDuplicate = _target.GetReference(indexToRemove);
            _controllers.serializedProperty.RemoveAt(indexToRemove);
            if (referenceToDuplicate != null && this.GetPropertie("_removeReferenceWhenRemovingPoint").boolValue)
            {
                Undo.DestroyObjectImmediate(referenceToDuplicate.gameObject);
            }
            _controllers.index = indexToRemove - 1;
        }

        private GameObject GeneratePoint(int index)
        {
            GameObject referenceToLink = new GameObject(ComponentName(index), ComponentsToAdd());
            //Vector3 position = _componentTarget.GetPositionFromPoint(positionPath, SplineBase.PositionUnits.Distance);
            //Quaternion rotation = _componentTarget.GetRotationFromPoint(positionPath, SplineBase.PositionUnits.Distance);
            referenceToLink.transform.SetParent(_target.transform);
            //referenceToLink.transform.position = position + (rotation * Vector3.left) * 3;
            return referenceToLink;
        }

        protected virtual string ComponentName(int index)
        {
            return ("Chunk Action");
        }

        protected virtual Type[] ComponentsToAdd()
        {
            return (new Type[1] { typeof(TriggerActionChunk) });
        }
        #endregion

        #region draw SceneView
        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
            | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(ChunkLinker))]
        static void DrawGizmos(ChunkLinker linker, GizmoType selectionType)
        {
            if (linker == null || !linker.ShowWayPoints || linker.Points == null)
            {
                return;
            }
            if (linker.gameObject != Selection.activeGameObject && !linker.ShowWayPointsWhenUnselected)
            {
                return;
            }
            for (int i = 0; i < linker.ChunkCount; i++)
            {
                DrawOneChunk(linker.SplineBase, linker.GetChunk(i), linker.Points, ((i + 1) * linker.OffsetChunks), linker.gameObject == Selection.activeGameObject);
            }
        }

        private static void DrawOneChunk(SplineBase spline, ChunkLinker.Chunk chunk, PointsOnSplineExtension points, float offset, bool isSelected)
        {
            float a = chunk.PositionA(points);
            float b = chunk.PositionB(points);
            Color color = chunk.Color;
            if (!isSelected)
            {
                color.a *= 0.6f;
            }

            float min = spline.ConvertPathUnit(spline.MinPos, SplineBase.PositionUnits.PathUnits, points.PositionUnits);
            float max = spline.ConvertPathUnit(spline.MaxPos, SplineBase.PositionUnits.PathUnits, points.PositionUnits);
            float lenght = spline.MaxUnit(points.PositionUnits);

            if (chunk.SamePosition())
            {
                b = a + lenght;
            }

            if (!spline.Looped)
            {
                if (chunk.SamePosition())
                {
                    DrawPathGizmo(spline, color, min, a, points.PositionUnits, offset);
                    DrawPathGizmo(spline, color, a, max, points.PositionUnits, offset);
                }
                else if (b < a)
                {
                    DrawPathGizmo(spline, color, a, max, points.PositionUnits, offset);
                    DrawPathGizmo(spline, color, min, b, points.PositionUnits, offset);
                }
                else
                {
                    DrawPathGizmo(spline, color, a, b, points.PositionUnits, offset);
                }
            }
            else
            {
                if (b < a)
                {
                    DrawPathGizmo(spline, color, min, b, points.PositionUnits, offset, false, true);
                    DrawPathGizmo(spline, color, a, lenght, points.PositionUnits, offset, true, false);
                }
                else
                {
                    DrawPathGizmo(spline, color, a, b, points.PositionUnits, offset);
                }
            }
        }

        public static void DrawPathGizmo(SplineBase path, Color pathColor, float start, float end, SplineBase.PositionUnits units, float offset, bool canDrawMin = true, bool canDrawMax = true)
        {
            // Draw the path
            Color colorOld = Gizmos.color;
            Gizmos.color = pathColor;
            float step = 1f / path.Resolution;
            Vector3 lastPos = path.EvaluatePositionAtUnit(start, units);
            Vector3 lastW = (path.EvaluateOrientationAtUnit(start, units)
                             * Vector3.right) * path.Appearances.Width * offset;

            Vector3 p0 = lastPos;
            Vector3 p1 = lastW;
            int currentStep = 0;
            for (float t = start + step; t <= end + step / 2; t += step)
            {
                Vector3 p = path.EvaluatePositionAtUnit(t, units);
                Quaternion q = path.EvaluateOrientationAtUnit(t, units);
                Vector3 w = (q * Vector3.right) * path.Appearances.Width * offset;

                p0 = p - w;
                p1 = p + w;
                
                if (currentStep == 0 && canDrawMin)
                {
                    Gizmos.DrawLine(p0, p1);
                }
                Gizmos.DrawLine(lastPos - lastW, p - w);
                Gizmos.DrawLine(lastPos + lastW, p + w);
#if false
                // Show the normals, for debugging
                Gizmos.color = Color.red;
                Vector3 y = (q * Vector3.up) * path.Appearances.Width / 4;
                Gizmos.DrawLine(p, p + y);
                Gizmos.color = pathColor;
#endif
                lastPos = p;
                lastW = w;
                currentStep++;
            }
            if (canDrawMax)
            {
                Gizmos.DrawLine(p0, p1);
            }

            Gizmos.color = colorOld;
        }
        #endregion
    }
}
