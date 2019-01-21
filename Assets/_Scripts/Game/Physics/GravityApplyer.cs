using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// AttractObject Description
/// </summary>
public class GravityApplyer : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("planette qui attire"), SerializeField]
    private List<Rigidbody> planetsList;

    [FoldoutGroup("Object"), Tooltip("opti fps"), SerializeField]
    private Rigidbody rb;

    private Vector3 dirGravity = Vector3.zero;

    public Vector3 GetDirGravity()
    {
        return (dirGravity);
    }

    private void Awake()
    {
        DoAllAttractor();
    }

    /// <summary>
    /// attire l'objet vers le centre de la planette
    /// </summary>
    private void Attract(Rigidbody rbPlanet)
    {
        Vector3 direction = rb.position - rbPlanet.position;
        float distance = direction.magnitude;

        float forceMagnitude = (rb.mass * rbPlanet.mass) / Mathf.Pow(distance, 2);

        dirGravity = direction.normalized;
        Vector3 force = dirGravity * forceMagnitude;


        rb.AddForce(-force);

        //Debug.DrawRay(rb.transform.position, dirGravity, Color.red, 0.5f);
    }

    /// <summary>
    /// attract object from multiple planet
    /// </summary>
    private void DoAllAttractor()
    {
        for (int i = 0; i < planetsList.Count; i++)
        {
            Attract(planetsList[i]);
        }
    }

    private void FixedUpdate()
    {
        DoAllAttractor();
    }
}
