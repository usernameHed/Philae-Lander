using Sirenix.OdinInspector;
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
        TO_UP,
        TO_SIDE,
        TO_DOWN_NORMAL,
    }

    public bool didWeHit;
    public Vector3 pointHit;
    public Vector3 normalHit;
    public Transform objHit;
    public Vector3 dirUltimatePlotPoint;
    public Vector3 initialPosBeforePlots;
    public Vector3 ultimatePlotPoint;
    public Vector3 lastVelocity;
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
    private string[] layerSwitch = new string[] { "Walkable/Floor" };
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

    [FoldoutGroup("Debug"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1.4f;
    private bool normalGravityTested = false;   //know if we are in the 0.5-0.8 sec between norma and attractor

    [SerializeField]
    private InfoJump infoJump = new InfoJump();
    private RaycastHit hitInfo;

    public void ResetCalculation()
    {
        normalGravityTested = false;
    }

    /// <summary>
    /// calculate trajectory of entity
    /// </summary>
    public Vector3[] Plots(Rigidbody rigidbody, Vector3 pos, Vector3 velocity, int steps, bool applyForceUp, bool applyForceDown)
    {
        Vector3[] results = new Vector3[steps];

        float timestep = Time.fixedDeltaTime / magicTrajectoryCorrection;
        //gravityAttractorLerp = 1f;

        float drag = 1f - timestep * rigidbody.drag;
        Vector3 moveStep = velocity * timestep;

        int i = -1;
        while (++i < steps)
        {
            Vector3 gravityOrientation = playerGravity.CalculateGravity(pos);
            Vector3 gravityAccel = playerGravity.FindAirGravity(pos, moveStep, gravityOrientation, applyForceUp, applyForceDown) * timestep;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;

            //infoJump.lastVelocity = moveStep;

            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 5f);
        }
        return (results);
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
            Debug.Log("find something ! keep going with normal gravity");
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
    /// return true if we are far enought from the hit point !
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerAtGoodDistanceForSwitch(Vector3 posFuture, Vector3 hitPoint)
    {
        Debug.Log("dont manage dist ?");
        return (true);
        /*
        if (Vector3.SqrMagnitude(posFuture - hitPoint) > minDistAcceptedForGoingUp)
        {
            return (true);
        }
        return (false);
        */
    }

    /// <summary>
    /// do raycast along the Plot, and reutrn true if we hit
    /// </summary>
    /// <param name="infoPlot"></param>
    /// <returns></returns>
    public bool DoLoopRaycastStartJump(Vector3[] infoPlot, int depth = 2)
    {
        Vector3 prevPos = infoJump.initialPosBeforePlots;// rb.transform.position;

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
                hit = DoSphereCast(lastPoint, dirRaycast, distRaycastSIDE, entityController.layerMask);
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
            return (false);

        //TO_DOWN_NORMAL true
        //BASE true
        //TO UP pas utile mais vrai

        return (true);
    }

    public void DetermineJumpType()
    {
        Vector3 normalJump = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalHit = infoJump.normalHit;

        float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
        if (dotImpact > 0 - marginSideSlope && !entityAction.NotMoving(marginNotMovingTestJump))
        {
            Debug.Log("we hit way !");
            infoJump.jumpType = InfoJump.JumpType.TO_SIDE;
            playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, infoJump.normalHit);
        }
        else
        {
            Debug.Log("No way we climb That !, Obstacle to inclined (or no input forward)");
            infoJump.jumpType = InfoJump.JumpType.BASE;
        }
    }

    public void JumpCalculation()
    {
        //reset jump first test timer
        normalGravityTested = false;
//dontApplyForceDownForThisRound = false;

        //first create 30 plot of the normal jump
        Vector3[] plots = Plots(rb, rb.transform.position, rb.velocity, 30, false, true);
        infoJump.Clear();
        infoJump.SetDirLast(plots, rb.transform.position);

        //here we know if we are in JUMP UP
        bool hit = DoLoopRaycastStartJump(plots);    //return true if we hit a wall in the first jump plot




        if (hit)
        {
            Debug.Log("hit something...");
            DetermineJumpType();

            if (infoJump.jumpType == InfoJump.JumpType.BASE)
            {
                EndJumpCalculation(plots);
            }

        }
        else
        {
            Debug.Log("no hit...");
            EndJumpCalculation(plots);
        }
        
    }

    public bool IsNormalGravityJump()
    {
        Vector3 normalJump = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalHit = infoJump.normalHit;
        Vector3 rightPlayer = entityController.GetFocusedRightDirPlayer();

        float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);

        if (dotImpact > marginJumpEndDot)
        {
            Debug.Log("dotImpact close: " + dotImpact);
            return (true);
        }
            

        Debug.DrawRay(infoJump.pointHit, normalJump, Color.blue, 5f);
        Debug.DrawRay(infoJump.pointHit, normalHit, Color.gray, 5f);
        Debug.DrawRay(infoJump.pointHit, rightPlayer, Color.green, 5f);
        int rightToImpact = ExtQuaternion.IsRightOrLeft(normalJump, rightPlayer, normalHit, infoJump.pointHit);

        if (rightToImpact == 1)
        {
            Debug.Log("not close to normalJump, BUT right normal !");
            return (true);
        }
        
        return (false);
    }

    private void RaycastGapTest(Vector3[] startJumpPlots, Vector3[] endJumpPlots)
    {
        Debug.Log("Do a Raycast gap test ! if there is no gap, then try planet oriented !");
    }

    private void EndJumpCalculation(Vector3[] lastPlots)
    {
        Vector3 futurePosRb = infoJump.ultimatePlotPoint;
        Vector3 lastVelocityRb = infoJump.lastVelocity;

        infoJump.Clear();

        Vector3[] plots = Plots(rb, futurePosRb, lastVelocityRb, 30, false, true);
        infoJump.SetDirLast(plots, futurePosRb);

        //here we know if we are in JUMP UP
        bool hit = DoLoopRaycastEndJump(plots, 4);    //return true if we hit a wall in the first jump plot

        if (hit)
        {
            if (!IsNormalGravityJump())
            {
                RaycastGapTest(lastPlots, plots);
                Debug.Break();
            }
            else
            {
                Debug.Log("normal jump !");
                infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
                Vector3 normalGravity = playerGravity.CalculateGravity(rb.transform.position);
                //playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, infoJump.normalHit);
                playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, normalGravity);
                Debug.Break();
            }
        }


        
    }

    /// <summary>
    /// do an ultime test of PLOT / raycast, if we find something: switch to Object oriented
    /// else, we will pass soon on Attractor...
    /// </summary>
    public void UltimaTestBeforeAttractor()
    {
        if (normalGravityTested)
            return;
        normalGravityTested = true;

        Vector3 ultimate = infoJump.ultimatePlotPoint;
        Vector3 dirUltimate = infoJump.dirUltimatePlotPoint;

        if (infoJump.jumpType == InfoJump.JumpType.TO_UP)
        {
            //refaire des test ici pour le jump...
            playerGravity.ChangeMainAttractObject(infoJump.objHit, infoJump.pointHit, infoJump.normalHit);
            return;
        }

        //chose if we add force or not
        Debug.Log("ultimate raycast");

        infoJump.Clear();
        //create plot WITH force down
        Vector3[] plots = Plots(rb, rb.transform.position, rb.velocity, 16, false, true);

        infoJump.SetDirLast(plots, rb.transform.position);

        bool hit = DoLoopRaycastStartJump(plots, 1);    //return true if we hit a wall in the first jump plot

        if (!hit)
        {
            infoJump.Clear();
            //create plot WITOUT force down
            plots = Plots(rb, rb.transform.position, rb.velocity, 30, false, false);
            infoJump.SetDirLast(plots, rb.transform.position);

            hit = DoLoopRaycastStartJump(plots, 1);
            if (hit)
            {
                Debug.Log("hit the long run ! desactive force down for this one !");
//dontApplyForceDownForThisRound = true;
            }
        }

        if (hit)
        {
            playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, infoJump.normalHit);
        }
        
        //Debug.Break();
    }
}
