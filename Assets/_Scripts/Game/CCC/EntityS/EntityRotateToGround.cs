using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRotateToGround : RotateToGround
{
    [FoldoutGroup("GamePlay"), Tooltip("speed of rotation to ground"), SerializeField, OnValueChanged("UpdateSpeed")]
    public float speedRotate = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed of rotation to ground"), SerializeField, OnValueChanged("UpdateSpeed")]
    public float speedLerpRaulBack = 5f;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    private SmoothNormals smoothNormals = null;

    private float tmpSpeed;

    private void Start()
    {
        UpdateSpeed();
        InstantRotate(smoothNormals.GetSmoothedNormalPlayer());
    }
    private void UpdateSpeed()
    {
        tmpSpeed = speedRotate;
    }

    public void SetNewTempSpeed(float newSpeed)
    {
        speedRotate = newSpeed;
    }

    private void FixedUpdate()
    {
        RotateObject(speedRotate, smoothNormals.GetSmoothedNormalPlayer());
        speedRotate = Mathf.Lerp(speedRotate, tmpSpeed, Time.deltaTime * speedLerpRaulBack);
    }
}
