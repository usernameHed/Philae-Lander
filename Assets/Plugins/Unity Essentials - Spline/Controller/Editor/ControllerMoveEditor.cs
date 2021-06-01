using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.ControllerExtensions.EventsTrigger;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.Extensions.Editor;
using UnityEssentials.Spline.Extensions.Editor.EventEditor;
using UnityEssentials.Spline.PointsOnSplineExtensions;

namespace UnityEssentials.Spline.Controller.editor
{
    [CustomEditor(typeof(SplineControllerMove), true)]
    public class ControllerMoveEditor : ControllerStickEditor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
              | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(SplineControllerMove))]
        static void DrawGizmos(SplineControllerMove sticker, GizmoType selectionType)
        {
            if (Application.isPlaying || sticker == null || sticker.SplineBase == null)
            {
                return;
            }
            if (sticker.MoveInEditor)
            {
                sticker.AttemptToMove();
            }
        }
    }
}