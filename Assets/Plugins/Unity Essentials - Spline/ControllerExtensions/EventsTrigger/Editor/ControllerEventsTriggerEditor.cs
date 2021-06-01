using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;
using UnityEssentials.Spline.PointsOnSplineExtensions;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.Controller;

namespace UnityEssentials.Spline.ControllerExtensions.EventsTrigger
{
    [CustomEditor(typeof(ControllerEventsTrigger))]
    public class ControllerEventsTriggerEditor : ControllerEventsEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Events/Trigger", false, 10)]
        private static void CreatePointsLister()
        {
            GameObject whereToPut = CreateEventTrigger("Event Trigger");
            ControllerEventsTrigger trigger = Undo.AddComponent(whereToPut, typeof(ControllerEventsTrigger)) as ControllerEventsTrigger;
            //ActionPointsEditor pointsEditor = (ActionPointsEditor)CreateEditor((ActionPoints)points, typeof(ActionPointsEditor));
            //pointsEditor.Construct(Color.magenta);
            //DestroyImmediate(pointsEditor);
            Selection.activeGameObject = trigger.gameObject;
        }
    }
}
