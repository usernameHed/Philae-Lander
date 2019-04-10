﻿using DynamicShadowProjector;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAdvancedCheck : MonoBehaviour
{
    [FoldoutGroup("GamePLay"), SerializeField, Tooltip("ref")]
    private float minDistShadow = 3.5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private DrawTargetObject drawTargetObject = default;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private Projector projector = default;
    public GameObject GetObjProjector() => projector.gameObject;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private BaseGravity baseGravity = default;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private Rigidbody rb = default;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GroundCheck groundCheck;

    private void Awake()
    {
        drawTargetObject.rotateToVector = true;
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 dirGravity = baseGravity.GetMainAndOnlyGravity();
        //Vector3 posGravity = groundCheck.ResearchInitialGround(false);
        Vector3 posGravity = baseGravity.GetPointGravityDown();
        drawTargetObject.posToLookAt = (rb.position - (rb.transform.up * 999));
        projector.farClipPlane = Mathf.Max(minDistShadow, 1 + (rb.transform.position - posGravity).magnitude);

        //ExtDrawGuizmos.DebugWireSphere(posGravity, Color.red, 0.5f, 5f);
        //Debug.DrawLine(rb.position, posGravity, Color.cyan, 5f);
    }
}
