using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSwitcher : SingletonMono<PlanetSwitcher>
{
    [FoldoutGroup("GamePlay"), Tooltip("planette qui attire"), SerializeField]
    private List<Rigidbody> planetsList;

    /// <summary>
    /// return the closest rigidbody from the other gameObject
    /// </summary>
    public Rigidbody GetClosestRigidBody(GameObject other)
    {
        Rigidbody closestRb = planetsList[0];
        float closestDist = Vector3.SqrMagnitude(other.transform.position - planetsList[0].position);

        for (int i = 1; i < planetsList.Count; i++)
        {
            float dist = Vector3.SqrMagnitude(other.transform.position - planetsList[i].position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestRb = planetsList[i];
            }
        }

        return (closestRb);
    }
}
