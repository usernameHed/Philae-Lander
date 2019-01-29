using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRotateToGround : RotateToGround
{
    [FoldoutGroup("GamePlay"), Tooltip("speed of rotation to ground"), SerializeField]
    public float speedRotate = 5f;

    private void Start()
    {
        InstantRotate();
    }

    private void FixedUpdate()
    {
        RotateObject(speedRotate);
    }
}
