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
    [CustomEditor(typeof(ControllerEventsChunks))]
    public class ControllerEventsChunksEditor : ControllerEventsEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Events/Chunks", false, 10)]
        private static void CreatePointsLister()
        {
            GameObject whereToPut = CreateEventTrigger("Event Chunks");
            ControllerEventsChunks trigger = Undo.AddComponent(whereToPut, typeof(ControllerEventsChunks)) as ControllerEventsChunks;
            //ActionPointsEditor pointsEditor = (ActionPointsEditor)CreateEditor((ActionPoints)points, typeof(ActionPointsEditor));
            //pointsEditor.Construct(Color.magenta);
            //DestroyImmediate(pointsEditor);
            Selection.activeGameObject = trigger.gameObject;
        }
    }
}
