using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Say player can slide, or climb !")]
public class EntitySlide : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Range(0, 90), Tooltip("Each collision give us difference angle, to 0 (floor), up to 90 (wall). value of 10 = get blocked on very low surface, value of 70 = climb anything !")]
    public float marginForSlideFloor = 50;

    [FoldoutGroup("Debug"), Tooltip("Floor angle by default ?")]
    public float angleFloor = 0;

    [Space(10)]

    [FoldoutGroup("Debug"), ReadOnly, Tooltip("is colliding on wall ?")]
    public bool canSlide = false;

    [FoldoutGroup("Debug"), ReadOnly, Tooltip("main Straff direction")]
    private Vector3 playerStraff = Vector3.zero;

    public Vector3 GetStraffDirection()
    {
        return (playerStraff);
    }
    /// <summary>
    /// called by PlayerMove after sliding
    /// </summary>
    public void JustMove()
    {
        canSlide = false;
    }

    /*/// <summary>
    /// take an angle in parameter.
    /// if the angle is close to 0, or 180, we touch floor or ceilling, reutrn false;
    /// </summary>
    private bool IsGoodAngleForSliding(Vector3 angleVector)
    {
        float closeToFloor = ExtQuaternion.GetAngleFromVector3(angleVector, Vector3.up);
        float closeToCeilling = ExtQuaternion.GetAngleFromVector3(angleVector, -Vector3.up);
        
        if (ExtQuaternion.IsAngleCloseToOtherByAmount(angleFloor, closeToFloor, marginForSlideFloor))
        {
            //Debug.DrawRay(transform.position, angleVector, Color.red, 3f);
            return (false);
        }            

            
        return (true);
    }*/

    /// <summary>
    /// calculate the normal direction, based on the hit forward
    /// </summary>
    public void CalculateStraffDirection(Vector3 normalHit)
    {
        //Vector3 gravityNormal = 
    }
}
