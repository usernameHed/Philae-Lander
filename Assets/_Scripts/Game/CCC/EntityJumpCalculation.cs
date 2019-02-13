﻿using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InfoJump
{
    public enum JumpType
    {
        BASE,
        TO_SIDE,
        TO_DOWN_NORMAL,
        TO_SPHERICAL,
    }

    public bool didWeHit;
    public Vector3 pointHit;
    public Vector3 normalHit;
    public Transform objHit;
    public Vector3 dirUltimatePlotPoint;
    public Vector3 initialPosBeforePlots;
    public Vector3 ultimatePlotPoint;
    public Vector3 lastVelocity;
    public bool lastRaycastHit;
    public JumpType jumpType;

    public void SetDirLast(Vector3[] plots, Vector3 initialPos)
    {
        Vector3 penultimate = plots[plots.Length - 2];
        Vector3 ultimate = plots[plots.Length - 1];

        ultimatePlotPoint = ultimate;
        dirUltimatePlotPoint = ultimate - penultimate;

        initialPosBeforePlots = initialPos;

        lastVelocity = (ultimate - penultimate) / Time.deltaTime;
    }

    public void Clear()
    {
        didWeHit = false;
        pointHit = Vector3.zero;
        normalHit = Vector3.zero;
        objHit = null;
        dirUltimatePlotPoint = Vector3.zero;
        ultimatePlotPoint = Vector3.zero;
        initialPosBeforePlots = Vector3.zero;
        lastVelocity = Vector3.zero;
        lastRaycastHit = false;
        jumpType = JumpType.BASE;
    }
}

