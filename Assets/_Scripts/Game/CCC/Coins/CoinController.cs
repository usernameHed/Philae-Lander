using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class CoinController : MonoBehaviour, IPooledObject, IKillable
{
    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private bool isKinematic = false;
    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private Rigidbody rb;

    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private int reward = 1;

    private void Awake()
    {
        SetKinematic();
    }

    private void SetKinematic()
    {
        rb.isKinematic = isKinematic;
    }

    private void FixedUpdate()
    {

    }

    public void OnObjectSpawn()
    {
        rb.transform.position = transform.position;
        //throw new System.NotImplementedException();
    }

    public void OnDesactivePool()
    {
        //throw new System.NotImplementedException();
    }

    public void Kill()
    {
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }

    public void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }
}
