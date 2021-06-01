using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEssentials.Spline.Extensions.Editor;
using System;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(ActionPoints), true)]
    public class ActionPointsEditor : ComponentPointsEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Points Lister/Action", false, 10)]
        public static ActionPoints CreatePointsLister()
        {
            GameObject whereToPut = CreatePointsLister("Action Points Lister");
            ActionPoints points = whereToPut.AddComponent(typeof(ActionPoints)) as ActionPoints;
            ActionPointsEditor pointsEditor = (ActionPointsEditor)CreateEditor((ActionPoints)points, typeof(ActionPointsEditor));
            pointsEditor.Construct(Color.magenta);
            DestroyImmediate(pointsEditor);
            Selection.activeGameObject = points.gameObject;
            return (points);
        }

        protected ActionPointsEditor _actionTarget { get { return target as ActionPointsEditor; } }

        protected override SerializedProperty SerializedWaypoints
        {
            get
            {
                return (serializedObject.FindProperty(nameof(ActionPoints.WaypointsAction)));
            }
        }

        protected override string ComponentReference
        {
            get
            {
                return (nameof(ActionPoints.ActionPointsWaypoint.ReferenceAction));
            }
        }

        protected override bool SerializeObjectField(Rect rect, SerializedProperty toChange)
        {
            return (ObjectField<TriggerActionPoint>(rect, toChange));
        }

        protected override Type[] ComponentsToAdd()
        {
            return (new Type[] { typeof(TriggerActionPoint) });
        }

        protected override string ComponentName(int index)
        {
            return ("TriggerAction");
        }

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(nameof(ActionPoints.WaypointsAction));
            return excluded;
        }

        protected override void RepairIndexWaypoints()
        {
            for (int i = 0; i < _waypointList.count; i++)
            {
                _waypointList.serializedProperty.GetArrayElementAtIndex(i).GetPropertie("_index").intValue = i;

                SerializedProperty reference = SerializedWaypoints.GetArrayElementAtIndex(i).GetPropertie(ComponentReference);
                if (reference.objectReferenceValue == null)
                {
                    continue;
                }
                TriggerActionPoint actionPoints = (TriggerActionPoint)reference.objectReferenceValue;
                actionPoints.transform.SetSiblingIndex(i);
            }
        }
        
        public override void RecalculatePath()
        {
            if (_waypointList == null)
            {
                SetupWaypointList();
            }

            for (int i = 0; i < SerializedWaypoints.arraySize; i++)
            {
                SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(i);
                SerializedProperty previous = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PreviousPosition));

                float closestPositionOnSpline = _target.SplineBase.FindClosestPoint(previous.vector3Value, 0, -1, 10);
                float currentUnits = _target.SplineBase.FromPathNativeUnits(closestPositionOnSpline, _target.PositionUnits);

                SerializedProperty pathPosition = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition));
                pathPosition.floatValue = currentUnits;
            }
            this.ApplyModification();

            //UpdateTargetPosition();
        }
        
    }
}