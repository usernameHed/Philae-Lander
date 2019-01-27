using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour, IKillable
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    public PlayerController playerController;

    public void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }

    public void Kill()
    {
        playerController.Kill();
    }
}
