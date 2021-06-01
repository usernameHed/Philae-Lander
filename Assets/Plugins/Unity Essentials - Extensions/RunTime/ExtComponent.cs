using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    public static class ExtComponent
    {
        public static Renderer[] GetAllRendererInGameObjects(GameObject gameObject, int depth = 99, bool startWithOurSelf = false, bool includeInactive = false)
        {
            List<Renderer> results = new List<Renderer>();
            if (startWithOurSelf)
            {
                Renderer[] result = gameObject.GetComponents<Renderer>();
                for (int i = 0; i < result.Length; i++)
                {
                    if (!includeInactive && !result[i].gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                    if (result[i] is ParticleSystemRenderer)
                    {
                        continue;
                    }
                    results.Add(result[i]);
                }
            }

            foreach (Transform t in gameObject.transform)
            {
                if (depth - 1 <= 0)
                    break;
                results.AddRange(GetAllRendererInGameObjects(t.gameObject, depth - 1, true, includeInactive));
            }

            return results.ToArray();
        }

        /// <summary>
        /// Gets or add a component. Usage example:
        /// use: BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T result = component.GetComponent<T>();
            if (result == null)
            {
                result = component.gameObject.AddComponent<T>();
            }
            return result;
        }
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T result = obj.GetComponent<T>();
            if (result == null)
            {
                result = obj.AddComponent<T>();
            }
            return result;
        }

    }
}