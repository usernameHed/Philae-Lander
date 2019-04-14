using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueSmoothNormals : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    protected float smoothSpeedPlayer = 2f;
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    protected bool calculateEveryFixedFrame = true;

    [FoldoutGroup("Object"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    protected UniqueGravity uniqueGravity = null;
    [FoldoutGroup("Debug"), Tooltip("Smoothed normals"), SerializeField, ReadOnly]
    protected Vector3 smoothedNormalPlayer;

    private void Start()
    {
        smoothedNormalPlayer = GetRotationOrientationDown();
    }
    
    public Vector3 GetSmoothedNormalPlayer()
    {
        return (smoothedNormalPlayer);
    }

    public virtual Vector3 GetRotationOrientationDown()
    {
        return (uniqueGravity.GetMainAndOnlyGravity());
    }

    protected virtual void CalculateSmoothNormal()
    {
        Vector3 actualNormal = GetRotationOrientationDown();
        smoothedNormalPlayer = Vector3.Lerp(smoothedNormalPlayer, actualNormal, Time.deltaTime * smoothSpeedPlayer);
    }

    private void FixedUpdate()
    {
        if (calculateEveryFixedFrame)
        {
            CalculateSmoothNormal();
        }
    }
}
