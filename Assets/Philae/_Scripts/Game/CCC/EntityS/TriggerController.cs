
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour, IKillable
{
    [SerializeField, Tooltip("ref script")]
    public EntityTriggerManager entityTriggerManager;

    public void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        entityTriggerManager.OwnTriggerEnter(other);
    }
    private void OnTriggerStay(Collider other)
    {
        entityTriggerManager.OwnTriggerStay(other);
    }
    private void OnTriggerExit(Collider other)
    {
        entityTriggerManager.OwnTriggerExit(other);
    }
    private void OnCollisionEnter(Collision collision)
    {
        entityTriggerManager.OwnCollisionEnter(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        entityTriggerManager.OwnCollisionStay(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        entityTriggerManager.OwnCollisionExit(collision);
    }
    */

    public void Kill()
    {
        entityTriggerManager.Kill();
    }
}
