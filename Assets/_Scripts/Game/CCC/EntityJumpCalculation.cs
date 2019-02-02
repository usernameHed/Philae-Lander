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
        TO_ROTATE,
        TO_ATTRACT,
    }

    public bool didWeHit;
    public Vector3 pointHit;
    public Vector3 normalHit;
    public Transform objHit;
    public Vector3 dirUltimatePlotPoint;
    public Vector3 ultimatePlotPoint;
    public JumpType jumpType;

    public void SetDirLast(Vector3[] plots)
    {
        Vector3 penultimate = plots[plots.Length - 2];
        Vector3 ultimate = plots[plots.Length - 1];

        ultimatePlotPoint = ultimate;
        dirUltimatePlotPoint = ultimate - penultimate;
    }

    public void Clear()
    {
        didWeHit = false;
        pointHit = Vector3.zero;
        normalHit = Vector3.zero;
        objHit = null;
        dirUltimatePlotPoint = Vector3.zero;
        ultimatePlotPoint = Vector3.zero;
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
    private float minDistAcceptedForGoingUp = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float timeFeforeCalculateAgainJump = 0.5f;
    [FoldoutGroup("GamePlay"), Range(0f, 0.8f), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float marginSideSlope = 0.1f;
    

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

    protected FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDownOnGround = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDowwnBeforeCalculateAgain = new FrequencyCoolDown();
    public bool IsReadyToTestCalculation() { return (coolDowwnBeforeCalculateAgain.IsStartedAndOver()); }
    private bool normalGravityTested = false;   //know if we are in the 0.5-0.8 sec between norma and attractor

    [SerializeField]
    private InfoJump infoJump = new InfoJump();
    private RaycastHit hitInfo;



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
            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 5f);
        }
        return (results);
    }

    /// <summary>
    /// do a sphereCast
    /// </summary>
    private bool DoSphereCast(Vector3 origin, Vector3 dir, float maxDist)
    {
        Debug.DrawRay(origin, dir * maxDist, Color.red, 5f);
        if (Physics.SphereCast(origin, 0.3f, dir, out hitInfo,
                                   maxDist, entityController.layerMask, QueryTriggerInteraction.Ignore))
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
        if (Vector3.SqrMagnitude(posFuture - hitPoint) > minDistAcceptedForGoingUp)
        {
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// do raycast along the Plots, and reutrn true if we hit
    /// </summary>
    /// <param name="infoPlot"></param>
    /// <returns></returns>
    public bool DoLoopRaycast(Vector3[] infoPlot, int depth = 2)
    {
        Vector3 prevPos = rb.transform.position;

        for (int i = 0; i <= depth; i++)
        {
            Vector3 dirRaycast;
            Vector3 lastPoint;
            bool hit;

            if (i == depth)
            {
                

                //TODO: ICI faire un raycast parallele au sol !!!
                //dirRaycast = infoJump.dirUltimatePlotPoint.normalized * distRaycastForNormalSwitch;
                dirRaycast = entityController.GetForwardDirPlayer();
                lastPoint = infoJump.ultimatePlotPoint;
                //lastPoint pas utile de le mettre ?
                hit = DoSphereCast(lastPoint, dirRaycast, distRaycastSIDE);
            }
            else
            {
                int indexPoint = ((infoPlot.Length / depth) * (i + 1)) - 1;
                Debug.Log("index: " + indexPoint + "(max: " + infoPlot.Length);
                lastPoint = infoPlot[indexPoint];

                dirRaycast = lastPoint - prevPos;
                hit = DoSphereCast(prevPos, dirRaycast.normalized, dirRaycast.magnitude);
            }
            
            if (hit)
            {

                return (true);
            }

            hit = DoSphereCast(prevPos, playerGravity.GetMainAndOnlyGravity(), distRaycastUP);
            if (hit && IsPlayerAtGoodDistanceForSwitch(prevPos, infoJump.pointHit))
            {
                infoJump.jumpType = InfoJump.JumpType.TO_UP;
                return (true);
            }

            prevPos = lastPoint;
        }
        return (false);
    }



    public void DetermineJumpType()
    {
        Vector3 normalJump = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalHit = infoJump.normalHit;

        float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
        if (dotImpact > 0 - marginSideSlope)
        {
            Debug.Log("we hit way !");
            infoJump.jumpType = InfoJump.JumpType.TO_SIDE;
            playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit);
        }
        else
        {
            Debug.Log("No way we climb That !, Obstacle to inclined");
            infoJump.jumpType = InfoJump.JumpType.BASE;
        }
    }

    public void JumpCalculation()
    {
        //reset jump first test timer
        coolDowwnBeforeCalculateAgain.StartCoolDown(timeFeforeCalculateAgainJump);
        normalGravityTested = false;
//dontApplyForceDownForThisRound = false;

        //first create 30 plot of the normal jump
        Vector3[] plots = Plots(rb, rb.transform.position, rb.velocity, 30, false, true);
        infoJump.Clear();
        infoJump.SetDirLast(plots);

        //here we know if we are in JUMP UP
        bool hit = DoLoopRaycast(plots);    //return true if we hit a wall in the first jump plot


        if (hit)
        {
            if (infoJump.jumpType != InfoJump.JumpType.TO_UP)
                DetermineJumpType();

            

        }
        else
        {
            Debug.Log("no hit...");
        }
        //Debug.Break();
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

        //chose if we add force or not
        Debug.Log("ultimate raycast");

        infoJump.Clear();
        //create plot WITH force down
        Vector3[] plots = Plots(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 16, false, true);

        infoJump.SetDirLast(plots);

        bool hit = DoLoopRaycast(plots, 1);    //return true if we hit a wall in the first jump plot

        if (!hit)
        {
            infoJump.Clear();
            //create plot WITOUT force down
            plots = Plots(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 30, false, false);
            infoJump.SetDirLast(plots);

            hit = DoLoopRaycast(plots, 1);
            if (hit)
            {
                Debug.Log("hit the long run ! desactive force down for this one !");
//dontApplyForceDownForThisRound = true;
            }
        }

        if (hit)
        {
            playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit);
        }
        
        //Debug.Break();
    }
}
