
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

[Serializable]
public struct InfoJump
{
    public bool didWeHit;
    public Vector3 pointHit;
    public Vector3 normalHit;
    public Transform objHit;
    public Vector3 dirUltimatePlotPoint;
    public Vector3 initialPosBeforePlots;
    public Vector3 ultimatePlotPoint;
    public Vector3 lastVelocity;
    public bool lastRaycastHit;

    public bool penultimateRaycastHit;

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
        penultimateRaycastHit = false;
        lastRaycastHit = false;
    }
}

public class EntityJumpCalculation : MonoBehaviour
{
    [SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastDOWN = 3f;

    [Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginJumpEndDot = 0.5f;

    [Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float minJumpSpeedForNormalGravity = 0.7f;

    [SerializeField, Tooltip("ref script")]
    private EntityController entityController = default;
    [Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb = default;

    [SerializeField, Tooltip("ref script")]
    private EntityGravity entityGravity = default;


    [SerializeField, Tooltip("ref script")]
    private EntityJump entityJump = default;
    [SerializeField, Tooltip("ref script")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch = default;

    [Tooltip("gravité du saut"), SerializeField]
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
            Vector3 gravityOrientation = entityGravity.CalculateGravity(pos);
            //Get the vector acceleration (dir + magnitude)
            Vector3 gravityAccel = entityGravity.FindAirGravity(pos, moveStep,
                gravityOrientation, applyForceUp, applyForceDown) * timestep;// * timestep;
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
            //Debug.Log("find something raycast !");
            infoJump.didWeHit = true;
            infoJump.normalHit = hitInfo.normal;
            infoJump.objHit = hitInfo.transform;
            infoJump.pointHit = hitInfo.point;
            Debug.DrawRay(hitInfo.point, infoJump.normalHit, Color.magenta, 5f);
            return (true);
        }
        return (false);
    }

    /*
    /// <summary>
    /// return result based on impact normal
    /// </summary>
    /// 1: normal equal, or realy close, we can do TO_DOWN_NORMAL
    /// 2: normal negative, do nothing !
    /// 3: right angle, we can do TO_DOWN_NORMAL !
    /// <returns></returns>
    public bool IsNormalOkToLand()
    {
        Vector3 normalJump = entityGravity.GetMainAndOnlyGravity();
        Vector3 normalHit = infoJump.normalHit;
        Vector3 rightPlayer = entityController.GetFocusedRightDirPlayer();

        float dotImpact = Vector3.Dot(normalJump, normalHit);

        if (dotImpact > marginJumpEndDot)
        {
            Debug.Log("dotImpact close: " + dotImpact);
            //infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
            return (true);
        }
        if (dotImpact < 0)
        {
            Debug.Log("omg dotImpact negatif ! try side jump ? !");
            return (false);
        }
        return (false);
    }
    */


    public bool DoLoopRaycastUltime(Vector3[] infoPlot, int depth = 2)
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
                //Debug.Log("index: " + indexPoint + "(max: " + infoPlot.Length);
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

    public Vector3 GetHitPoint()
    {
        if (infoJump.didWeHit)
            return (infoJump.pointHit);
        return (Vector3.zero);
    }

    /// <summary>
    /// called just when we fall down !
    /// </summary>
    public bool UltimeTestBeforeAttractor()
    {
        infoJump.Clear();

        Vector3[] ultimePlots = Plots(rb, rb.position, rb.velocity, 30, false, true);

        infoJump.SetDirLast(ultimePlots, rb.position);

        //here we know if we are in JUMP UP
        bool hit = DoLoopRaycastUltime(ultimePlots, 4);    //return true if we hit a wall in the first jump plot

        //if we hit... have normal gravity
        if (hit)
        {
            string layerNameHit = LayerMask.LayerToName(infoJump.objHit.gameObject.layer);


            //if this is stick layer, keep sticking !
            if (entityController.IsStickPlatform(layerNameHit))
                return (true);


            //else, if this is a Dont platform, and normal is NOT ok, return false
            if (entityController.IsForbidenLayerSwitch(layerNameHit)
                && !entityGravityAttractorSwitch.IsNormalIsOkWithCurrentGravity(infoJump.normalHit, entityGravityAttractorSwitch.GetGAGravityAtThisPoint(infoJump.pointHit)))
            {
                return (false);
            }
            //else, if this is a mario galaxy platform, whatever normal, if forwardSpeed is FAST; return false
            if (entityController.IsMarioGalaxyPlatform(layerNameHit) && entityJump.GetLastJumpForwardVelocity() > minJumpSpeedForNormalGravity)
            {
                return (false);
            }


            return (true);
        }
            
        return (false);
    }
}
