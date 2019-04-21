using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ExtComponent
{
    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    static public T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    /// <summary>
    /// Checks whether a component's game object has a component of type T attached.
    /// </summary>
    /// <param name="component">Component.</param>
    /// <returns>True when component is attached.</returns>
    public static bool HasComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() != null;
    }

    public static bool HasComponentInParent<T>(this Component component) where T : Component
    {
        return component.GetComponentInParent<T>() != null;
    }
    public static bool HasComponentInParent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponentInParent<T>() != null;
    }



    /// <summary>
    /// Checks whether a game object has a component of type T attached.
    /// </summary>
    /// <param name="gameObject">Game object.</param>
    /// <returns>True when component is attached.</returns>
    public static bool HasComponentOrInChild<T>(this GameObject gameObject) where T : Component
    {
        if (HasComponent<T>(gameObject))
            return (true);
        return HasComponentInChild<T>(gameObject);
    }

    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() != null;
    }

    public static bool HasComponentInChild<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponentInChildren<T>() != null;
    }

    /// <summary>
    /// Destroy component if present
    /// </summary>
    /// <returns>return true if object found and destroyed</returns>
    public static bool DestroyComponent<T>(this Component child, bool immediate = false) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result != null)
        {
            if (immediate)
            {
                GameObject.DestroyImmediate(child.gameObject.GetComponent<T>());
            }
            else
            {
                GameObject.Destroy(child.gameObject.GetComponent<T>());
            }
            return (true);
        }
        return false;
    }

    /// <summary>
    /// destroy all component inside that object
    /// use: gameObject.DestroyAllComponentInside<Renderer>();
    /// use: transform.DestroyAllComponentInside<Renderer>();
    /// use: transform.DestroyAllComponentInside<Renderer>(true, false);
    /// </summary>
    /// <param name="imediate">type of destruction: imediate, or normal</param>
    /// /// <param name="includeInactive">destroy also in inactive object</param>
    public static void DestroyAllComponentInside<T>(this GameObject child, bool immediate = false, bool includeInactive = false) where T : Component
    {
        DestroyAllComponentInside<T>(child);
    }
    public static void DestroyAllComponentInside<T>(this Component child, bool immediate = false, bool includeInactive = false) where T :  Component
    {
        T[] allComponent = child.GetComponentsInChildren<T>(includeInactive);

        for (int i = 0; i < allComponent.Length; i++)
        {
            if (immediate)
            {
                GameObject.DestroyImmediate(allComponent[i]);
            }
            else
            {
                GameObject.Destroy(allComponent[i]);
            }
        }
    }


    /// <summary>
    /// GameObject.GetComponentsInChildren(), only with a certain tag
    /// use: m_renderers = gameObject.GetComponentsInChildrenWithTag<Renderer>("Shield");
    /// </summary>
    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject gameObject, string tag)
        where T : Component
    {
        List<T> results = new List<T>();

        if (gameObject.CompareTag(tag))
            results.Add(gameObject.GetComponent<T>());

        foreach (Transform t in gameObject.transform)
            results.AddRange(t.gameObject.GetComponentsInChildrenWithTag<T>(tag));

        return results.ToArray();
    }

    /// <summary>
    /// Get a script on the parent
    /// Player player = collider.gameObject.GetComponentInAllParents<Player>();
    /// depth = 0: stop at first parent
    /// testCurrent: GetAlsoCOmponent In the current gameObject
    /// </summary>
    public static T GetComponentInAllParentsWithTag<T>(this GameObject gameObject, string tag, int depth = 99, bool testCurrent = false)
        where T : Component
    {
        if (testCurrent && gameObject.CompareTag(tag))
        {
            T result = gameObject.GetComponent<T>();
            if (result != null)
                return (result);
        }

        int currentDepth = 0;
        for (Transform t = gameObject.transform; t != null; t = t.parent)
        {
            if (!gameObject.CompareTag(tag))
            {
                currentDepth++;
                continue;
            }
                

            T result = t.GetComponent<T>();
            if (result != null)
                return result;
            currentDepth++;
            if (currentDepth >= depth)
                return (null);
        }

        return null;
    }

    /// <summary>
    /// Get a script on the parent
    /// Player player = collider.gameObject.GetComponentInAllParents<Player>();
    /// depth = 0: stop at first parent
    /// testCurrent: GetAlsoCOmponent In the current gameObject
    /// </summary>
    public static T GetComponentInAllParents<T>(this GameObject gameObject, int depth = 99, bool testCurrent = false)
        where T : Component
    {
        if (testCurrent)
        {
            T result = gameObject.GetComponent<T>();
            if (result != null)
                return (result);
        }

        int currentDepth = 0;
        for (Transform t = gameObject.transform; t != null; t = t.parent)
        {
            T result = t.GetComponent<T>();
            if (result != null)
                return result;
            currentDepth++;
            if (currentDepth >= depth)
                return (null);
        }

        return null;
    }

    public static T[] GetComponentsInAllParents<T>(this GameObject gameObject, int depth = -1)
        where T : Component
    {
        int currentDepth = 0;
        List<T> results = new List<T>();
        for (Transform t = gameObject.transform; t != null; t = t.parent)
        {
            T result = t.GetComponent<T>();
            if (result != null)
                results.Add(result);
            currentDepth++;
            if (depth != -1 && currentDepth >= depth)
                break;
        }
        return results.ToArray();
    }

}
