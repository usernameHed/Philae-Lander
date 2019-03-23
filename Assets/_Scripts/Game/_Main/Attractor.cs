using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour, IKillable
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float radius = 3f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float gravityAdd = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private string[] layersRaycast = new string[] { "Player" };

    [FoldoutGroup("Object"), SerializeField, Tooltip("raycast to ground layer")]
    private EntityGravity playerGravity = null;

    
    private Collider[] overlapResults = new Collider[10];
    [FoldoutGroup("Debug"), SerializeField, Tooltip("raycast to ground layer"), ReadOnly]
    private Rigidbody[] rigidBody = new Rigidbody[10];

    public FrequencyTimer timer;
    private int numLastFound = 0;

    private void Start()
    {
        Init();
    }

    private void Init()
    {

    }

    private void UpdateObjectToAttract()
    {
        int raycastLayerMask = LayerMask.GetMask(layersRaycast);
        numLastFound = Physics.OverlapSphereNonAlloc(transform.position, 10f, overlapResults, raycastLayerMask);

        for (int i = 0; i < numLastFound; i++)
        {
            rigidBody[i] = overlapResults[i].transform.GetComponent<Rigidbody>();

            //Debug.DrawLine(transform.position, overlapResults[i].transform.position, Color.red);
        }
    }

    private void ApplyGravity()
    {
        for (int i = 0; i < numLastFound; i++)
        {
            Vector3 dirGravity = rigidBody[i].transform.position - transform.position;
            Vector3 orientationDown = -dirGravity * playerGravity.Gravity * (gravityAdd - 1) * Time.fixedDeltaTime;
            //Debug.DrawRay(rb.transform.position, orientationDown, Color.blue, 5f);
            rigidBody[i].velocity += orientationDown;
        }
    }

    private void FixedUpdate()
    {
        if (timer.Ready())
        {
            //ExtDrawGuizmos.DebugWireSphere(transform.position, Color.red, radius, 0.1f);
            UpdateObjectToAttract();
        }

        ApplyGravity();
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void GetHit(int amount, Vector3 posAttacker)
    {
        
    }
}
