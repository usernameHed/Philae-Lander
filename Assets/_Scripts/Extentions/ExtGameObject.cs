using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class ExtGameObject
{
    /// <summary>
    /// Find a GameObject even if it's disabled.
    /// </summary>
    /// <param name="name">The name.</param>
    public static GameObject FindWithDisabled(this GameObject go, string name)
    {
        var temp = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        var obj = new GameObject();
        foreach (GameObject o in temp)
        {
            if (o.name == name)
            {
                obj = o;
            }
        }
        return obj;
    }

    /// <summary>
    /// create a gameObject, with a set of components
    /// ExtGameObject.CreateGameObject("game object name", Vector3.zero, Quaternion.identity, Vector3 Vector.One, Component [] components, Transform parent)
    /// </summary>
    public static GameObject CreateGameObject(string name, 
        Vector3 position,
        Quaternion rotation,
        Vector3 localScale,
        Component [] components, Transform parent)
    {
        GameObject newObject = new GameObject(name + " " + parent.childCount);
        newObject.SetActive(false);
        newObject.transform.SetParent(parent);
        newObject.transform.SetAsLastSibling();
        newObject.transform.localScale = localScale;

        for (int i = 0; i < components.Length; i++)
        {
            newObject.AddComponent(components[i]);
        }
        return (newObject);
    }

    /// <summary>
    /// hide all renderer
    /// </summary>
    /// <param name="toHide">apply this to a transform, or a gameObject</param>
    /// <param name="hide">hide (or not)</param>
    public static void HideAllRenderer(this Transform toHide, bool hide = true)
    {
        HideAllRenderer(toHide.gameObject, hide);
    }
    public static void HideAllRenderer(this GameObject toHide, bool hide = true)
    {
        Renderer[] allrenderer = toHide.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < allrenderer.Length; i++)
        {
            allrenderer[i].enabled = !hide;
        }
    }

    /// <summary>
    /// Returns true if the GO is null or inactive
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static bool IsNullOrInactive(this GameObject go)
    {
        return ((go == null) || (!go.activeSelf));
    }

    /// <summary>
    /// Returns true if the GO is not null and is active
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static bool IsActive(this GameObject go)
    {
        return ((go != null) && (go.activeSelf));
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
            t.gameObject.SetLayerRecursively(layer);
        }            
    }

    /// <summary>
    /// activate recursivly the Colliders
    /// use: gameObject.SetCollisionRecursively(false);
    /// </summary>
    public static void SetCollisionRecursively(this GameObject gameObject, bool tf)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = tf;
        }            
    }

    /// <summary>
    /// but what if you want the collision mask to be based on the weapon’s layer?
    /// It’d be nice to set some weapons to “Team1” and others to “Team2”,
    /// perhaps, and also to ensure your code doesn’t break if you change
    /// the collision matrix in the project’s Physics Settings
    /// 
    /// USE:
    /// if(Physics.Raycast(startPosition, direction, out hitInfo, distance,
    ///                          weapon.gameObject.GetCollisionMask()) )
    ///{
    ///    // Handle a hit
    ///}
    ///
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int GetCollisionMask(this GameObject gameObject, int layer = -1)
    {
        if (layer == -1)
            layer = gameObject.layer;

        int mask = 0;
        for (int i = 0; i < 32; i++)
            mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

        return mask;
    }

    /// <summary>
    /// is the object's layer in the specified layermask
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool IsInLayerMask(this GameObject gameObject, LayerMask mask)
    {
        return ((mask.value & (1 << gameObject.layer)) > 0);
    }

    /// <summary>
    /// Is a gameObject grounded or not ?
    /// </summary>
    /// <param name="target">object to test for grounded</param>
    /// <param name="dirUp">normal up of the object</param>
    /// <param name="distToGround">dist to test</param>
    /// <param name="layerMask">layermask</param>
    /// <param name="queryTriggerInteraction"></param>
    /// <param name="marginDistToGround">aditionnal margin to the distance</param>
    /// <returns></returns>
    public static bool IsGrounded(GameObject target, Vector3 dirUp, float distToGround, int layerMask, QueryTriggerInteraction queryTriggerInteraction, float marginDistToGround = 0.1f)
    {
        return Physics.Raycast(target.transform.position, -dirUp, distToGround + marginDistToGround, layerMask, queryTriggerInteraction);
    }
}
