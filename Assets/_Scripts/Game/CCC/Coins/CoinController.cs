using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class CoinController : MonoBehaviour, IPooledObject, IKillable
{
    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private readonly bool isKinematic = false;

    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private readonly bool autoRotateAtStart = true;

    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private Rigidbody rb = default;

    [FoldoutGroup("Sound"), SerializeField, Tooltip("ref script")]
    public FmodEventEmitter SFX_kill;
    [FoldoutGroup("Sound"), SerializeField, Tooltip("ref script")]
    public FmodEventEmitter SFX_loop;

    private void Awake()
    {
        SetKinematic();
    }

    private void SetKinematic()
    {
        rb.isKinematic = isKinematic;
        if (autoRotateAtStart)
        {
            //here rotate

        }
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
        SoundManager.Instance.PlaySound(SFX_kill);
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }

    public void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }
}
