﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EntityRaycastForward : MonoBehaviour
{
    public struct InfoHit
    {
        public Vector3 posA;
        public Vector3 posB;
        public bool hasHit;
        public Vector3 hitPoint;
        public Vector3 normal;

        public InfoHit(Vector3 _posA, Vector3 _posB,  bool _hasHit, Vector3 _hitPoint, Vector3 _normal)
        {
            posA = _posA;
            posB = _posB;
            hasHit = _hasHit;
            hitPoint = _hitPoint;
            normal = _normal;
        }
    }

    [FoldoutGroup("Forward"), Tooltip("number of steps"), SerializeField]
    private bool showPoint = true;

    [FoldoutGroup("Forward"), Tooltip("number of steps"), Range(0, 200), SerializeField]
    private int stepsForward = 5;
    [FoldoutGroup("Forward"), Tooltip(""), SerializeField]
    private float backWardRatioWhenHit = 0.1f;

    [FoldoutGroup("Down"), Tooltip(""), Range(0, 3), SerializeField]
    private float distDownRaycast = 1f;
    [FoldoutGroup("Down"), Tooltip(""), SerializeField]
    private float distDownConsideredAsTooHight = 0.4f;
    [FoldoutGroup("Down"), Tooltip(""), SerializeField]
    private bool doBackWardFromNormal = true;


    [FoldoutGroup("SphereCast"), Tooltip(""), Range(0, 2), SerializeField]
    public float sizeRadiusSphereCast = 0.3f;
    [FoldoutGroup("SphereCast"), Tooltip("dist to check forward player"), Range(0.1f, 10), SerializeField]
    private float distForwardRaycast = 0.6f;

    
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private Rigidbody rb = default;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityController entityController = default;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityAction _entityAction;

    [FoldoutGroup("Debug"), Tooltip("dist to check forward player"), SerializeField, ReadOnly]
    private float distMax = 0;
    [FoldoutGroup("Debug"), Tooltip("dist to check forward player"), SerializeField, ReadOnly]
    private float currentDistChecked = 0f;
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private float marginDistOk = 0.1f;
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private int maxStepsWhenAdditionnalCalculs = 30;

    private Vector3 focusForwardDir;
    private Vector3 focusRightDir;
    private bool breakLoop = false;

    private Vector3 lastPos;
    private Vector3 lastNormal;
    public Vector3 GetLastPos() => lastPos;
    public Vector3 GetLastNormal() => lastNormal;
    private int hitCount = 0;

    private void SaveLastPos(Vector3 pos)
    {
        lastPos = pos;
    }

    private void SaveLastNormal(Vector3 normal)
    {
        lastNormal = normal;
    }

    private InfoHit DoRayCastForward(Vector3 posA, Vector3 posB, Vector3 focusDir, float dist, bool isForward)
    {
        if (isForward)
        {
            if (showPoint)
                ExtDrawGuizmos.DebugWireSphere(posA, Color.blue, sizeRadiusSphereCast);
        }

        RaycastHit hitInfo;
        if (Physics.SphereCast(posA, sizeRadiusSphereCast, focusDir, out hitInfo,
                               dist, entityController.GetLayerMastAllWalkable(), QueryTriggerInteraction.Ignore))
        {
            Vector3 centerOnCollision = ExtUtilityFunction.SphereOrCapsuleCastCenterOnCollision(posA, focusDir, hitInfo.distance);


            if (showPoint)
            {
                Debug.DrawLine(posA, centerOnCollision, Color.red);
                if (isForward)
                {
                    ExtDrawGuizmos.DebugWireSphere(centerOnCollision, Color.cyan, sizeRadiusSphereCast);
                    Debug.DrawLine(centerOnCollision, hitInfo.point, Color.red);
                    ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.red, 0.1f);
                    SaveLastNormal(hitInfo.point - centerOnCollision);
                }
                else
                {
                    ExtDrawGuizmos.DebugWireSphere(centerOnCollision, Color.grey, sizeRadiusSphereCast);
                    Debug.DrawLine(centerOnCollision, hitInfo.point, Color.grey);
                    ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.grey, 0.1f);
                    SaveLastNormal(hitInfo.point - centerOnCollision);
                }
            }
            return (new InfoHit(posA, centerOnCollision, true, hitInfo.point, hitInfo.normal));
        }
        else
        {
            if (showPoint)
            {
                if (isForward)
                {
                    Debug.DrawLine(posA, posB, Color.green);
                    ExtDrawGuizmos.DebugWireSphere(posB, Color.cyan, sizeRadiusSphereCast);
                }
                else
                {
                    Debug.DrawLine(posA, posB, Color.grey);
                    ExtDrawGuizmos.DebugWireSphere(posB, Color.grey, sizeRadiusSphereCast);
                }
            }
            return (new InfoHit(posA, posB, false, ExtUtilityFunction.GetNullVector(), ExtUtilityFunction.GetNullVector()));
        }
    }

    /// <summary>
    /// here we have hit, prepare next move
    /// </summary>
    /// <param name="infoHit"></param>
    private void SetupWhenHit(ref InfoHit infoHit, ref Vector3 prevFocusDir)
    {
        currentDistChecked += (infoHit.posA - infoHit.posB).magnitude;

        //float dot = ExtQuaternion.DotProduct(infoHit.normal, -prevFocusDir);
        //Debug.Log(dot);

        //Vector3 refUp = ExtQuaternion.CrossProduct(prevFocusDir, focusRightDir);
        //Debug.DrawRay(infoHit.posB, refUp, Color.black);

        Vector3 downDir = ExtQuaternion.CrossProduct(infoHit.normal, focusRightDir);
        //SaveLastNormal(downDir);
        Vector3 upDir = -downDir;

        if (showPoint)
        {
            Debug.DrawRay(infoHit.posB, upDir * 0.3f, Color.magenta);
            Debug.DrawRay(infoHit.posB, downDir * 0.3f, Color.green);
        }


        infoHit.posA = infoHit.posB - (prevFocusDir * backWardRatioWhenHit);    //backward a little bit for the next move

        //then change dir
        prevFocusDir = upDir.normalized;

        infoHit.posB = infoHit.posA + prevFocusDir * GetDistForward();
    }

    /// <summary>
    /// here we have hit at down
    /// </summary>
    private bool SetupDownHit(ref InfoHit infoHitDown, ref Vector3 prevFocusDirDown, ref InfoHit infoHitForward, ref Vector3 prevFocusDirToChange)
    {
        float distDown = (infoHitDown.posA - infoHitDown.posB).magnitude;

        if (distDown > distDownConsideredAsTooHight)
        {
            //Debug.Log("dist down to high: " + distDown);
            currentDistChecked += distDown;

            //Vector3 refUp = ExtQuaternion.CrossProduct(prevFocusDirDown, focusRightDir);
            //Debug.DrawRay(infoHitDown.posB, refUp, Color.black);

            Vector3 downDir = ExtQuaternion.CrossProduct(infoHitDown.normal, focusRightDir);
            Vector3 upDir = -downDir;

            if (showPoint)
            {
                Debug.DrawRay(infoHitDown.posB, upDir * 0.3f, Color.magenta);
                Debug.DrawRay(infoHitDown.posB, downDir * 0.3f, Color.green);
            }

            if (doBackWardFromNormal)
                infoHitDown.posA = infoHitDown.posB - (-infoHitDown.normal * backWardRatioWhenHit);    //backward a little bit for the next move
            else
                infoHitDown.posA = infoHitDown.posB - (prevFocusDirDown * backWardRatioWhenHit);    //backward a little bit for the next move


            //then change dir
            prevFocusDirDown = upDir.normalized;

            infoHitDown.posB = infoHitDown.posA + prevFocusDirDown * GetDistForward();

            //set to forward our down result
            infoHitForward.posA = infoHitDown.posA;
            infoHitForward.posB = infoHitDown.posB;
            prevFocusDirToChange = prevFocusDirDown;

            return (true);
        }
        return (false);
    }

    private void SetupGoingDown(ref InfoHit infoHitDown, ref Vector3 prevFocusDown, ref InfoHit infoHit, ref Vector3 prevFocusDir, int step)
    {
        Vector3 backDir = infoHit.posA - infoHitDown.posB;
        InfoHit infoHitBack = DoRayCastForward(infoHitDown.posB, infoHit.posA, backDir, backDir.magnitude, false);   //do a raycast
        if (!infoHitBack.hasHit)
        {
            if (step == 0)
            {
                Debug.Log("here first step fails, do nothing");
                SaveLastPos(rb.transform.position);

                breakLoop = true;
                return;
            }

            hitCount++;
            //Debug.Log("Here NO HIT BACK !!! Reflect !");

            Vector3 posA = infoHitDown.posB;
            Vector3 dirReflect = ExtQuaternion.GetReflectAngle(-backDir, infoHit.posA - infoHit.posB);
            Vector3 posB = posA + dirReflect * (infoHit.posA - infoHit.posB).magnitude;

            currentDistChecked += GetDistDown();

            prevFocusDir = dirReflect;
            infoHit.posA = posA;
            infoHit.posB = posB;
        }
        else
        {
            hitCount = 0;
            Vector3 posA = infoHit.posA;
            Vector3 posB = infoHitBack.posB;
            Vector3 dirBack = posB - posA;

            Vector3 downDir = ExtQuaternion.CrossProduct(infoHitBack.normal, focusRightDir);
            Vector3 upDir = -downDir;

            if (showPoint)
            {
                Debug.DrawRay(posB, upDir * 0.3f, Color.magenta);
                Debug.DrawRay(posB, downDir * 0.3f, Color.green);
            }

            prevFocusDir = upDir;

            if (doBackWardFromNormal)
                infoHit.posA = posB - infoHitBack.normal * -backWardRatioWhenHit;    //backward a little bit for the next move
            else
                infoHit.posA = posB - dirBack * -backWardRatioWhenHit;    //backward a little bit for the next move


            //infoHit.posA = posB - dirBack * -backWardRatioWhenHit;

            infoHit.posB = infoHit.posA + prevFocusDir * GetDistForward();

            currentDistChecked += GetDistDown();

            /*
            Vector3 ToCenterTriangle = -ExtQuaternion.CrossProduct((posA - posB), focusRightDir).normalized;

            Debug.DrawRay(posB, ToCenterTriangle * backWardRatioUpWhenHit, Color.green);

            //infoHit.posA // n'a pas changé
            infoHit.posB = posB;
            prevFocusDir = infoHitBack.posB - posB;

            breakLoop = true;
            */
        }
    }

    /// <summary>
    /// here we have no hit, prepare next move
    /// </summary>
    /// <param name="infoHit"></param>
    /// <param name="prevFocusDir"></param>
    private void SetupNoHit(ref InfoHit infoHit, ref Vector3 prevFocusDir, int step)
    {
        //search for down !
        Vector3 downDir = -ExtQuaternion.CrossProduct(prevFocusDir, focusRightDir);
        //SaveLastNormal(downDir);

        Vector3 posC = infoHit.posB + downDir * GetDistDown();

        InfoHit infoHitDown = DoRayCastForward(infoHit.posB, posC, downDir, GetDistDown(), false);   //do a raycast

        //Debug.DrawRay(infoHit.posB, downDir, Color.blue);

        //here down hit something
        if (infoHitDown.hasHit)
        {
            hitCount = 0;

            //if we are realy in down, change info in SetupDown !
            if (!SetupDownHit(ref infoHitDown, ref downDir, ref infoHit, ref prevFocusDir))
            {
                //here down fail, continiue forward
                infoHit.posA = infoHit.posB;
                infoHit.posB = infoHit.posA + prevFocusDir * GetDistForward();

                currentDistChecked += GetDistForward();
            }
            
        }
        else
        {
            hitCount++;
            //Debug.Log("no hit in down ???");
            SetupGoingDown(ref infoHitDown, ref downDir, ref infoHit, ref prevFocusDir, step);

            //breakLoop = true;
            return;
        }
    }

    private bool ContinueLoop(int i, int currentStep, float currentDist, float maxDist)
    {
        if (breakLoop)
            return (false);

        if (i < stepsForward)
            return (true);

        if (currentDist + marginDistOk < maxDist)
        {
            //ici continue les steps, la distance n'est pas suffisante

            //mais on arrete quand meme, trop de test !
            if (i >= maxStepsWhenAdditionnalCalculs)
            {
                //Debug.Log("ok, no enought dist, but we did all additionnal steps");
                return (false);
            }


            return (true);
        }

        return (false);
    }

    private void DoLoop(int step, ref InfoHit infoHit, ref Vector3 focusDir, ref int i)
    {
        do
        {
            infoHit = DoRayCastForward(infoHit.posA, infoHit.posB, focusDir, GetDistForward(), true);   //do a raycast

            SaveLastPos(infoHit.posB);
            //SaveLastPos(infoHit.posB, infoHit.normal);

            //here calculate nextForward
            if (infoHit.hasHit)
            {
                hitCount = 0;
                SetupWhenHit(ref infoHit, ref focusDir);
            }
            else
            {
                hitCount++;
                SetupNoHit(ref infoHit, ref focusDir, i);
            }

            //Debug.Log("loop 1 done");

            i++;
        } while (ContinueLoop(i, stepsForward, currentDistChecked, distMax));
    }

    /// <summary>
    /// get dist forward, taking into account the maxDist
    /// </summary>
    private float GetDistForward()
    {
        float diffDist = Mathf.Abs(distMax - currentDistChecked);
        return (Mathf.Min(diffDist, distForwardRaycast));
    }
    private float GetDistDown()
    {
        float diffDist = Mathf.Abs(distMax - currentDistChecked);
        return (Mathf.Min(diffDist, distDownRaycast));
    }

    private void DoStepForward()
    {
        if (stepsForward == 0)
            return;

        hitCount = 0;
        breakLoop = false;

        //Vector3 focusDir = entityController.GetFocusedForwardDirPlayer();

        if (_entityAction.IsMoving())
        {
            focusForwardDir = _entityAction.GetRelativeDirection();
            focusRightDir = -_entityAction.GetRelativeDirectionRight();
        }


        if (!Application.isPlaying)
        {
            focusForwardDir = rb.transform.forward;
            focusRightDir = rb.transform.right;
        }

        //setup dist
        distMax = distForwardRaycast * stepsForward;
        currentDistChecked = 0f;


        Vector3 firstPosA = rb.transform.position;
        Vector3 firstPosB = firstPosA + focusForwardDir * GetDistForward();

        InfoHit infoHit = new InfoHit(firstPosA, firstPosB, false, ExtUtilityFunction.GetNullVector(), ExtUtilityFunction.GetNullVector());
        //save first pos & normal
        SaveLastPos(firstPosA);
        SaveLastNormal(-ExtQuaternion.CrossProduct(focusForwardDir, focusRightDir));
        //SaveLastNormal(infoHit.normal);

        int i = 0;
        DoLoop(stepsForward, ref infoHit, ref focusForwardDir, ref i);

        ExtDrawGuizmos.DebugWireSphere(GetLastPos(), Color.green, 0.5f);
        Debug.DrawRay(GetLastPos(), GetLastNormal() * 20f, Color.red);
        //Debug.Log("<color=blue>maxDist: " + distMax + ", dist effecctued: " + currentDistChecked + "(steps: " + i + ")</color>");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Application.isPlaying)
            DoStepForward();
    }

    private void FixedUpdate()
    {
        if (Application.isPlaying)
            DoStepForward();
    }
}
