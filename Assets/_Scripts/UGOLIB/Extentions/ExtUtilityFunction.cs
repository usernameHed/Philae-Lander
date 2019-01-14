using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;


/// <summary>
/// Fonctions utile
/// <summary>
public static class ExtUtilityFunction
{
    #region core script

    //[MenuItem("Tools/Clear Console %#c")] // CMD + SHIFT + C
    public static void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    /// <summary>
    /// is target on screen ??
    /// </summary>
    public static bool IsTargetOnScreen(Camera cam, Transform target, float xMargin = 0, float yMargin = 0, Renderer render = null)
    {
        if (!cam)
            return (false);
        Vector3 boundExtent = (render == null) ? Vector3.zero : render.bounds.extents;

        Vector3 bottomCorner = cam.WorldToViewportPoint(target.position - boundExtent);
        Vector3 topCorner = cam.WorldToViewportPoint(target.position + boundExtent);

        return (topCorner.x >= -xMargin && bottomCorner.x <= 1 + xMargin && topCorner.y >= -yMargin && bottomCorner.y <= 1 + yMargin);
    }

    public static bool IsClose(float x1, float x2, float factor)
    {
        if (Mathf.Abs(x1 - x2) < factor)
            return (true);
        return (false);
    }
    public static bool IsClose(Vector3 A, Vector3 B, float factor)
    {
        Vector3 offset = A - B;
        float sqrLen = offset.sqrMagnitude;

        // square the distance we compare with
        if (sqrLen < factor)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    public static bool AlmostEquals(this double double1, double double2, double precision = 0.00001)
    {
        return (Math.Abs(double1 - double2) <= precision);
    }

    /// <summary>
    /// Test si l'objet est dans la range d'un autre
    /// (pour visualiser la range dans l'éditeur, attacher le script DrawSolidArc
    ///  avec les valeur fovRange et fovAngle sur l'objet "first")
    /// </summary>
    /// <param name="first">objet emmeteur (avec son angle et sa range)</param>
    /// <param name="target">l'objet cible à tester</param>
    /// <param name="fovRange">La range du joueur</param>
    /// <param name="fovAngle">l'angle du joueur</param>
    /// <param name="withRaycast">Doit-on utiliser un raycast ?</param>
    /// <param name="layerMask">Si on utilise un raycast, sur quel layer ?</param>
    /// <returns>Retourne si oui ou non l'objet cible est dans la zone !</returns>
    public static bool isInRange(Transform first, Transform target, float fovRange, float fovAngle, bool checkDistance = false, bool withRaycast = false, int layerMask = -1)
    {
        Vector3 B = target.transform.position - first.position;
        Vector3 C = Quaternion.AngleAxis(90 + fovAngle / 2, -first.forward) * -first.right;

        RaycastHit hit;
        if (ExtQuaternion.SignedAngleBetween(first.up, B, first.up) <= ExtQuaternion.SignedAngleBetween(first.up, C, first.up) || fovAngle == 360)
        {
            //on est dans le bon angle !
            //est-ce qu'on check la distance ?
            if (checkDistance)
            {
                //on test la distance mais sans raycast ?
                if (!withRaycast)
                {
                    Vector3 offset = target.position - first.position;
                    float sqrLen = offset.sqrMagnitude;
                    if (sqrLen < fovRange * fovRange)
                        return (true);
                    return (false);
                }
                //on test la distance, puis le raycast ?
                else
                {
                    Vector3 offset = target.position - first.position;
                    float sqrLen = offset.sqrMagnitude;
                    if (sqrLen < fovRange * fovRange)
                    {
                        if (Physics.Raycast(first.position, B, out hit, fovRange * fovRange, layerMask))
                        {
                            if (hit.transform.GetInstanceID() == target.GetInstanceID())
                                return (true);
                        }
                    }
                    return (false);
                }
            }
            //ne check pas la distance !
            else
            {
                //si on ne check pas de raycast, alors on est dans la range !
                if (!withRaycast)
                    return (true);
                //ici on ne check pas la distance, mais le raycast !
                else
                {
                    if (Physics.Raycast(first.position, B, out hit, Mathf.Infinity, layerMask))
                    {
                        if (hit.transform.GetInstanceID() == target.GetInstanceID())
                            return (true);
                    }
                    return (false);
                }
            }
            
        }
        //ici on est pas dans l'angle...
        return (false);
    }
    #endregion
}
