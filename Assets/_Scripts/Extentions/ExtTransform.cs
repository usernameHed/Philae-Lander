using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtTransform
{
    /// <summary>
    /// get real pos world of rect transform ??
    /// </summary>
    public static Rect GetWorldRect(RectTransform rt, Vector2 scale)
    {
        // Convert the rectangle to world corners and grab the top left
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 topLeft = corners[0];

        // Rescale the size appropriately based on the current Canvas scale
        Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

        return new Rect(topLeft, scaledSize);
    }

    public static RectTransform ResetPos(this RectTransform transform)
    {
        transform.localPosition = Vector3.zero;
        return (transform);
    }

    public static RectTransform ResetScale(this RectTransform transform)
    {
        transform.localScale = Vector3.one;
        transform.offsetMin = new Vector2(0, 0);
        transform.offsetMax = new Vector2(0, 0);
        return (transform);
    }

    /// <summary>
    /// Remove every childs in a transform
    /// </summary>
    public static Transform ClearChild(this Transform transform)
	{
        if (Application.isPlaying)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        else
        {
            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        return (transform);
	}
    public static Transform ClearChildImediat(this Transform transform)
    {
        int children = transform.childCount;

        for (int i = children - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
        return (transform);
    }
    public static List<Transform> GetAllChild(this Transform transform)
    {
        List<Transform> allChild = new List<Transform>();
        foreach (Transform child in transform)
        {
            allChild.Add(child);
        }
        return (allChild);
    }
    public static Transform ActiveAllChild(this Transform transform, bool active, Transform exeption = null)
    {
        foreach (Transform child in transform)
        {
            if (exeption && child == exeption)
                child.gameObject.SetActive(!active);
            child.gameObject.SetActive(active);
        }
        return (transform);
    }
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    //use: transform.ResetTransformation();
    public static void ResetTransform(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
    public static void SetXY(this Transform transform, float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }
    public static void SetXZ(this Transform transform, float x, float z)
    {
        transform.position = new Vector3(x, transform.position.y, z);
    }
    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    public static void SetYZ(this Transform transform, float y, float z)
    {
        transform.position = new Vector3(transform.position.x, y, z);
    }
    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    /// <summary>
    /// Sets the position of a transform's children to zero.
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="recursive">Also reset ancestor positions?</param>
    public static void ResetChildPositions(this Transform transform, bool recursive = false)
    {
        foreach (Transform child in transform)
        {
            child.position = Vector3.zero;

            if (recursive)
            {
                child.ResetChildPositions(recursive);
            }
        }
    }

    /// <summary>
    /// Makes the given game objects children of the transform.
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="children">Game objects to make children.</param>
    public static void AddChildren(this Transform transform, GameObject[] children)
    {
        Array.ForEach(children, child => child.transform.parent = transform);
    }
    public static void AddChildren(this Transform transform, Component[] children)
    {
        Array.ForEach(children, child => child.transform.parent = transform);
    }
}