public class EntityJumpCalculation : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float radiusSphereCast = 0.4f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastSIDE = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastUP = 3f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastDOWN = 3f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastHOLE = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private string[] layerNoSideJump = new string[] { "Walkable/NoSide" };
    //[FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    //private float minDistAcceptedForGoingUp = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float timeFeforeCalculateAgainJump = 0.5f;
    [FoldoutGroup("GamePlay"), Range(0f, 0.8f), SerializeField, Tooltip("margin slope for SIDE jump")]
    private float marginSideSlope = 0.5f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginNotMovingTestJump = 0.5f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginJumpEndDot = 0.86f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginJumpEndDotRight = 0.5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Transform playerLocalyRotate;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction;
    
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAttractor entityAttractor;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch;

    [FoldoutGroup("Debug"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1.4f;

    [SerializeField]
    private InfoJump infoJump = new InfoJump();
    private RaycastHit hitInfo;

    /// <summary>
    /// calculate trajectory of entity
    ///rigidbody: rb of the object
    ///pos: position from where to start the plot Trajectory
    ///velocity: current velocity of the rigidbody
    ///steps: numbers of steps
    ///applyForceUp: do we apply additionnal gravity when going upward ?
    ///applyForceDown: do we apply additionnal gravity when going down ?
    /// </summary>
    public Vector3[] Plots(Rigidbody rigidbody, Vector3 pos, Vector3 velocity, int steps, bool applyForceUp, bool applyForceDown)
    {
        Vector3[] results = new Vector3[steps];

        float timestep = Time.fixedDeltaTime / magicTrajectoryCorrection;   //magicCorection = 1

        float drag = 1f - timestep * rigidbody.drag;    //take into account the rb drag
        Vector3 moveStep = velocity * timestep;

        int i = -1;
        while (++i < steps)
        {
            //get the gravity direction, depending on the position
            Vector3 gravityOrientation = playerGravity.CalculateGravity(pos);
            //Get the vector acceleration (dir + magnitude)
            Vector3 gravityAccel = playerGravity.FindAirGravity(pos, moveStep, gravityOrientation, applyForceUp, applyForceDown) * timestep;// * timestep;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;// * timestep;

            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 5f);
        }
        return (results);
    }

    /// <summary>
    /// called by entityJump
    /// </summary>
    public void OnGrounded()
    {
        infoJump.Clear();
    }

    /// <summary>
    /// do a sphereCast
    /// </summary>
    private bool DoSphereCast(Vector3 origin, Vector3 dir, float maxDist, int layers)
    {
        Debug.DrawRay(origin, dir * maxDist, Color.red, 5f);
        if (Physics.SphereCast(origin, 0.3f, dir, out hitInfo,
                                   maxDist, layers, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("find something raycast !");
            infoJump.didWeHit = true;
            infoJump.normalHit = hitInfo.normal;
            infoJump.objHit = hitInfo.transform;
            infoJump.pointHit = hitInfo.point;
            Debug.DrawRay(hitInfo.point, infoJump.normalHit, Color.magenta, 5f);
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// do raycast along the Plot, and reutrn true if we hit
    /// </summary>
    /// <param name="infoPlot"></param>
    /// <returns></returns>
    public bool DoLoopRaycastStartJump(Vector3[] infoPlot, int depth, ref bool longEndRaycastHit)
    {
        Vector3 prevPos = infoJump.initialPosBeforePlots;// rb.transform.position;
        longEndRaycastHit = false;
        for (int i = 0; i <= depth; i++)
        {
            Vector3 dirRaycast;
            Vector3 lastPoint;
            bool hit;

            if (i == depth)
            {
                

                //TODO: ICI faire un raycast parallele au sol !!!
                //dirRaycast = infoJump.dirUltimatePlotPoint.normalized * distRaycastForNormalSwitch;
                dirRaycast = entityController.GetFocusedForwardDirPlayer();
                lastPoint = infoJump.ultimatePlotPoint;
                //lastPoint pas utile de le mettre ?
                hit = DoSphereCast(lastPoint, dirRaycast, distRaycastSIDE * entityAction.GetMagnitudeInput(), entityController.layerMask);
                if (hit)
                {
                    longEndRaycastHit = true;
                }                    
            }
            else
            {
                int indexPoint = ((infoPlot.Length / depth) * (i + 1)) - 1;
                Debug.Log("index: " + indexPoint + "(max: " + infoPlot.Length);
                lastPoint = infoPlot[indexPoint];

                dirRaycast = lastPoint - prevPos;
                hit = DoSphereCast(prevPos, dirRaycast.normalized, dirRaycast.magnitude, entityController.layerMask);
            }
            
            if (hit)
            {

                return (true);
            }

            prevPos = lastPoint;
        }
        return (false);
    }
    public bool DoLoopRaycastEndJump(Vector3[] infoPlot, int depth = 2)
    {
        Vector3 prevPos = infoJump.initialPosBeforePlots;//rb.transform.position;

        for (int i = 0; i <= depth; i++)
        {
            Vector3 dirRaycast;
            Vector3 lastPoint = Vector3.zero;
            bool hit = false;

            if (i == depth)
            {
                
                hit = DoSphereCast(prevPos, infoJump.dirUltimatePlotPoint, distRaycastDOWN, entityController.layerMask);
                if (hit)
                    infoJump.lastRaycastHit = true;
            }
            else
            {
                int indexPoint = ((infoPlot.Length / depth) * (i + 1)) - 1;
                Debug.Log("index: " + indexPoint + "(max: " + infoPlot.Length);
                lastPoint = infoPlot[indexPoint];

                dirRaycast = lastPoint - prevPos;
                hit = DoSphereCast(prevPos, dirRaycast.normalized, dirRaycast.magnitude, entityController.layerMask);
            }

            if (hit)
            {

                return (true);
            }

            prevPos = lastPoint;
        }
        return (false);
    }

    /// <summary>
    /// if we are on a OBJECT oriented gravity, choose
    /// if: gravity is dir focused
    /// if: gravity is based on the normal of the object hit
    /// </summary>
    /// <returns></returns>
    public bool CanApplyNormalizedObjectGravity()
    {
        if (infoJump.jumpType == InfoJump.JumpType.TO_SIDE
            || infoJump.jumpType == InfoJump.JumpType.TO_DOWN_NORMAL)
            return (true);
        return (false);
    }

    public bool CanApplyForceDown()
    {
        if (infoJump.jumpType == InfoJump.JumpType.TO_SIDE)
            return (true);

        //TO_DOWN_NORMAL true
        //BASE true
        //TO UP pas utile mais vrai

        return (true);
    }
    public bool CanApplyForceUp()
    {
        if (infoJump.jumpType == InfoJump.JumpType.TO_SIDE)
            return (false);

        //TO_DOWN_NORMAL true
        //BASE true
        //TO UP pas utile mais vrai

        return (true);
    }

    /// <summary>
    /// return the main gravity in all case
    /// Except JUMP_SIDE: here we have a FAKE gravity toward the side, (for rotation
    /// But the actual gravity is down !
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSpecialAirGravity()
    {
        //here, we apply gravity DOWN (even if the actual gravity is to the side)
        if (infoJump.jumpType == InfoJump.JumpType.TO_SIDE)
            return (entityController.GetFocusedForwardDirPlayer());


        return (playerGravity.GetMainAndOnlyGravity());
    }

    private bool IsSideJumpLayerAccepted(int layer)
    {
        int isInForbiddenLayer = ExtList.ContainSubStringInArray(layerNoSideJump, LayerMask.LayerToName(layer));
        if (isInForbiddenLayer == -1)
            return (true);
        return (false);
    }

    private bool CanDoSideJump(float dotImpact)
    {
        if (!(dotImpact > 0 - marginSideSlope))
        {
            Debug.Log("No: not nice slope: " + dotImpact);
            return (false);
        }
        if (entityAction.NotMoving(marginNotMovingTestJump))
        {
            Debug.Log("No: not moving !");
            return (false);
        }
        if (!IsSideJumpLayerAccepted(infoJump.objHit.gameObject.layer))
        {
            Debug.Log("not good layer for jumping !");
            return (false);
        }
        if (!entityGravityAttractorSwitch.CanDoSideJump(infoJump.objHit))
        {
            Debug.Log("No: GA say not to side Jump !");
            return (false);
        }
        return (true);
    }

    public bool DetermineSideJump()
    {
        Vector3 normalJump = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalHit = infoJump.normalHit;

        float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
        //int isInForbiddenLayer = ExtList.ContainSubStringInArray(layerNoSideJump, LayerMask.LayerToName(infoJump.objHit.gameObject.layer));
        if (CanDoSideJump(dotImpact))
        {
            Debug.Log("SIDE JUMP DESICED");
            infoJump.jumpType = InfoJump.JumpType.TO_SIDE;

            
            //Debug.Break();
            return (true);
        }
        else
        {
            Debug.Log("NO SIDE JUMP: Obstacle to inclined, or no input forward, or NoSide layer, OR GA mode & obhHit is still DA !");
            return (false);
        }
    }

    /// <summary>
    /// do calculation based on velocity we want to jump
    /// </summary>
    /// <param name="orientedSetVelocity"></param>
    public void JumpCalculation(Vector3 orientedSetVelocity)
    {
        infoJump.jumpType = InfoJump.JumpType.BASE; //set basic jump

        //first create 30 plot of the normal jump
        Vector3[] plots = Plots(rb, rb.transform.position, orientedSetVelocity, 30, false, true);
        infoJump.Clear();
        infoJump.SetDirLast(plots, rb.transform.position);

        //here we know if we are in JUMP UP
        bool longEndRaycastHit = true;
        bool hit = DoLoopRaycastStartJump(plots, 2, ref longEndRaycastHit);    //return true if we hit a wall in the first jump plot

        if (hit)
        {
            Debug.Log("Start Jump calculation detect something... Test for sideJump");
            bool sideJump = DetermineSideJump();

            //if no sideJump, do end calculation
            if (!sideJump)
            {
                if (!longEndRaycastHit)
                {
                    Debug.Log("WE CANT DO SIDEJUMP for some reason, AND WE HIT IN THE TRAJECTORY, JUST DO BASE JUMP");
                    infoJump.jumpType = InfoJump.JumpType.BASE;
                    FinishCalculation();
                    return;
                }

                EndJumpCalculation(plots);
            }
            else
            {
                FinishCalculation();
            }
        }
        else
        {
            Debug.Log("Start Jump calculation detect nothing... Do endJumpCalculation");
            EndJumpCalculation(plots);
        }
    }

    private void TrySideJumpElseBase()
    {
        bool sideJump = DetermineSideJump();
        if (sideJump && !infoJump.lastRaycastHit)
        {
            infoJump.jumpType = InfoJump.JumpType.TO_SIDE;
            Debug.Log("side jump anyway !");
        }
        else
        {
            if (infoJump.lastRaycastHit)
            {
                Debug.LogWarning("Sorry, but last raycast hit is not precise enought !");
            }

            infoJump.jumpType = InfoJump.JumpType.BASE;
            Debug.Log("we realy can't do sideJump sorry..");
        }
    }

    /// <summary>
    /// return result based on impact normal
    /// </summary>
    /// 1: normal equal, or realy close, we can do TO_DOWN_NORMAL
    /// 2: normal negative, do nothing !
    /// 3: right angle, we can do TO_DOWN_NORMAL !
    /// <returns></returns>
    public void DetermineEndNormalsHit(Vector3[] startPlots, Vector3[] endPlots)
    {
        Vector3 normalJump = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalHit = infoJump.normalHit;
        Vector3 rightPlayer = entityController.GetFocusedRightDirPlayer();

        float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);

        if (dotImpact > marginJumpEndDot)
        {
            Debug.Log("dotImpact close: " + dotImpact);
            infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
            return;
        }
        if (dotImpact < 0)
        {
            Debug.Log("omg dotImpact negatif ! try side jump ? !");
            TrySideJumpElseBase();
            //Debug.Break();
            return;
        }
            

        Debug.DrawRay(infoJump.pointHit, normalJump, Color.blue, 5f);
        Debug.DrawRay(infoJump.pointHit, normalHit, Color.gray, 5f);
        Debug.DrawRay(infoJump.pointHit, rightPlayer, Color.green, 5f);
        float dotLeft = 0f;
        float dotRight = 0f;
        
        int rightToImpact = ExtQuaternion.IsRightOrLeft(normalJump, rightPlayer, normalHit, infoJump.pointHit, ref dotLeft, ref dotRight);

        if (rightToImpact == 1)
        {
            Debug.Log("RIGHT SIDE: dot: " + dotRight + " (max: " + marginJumpEndDotRight + ")");
            if (dotRight > marginJumpEndDotRight)
            {
                Debug.Log("mmm, too 90°... we could do SIDE_JUMP, but it's too far away, I prefer do BASE jump");

                TrySideJumpElseBase();
                return;
            }

            Debug.Log("Ok right side for TO_DOWN_NORMAL");
            infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
            return;
        }
        else if (rightToImpact == -1)
        {
            Debug.Log("LEFT SIDE !");
            DetermineEndSphericalJump(startPlots, endPlots, true);
            return;
        }

        Debug.Log("ok, just normal jump then...");
    }

    /*
    private bool IsNormalOkHere(Vector3 refJumpNormal, Vector3 currentNormal)
    {
        Debug.Log("");
    }
    */

    private bool IsThereGap(Vector3[] startPlots, Vector3[] endPlots)
    {
        Vector3 prevPos = startPlots[0];
        Vector3 gravityOrientation = -playerGravity.GetMainAndOnlyGravity();

        Vector3 refJumpNormal = -gravityOrientation;

        int depth = 4;
        bool isOneBadNormal = false;

        //DO Test HOLE
        for (int i = 0; i < depth - 1; i++)
        {
            Vector3 dirRaycast;
            Vector3 lastPoint = Vector3.zero;
            bool hit = false;

            int numberPoints = startPlots.Length + endPlots.Length;
            int indexPoint = ((numberPoints / depth) * (i + 1)) - 1;


            Debug.Log("index: " + indexPoint + "(max: " + numberPoints + ")");
            if (indexPoint >= numberPoints)
                break;

            lastPoint = (indexPoint < startPlots.Length) ? startPlots[indexPoint] : endPlots[indexPoint - startPlots.Length];

            //dirRaycast = lastPoint - prevPos;
            hit = DoSphereCast(lastPoint, gravityOrientation, distRaycastHOLE, entityController.layerMask);

            /*if (!hit || !IsNormalOkHere(refJumpNormal, infoJump.normalHit))   //or normal hit realy bad !
            {
                Debug.Log("here a gap, or a wrong normal !");
                ExtDrawGuizmos.DebugWireSphere(infoJump.pointHit, Color.red, 0.1f, 5f);
                isOneBadNormal = true;
            }
            else
            {
                if (isOneBadNormal)
                {
                    Debug.Log("there where a bad normal previously, and now we are good, there is a gap then !");
                    return (true);
                }
            }*/


            prevPos = lastPoint;
        }

        return (false);
    }

    /// <summary>
    /// first test a gap.
    /// </summary>
    public void DetermineEndSphericalJump(Vector3[] startPlots, Vector3[] endPlots, bool hasHit)
    {
        Debug.Log("here do the super test: is we are on planet, chose spherical, else, return");
        //infoJump.jumpType = InfoJump.JumpType.TO_SPHERICAL;
        //bool isThereGap = IsThereGap(startPlots, endPlots);
        if (hasHit)
        {
            infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
        }
        else
        {
            infoJump.jumpType = InfoJump.JumpType.BASE;
        }


        //Debug.B
    }

    private void EndJumpCalculation(Vector3[] startPlots)
    {
        Vector3 futurePosRb = infoJump.ultimatePlotPoint;
        Vector3 lastVelocityRb = infoJump.lastVelocity;

        infoJump.Clear();

        Vector3[] endPlots = Plots(rb, futurePosRb, lastVelocityRb, 30, false, true);
        infoJump.SetDirLast(endPlots, futurePosRb);

        //here we know if we are in JUMP UP
        bool hit = DoLoopRaycastEndJump(endPlots, 4);    //return true if we hit a wall in the first jump plot

        if (hit)
        {
            DetermineEndNormalsHit(startPlots, endPlots);
        }
        else
        {
            DetermineEndSphericalJump(startPlots, endPlots, false);
        }
        FinishCalculation();
    }

    /// <summary>
    /// we finaly finish calculation and choose our jump type, now apply this changement !
    /// </summary>
    private void FinishCalculation()
    {
        switch (infoJump.jumpType)
        {
            case (InfoJump.JumpType.TO_SIDE):
                if (entityGravityAttractorSwitch.IsInGravityAttractorMode()/* && !entitySphereAirMove.IsAirAttractorLayer(infoJump.objHit.gameObject.layer)*/)
                {
                    Debug.Log("mmm wut ? we decide to leave attractorGravity Mode for side jump !");

                    //here the layer we want to go is NOT in a airAttractor layer
                    if (!entityGravityAttractorSwitch.IsAirAttractorLayer(infoJump.objHit.gameObject.layer))
                    {
                        entityGravityAttractorSwitch.UnselectOldGA();
                    }
                    else
                    {
                        Debug.Log("it's the same, or an other ? do nothing ?");
                    }

                    //SEULEMENT SI INFO JUMP HIT LAYER n'est pas un sideJUMP !!!
                    
                }

                Debug.Log("WE DESIDED: TO_SIDE");
                Vector3 normalGravitySide = -entityController.GetFocusedForwardDirPlayer();
                playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, normalGravitySide);
                Debug.DrawRay(infoJump.pointHit, normalGravitySide, Color.black, 5f);
                break;
            case (InfoJump.JumpType.TO_DOWN_NORMAL):
                Debug.Log("WE DESIDED: TO_DOWN_NORMAL");
                Vector3 normalGravity = playerGravity.GetMainAndOnlyGravity();
                playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, normalGravity);
                Debug.DrawRay(infoJump.pointHit, normalGravity, Color.black, 5f);
                break;
            case (InfoJump.JumpType.TO_SPHERICAL):
                Debug.Log("WE DESIDED: TO_SPHERICAL");
                break;
            case (InfoJump.JumpType.BASE):
                Debug.Log("WE DESIDED: BASE");
                break;
        }
        //Debug.Break();
    }
}
