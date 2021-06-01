
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor
{
    public static class ExtComponentsEditor
    {
        public static T AddComponentByCopyPast<T>(this GameObject go, T toAdd) where T : Component
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(toAdd);
            T currentComponent = go.GetComponent<T>();
            if (currentComponent != null)
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(currentComponent);
            }
            else
            {
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
            }
            return (go.GetComponent<T>());
        }

        
        /// <summary>
        /// change le layer de TOUT les enfants
        /// </summary>
        //use: myButton.gameObject.SetLayerRecursively(LayerMask.NameToLayer(“UI”));
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform t in gameObject.transform)
            {
                SetLayerRecursively(t.gameObject, layer);
            }
        }

        public static void CopyRectTransformTo(RectTransform source, RectTransform destination)
        {
            destination.offsetMin = source.offsetMin;
            destination.offsetMax = source.offsetMax;
            destination.pivot = source.pivot;
            destination.anchorMin = source.anchorMin;
            destination.anchorMax = source.anchorMax;
            destination.anchoredPosition = source.anchoredPosition;
            destination.sizeDelta = source.sizeDelta;
        }

        public static void CopyPartialCameraTo(Camera source, Camera destination)
        {
            destination.transform.position = source.transform.position;
            destination.transform.rotation = source.transform.rotation;
            destination.fieldOfView = source.fieldOfView;
            destination.allowDynamicResolution = source.allowDynamicResolution;
            destination.aspect = source.aspect;
            destination.cameraType = source.cameraType;
            destination.focalLength = source.focalLength;
            destination.lensShift = source.lensShift;
            destination.orthographic = source.orthographic;
            destination.orthographicSize = source.orthographicSize;
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name" || prop.PropertyType.Equals(typeof(Material)) || prop.PropertyType.Equals(typeof(Material[]))) continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }

    }
}