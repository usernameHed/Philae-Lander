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

    [FoldoutGroup("Object"), Tooltip("ref rb"), SerializeField]
    private Rigidbody rb;

    [FoldoutGroup("Debug"), Tooltip("ratio gravity"), SerializeField]
    private float ratioGravity = 1;
    [FoldoutGroup("Debug"), Tooltip("ratio gravity"), SerializeField]
    private bool useGravity = true;


    private Vector3 dirGravity = Vector3.zero;

    public Vector3 GetDirGravity()
    {
        return (dirGravity);
    }

    /// <summary>
    /// activate or not gravity
    /// </summary>
    /// <param name="activate"></param>
    public void SetUseGravity(bool useIt)
    {
        useGravity = useIt;
    }

    public void SetPlanetList(Rigidbody planet)
    {
        planetsList.Clear();
        planetsList.Add(planet);
    }
    public void SetPlanetList(List<Rigidbody> list)
    {
        planetsList = list;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        DoAllAttractor();
    }

    public void SetRatioGravity(float ratio)
    {
        ratioGravity = ratio;
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
        Vector3 force = dirGravity * forceMagnitude * ratioGravity;


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
        if (useGravity)
            DoAllAttractor();
    }
}
