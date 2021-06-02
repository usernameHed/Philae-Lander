using UnityEngine;

using System.Collections.Generic;

/// <summary>
/// AttractObject Description
/// </summary>
public class AttractorOverlap : MonoBehaviour
{
    [Tooltip("radius d'attraction"), SerializeField]
    private float radius = 10f;
    [Tooltip("radius d'attraction"), SerializeField]
    private float attractionPlayer = 0.1f;

    [Tooltip("force d'attraction"), SerializeField]
    private float strenght = 1000f;
    [Tooltip("layer d'attration"), SerializeField]
    private string [] layerToAttract = new string[] { "Player" }; //select layer 8 (metallica and colider)

    [Tooltip("opti fps")]
    public FrequencyTimer updateTimer;

    [Tooltip("opti fps"), SerializeField]
    private List<Rigidbody> listObjectOverlaping = new List<Rigidbody>();

    private Collider[] overlapResults = new Collider[30];
    private int numFound = 0;

    /// <summary>
    /// attire l'objet vers le centre de la planette
    /// </summary>
    private void Attract(Rigidbody rbPlanet, bool player = false)
    {
        if (!rbPlanet)
            return;

        Vector3 direction = transform.position - rbPlanet.position;
        //float distance = direction.magnitude;

        //float forceMagnitude = (rb.mass * rbPlanet.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * strenght * ((player) ? attractionPlayer : 1);//forceMagnitude;

        rbPlanet.AddForce(force);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    /// <summary>
    /// remplie la lsite à chaque X temps des objets proche du satelites
    /// </summary>
    private void SetListOverlap()
    {
        numFound = Physics.OverlapSphereNonAlloc(transform.position, radius, overlapResults, LayerMask.GetMask(layerToAttract));

        for (int i = 0; i < numFound; i++)
        {
            Debug.DrawLine(transform.position, overlapResults[i].transform.position, Color.red);
        }
    }

    /// <summary>
    /// attique chaque objet se trouvant dans la liste, chaque fixedUpdate;
    /// </summary>
    private void AttractObject()
    {
        for (int i = 0; i < numFound; i++)
        {
            //PlayerController PC = overlapResults[i].gameObject.GetComponent<PlayerController>();
            Attract(overlapResults[i].gameObject.GetComponent<Rigidbody>());
        }
    }

    private void Update()
    {
        //optimisation des fps
        if (updateTimer.Ready())
        {
            SetListOverlap();
        }
    }

    private void FixedUpdate()
    {
        AttractObject();
    }
}
