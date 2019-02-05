using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySwitch : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float radiusOverlap = 2f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float timeBeforeStartOverlapping = 0.3f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private string[] layerSwitch = new string[] { "Walkable/Up"};


    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController;

    private Collider[] results = new Collider[5];
    private FrequencyCoolDown coolDownOverlap = new FrequencyCoolDown();
    private int layermask;
    private Vector3 dirGravityJump;

    private void Awake()
    {
        layermask = LayerMask.GetMask(layerSwitch);
    }

    private void OverlapTest()
    {
        int number = Physics.OverlapSphereNonAlloc(entityController.rb.position, radiusOverlap, results, layermask, QueryTriggerInteraction.Ignore);
        Debug.Log("Test Overlap ??");
        ExtDrawGuizmos.DebugWireSphere(entityController.rb.position, Color.blue, radiusOverlap, 0.1f);
        for (int i = 0; i < number; i++)
        {
            Vector3 dirPlayerObject = results[i].transform.position - entityController.rb.position;
            float dotPlayer = ExtQuaternion.DotProduct(dirPlayerObject, dirGravityJump);
            if (dotPlayer > 0)
            {
                Debug.Log("cet objet est dans la direction oposé de l'autre !");
                //Vector3 closestPos = results[i].transform.G
                Debug.DrawLine(entityController.rb.position, results[i].transform.position, Color.cyan, 5f);
            }
            //test l'objet le plus proche (le point du mesh le plus proche surtout !)
            //il faut pas que ce soit dans la direction de la où on vient... (dirGravityJump)
        }
    }

    public void JustJumped(Vector3 dirGravity)
    {
        coolDownOverlap.StartCoolDown(timeBeforeStartOverlapping);
        dirGravityJump = dirGravity;
    }
    public void ResetSwitch()
    {
        coolDownOverlap.Reset();
    }

    private void FixedUpdate()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir
             && coolDownOverlap.IsReady())
        {
            OverlapTest();
        }            
    }
}
