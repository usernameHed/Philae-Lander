using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Rotate localy the player")]
public class CoinAnim : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float turnRate = 200f;

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private Transform objToRotate = null;

    private void RotateCoin()
    {
        objToRotate.Rotate(objToRotate.up, turnRate * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        RotateCoin();
    }
}
