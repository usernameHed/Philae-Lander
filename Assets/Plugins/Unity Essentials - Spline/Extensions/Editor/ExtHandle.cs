using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions.Editor
{
    public static class ExtHandle
    {
        public enum DrawOutlineType
        {
            MIDDLE,
            INSIDE,
            OUTSIDE,
        }

        public static void DoMultiHandle(Transform toMove, out bool hasChanged)
        {
            hasChanged = false;
            Tool current = Tools.current;
            switch (current)
            {
                case Tool.Move:
                    DoHandleMove(toMove, true, out hasChanged);
                    break;
                case Tool.Rotate:
                    DoHandleRotation(toMove, true, out hasChanged);
                    break;
                case Tool.Scale:
                    DoHandleScale(toMove, true, out hasChanged);
                    break;
                case Tool.Rect:
                    DoHandleRect(toMove, true, out hasChanged);
                    break;
                    /*
                case Tool.Transform:
                    DoHandleTransform(toMove, true, out hasChanged);
                    break;
                    */
            }
        }

        public static void DoHandleMove(Transform toMove, bool record, out bool hasChanged, float snap = 0.1f)
        {
            hasChanged = false;
            if (record)
            {
                Undo.RecordObject(toMove.gameObject.transform, "handle Point move");
            }

            Quaternion rotation = (Tools.pivotRotation == PivotRotation.Global) ? Quaternion.identity : toMove.rotation;
            Vector3 newPosition = Handles.PositionHandle(toMove.position, rotation);

            if (newPosition != toMove.position)
            {
                hasChanged = true;
                toMove.position = AttemptToApplyGrid(newPosition, snap);
                if (record)
                {
                    EditorUtility.SetDirty(toMove);
                }
            }
        }

        public static Vector3 DoHandleMove(Vector3 toMove, Quaternion currentRotation, out bool hasChanged, float snap = 0.1f)
        {
            hasChanged = false;

            Quaternion rotation = (Tools.pivotRotation == PivotRotation.Global) ? Quaternion.identity : currentRotation;
            Vector3 newPosition = Handles.PositionHandle(toMove, rotation);
            if (newPosition != toMove)
            {
                hasChanged = true;
                toMove = AttemptToApplyGrid(newPosition, snap);
            }
            return (toMove);
        }

        public static Vector3 AttemptToApplyGrid(Vector3 position, float gridSize, float defaultWhenNoControl = 0)
        {
            if (gridSize != 0 && (Event.current.control || Event.current.alt || Event.current.shift))
            {
                if (Event.current.control && !Event.current.alt)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, gridSize),
                        RoundToGrid(position.y, gridSize),
                        RoundToGrid(position.z, gridSize));
                }
                else if (Event.current.alt && !Event.current.control)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, gridSize * 2),
                        RoundToGrid(position.y, gridSize * 2),
                        RoundToGrid(position.z, gridSize * 2));
                }
                else if (Event.current.alt && Event.current.control)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, gridSize * 8),
                        RoundToGrid(position.y, gridSize * 8),
                        RoundToGrid(position.z, gridSize * 8));
                }
            }
            else if (defaultWhenNoControl != 0)
            {
                position = new Vector3(
                       RoundToGrid(position.x, defaultWhenNoControl),
                       RoundToGrid(position.y, defaultWhenNoControl),
                       RoundToGrid(position.z, defaultWhenNoControl));
            }
            return (position);
        }

        public static float AttemptToApplyGrid(float position, float gridSize, float defaultWhenNoControl = 0)
        {
            if (gridSize != 0 && (Event.current.control || Event.current.alt || Event.current.shift))
            {
                if (Event.current.control && !Event.current.alt)
                {
                    position = RoundToGrid(position, gridSize);
                }
                else if (Event.current.alt && !Event.current.control)
                {
                    position = RoundToGrid(position, gridSize * 2);
                }
                else if (Event.current.alt && Event.current.control)
                {
                    position = RoundToGrid(position, gridSize * 8);
                }
            }
            else if (defaultWhenNoControl != 0)
            {
                position = RoundToGrid(position, defaultWhenNoControl);
            }
            return (position);
        }

        private static float RoundToGrid(float input, float snap)
        {
            return snap * Mathf.Round((input / snap));
        }

        public static Quaternion DoHandleRotation(Transform toMove, bool record, out bool hasChanged)
        {
            hasChanged = false;
            if (record)
            {
                Undo.RecordObject(toMove.gameObject.transform, "handle Point rotation");
            }

            Quaternion newRotation = Handles.RotationHandle(toMove.rotation, toMove.position);
            if (newRotation != toMove.rotation)
            {
                hasChanged = true;
                toMove.rotation = newRotation;
                if (record)
                {
                    EditorUtility.SetDirty(toMove);
                }
            }
            return (newRotation);
        }

        public static void DoHandleScale(Transform toMove, bool record, out bool hasChanged)
        {
            hasChanged = false;
            if (record)
            {
                Undo.RecordObject(toMove.gameObject.transform, "handle Point move");
            }

            Quaternion rotation = (Tools.pivotRotation == PivotRotation.Global) ? Quaternion.identity : toMove.rotation;
            Vector3 newScale = Handles.ScaleHandle(toMove.localScale, toMove.position, rotation, HandleUtility.GetHandleSize(toMove.position));
            if (newScale != toMove.localScale)
            {
                hasChanged = true;
                toMove.localScale = newScale;
                if (record)
                {
                    EditorUtility.SetDirty(toMove);
                }
            }
        }

        public static void DoHandleRect(Transform toMove, bool record, out bool hasChanged)
        {
            hasChanged = false;
            if (record)
            {
                Undo.RecordObject(toMove.gameObject.transform, "handle Point move");
            }
        }

        public static void DrawSphereArrow(Color newColor, Vector3 position, float sizeRatio = 1f)
        {
            DrawSphereArrow(newColor, position, ExtSceneView.GetSceneViewCameraTransform().rotation, sizeRatio);
        }

        public static void DrawSphereArrow(Color newColor, Vector3 position, Quaternion rotation, float sizeRatio = 1f)
        {
            Color old = Handles.color;
            Handles.color = newColor;
            Handles.CircleHandleCap(0, position, rotation, HandleUtility.GetHandleSize(position) * sizeRatio, EventType.Repaint);
            Handles.color = old;
        }
    }
}