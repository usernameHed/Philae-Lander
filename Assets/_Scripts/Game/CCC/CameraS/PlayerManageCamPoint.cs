using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Rotate Cam point")]
public class PlayerManageCamPoint : MonoBehaviour
{
    [FoldoutGroup("Object"), Tooltip("point child who rotate"), SerializeField]
    private readonly Transform forward;

    [FoldoutGroup("Object"), Tooltip("point child who rotate"), SerializeField]
    private readonly PlayerRotateCamPoint playerRotateCamPoint;

    private void Update()
    {

    }
}
