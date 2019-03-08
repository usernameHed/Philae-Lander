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
    public bool lastRaycastHit;
    public JumpType jumpType;
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
        jumpType = JumpType.BASE;
    }
}

public class EntityJumpCalculation : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastSIDE = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastDOWN = 3f;

    [FoldoutGroup("GamePlay"), Range(0f, 0.8f), SerializeField, Tooltip("margin slope for SIDE jump")]
    private float marginSideSlope = 0.5f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginNotMovingTestJump = 0.5f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginJumpEndDot = 0.86f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("margin from where we considere we dont move for jump calculation")]
    private float marginJumpEndDotRight = 0.5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityGravity playerGravity = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAttractor entityAttractor = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private GroundCheck groundCheck = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityJump entityJump = null;

    [FoldoutGroup("Debug"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1.4f;

    [SerializeField]
    private InfoJump infoJump = new InfoJump();
    private RaycastHit hitInfo;

    public InfoJump.JumpType GetJumpType()
    {
        return (infoJump.jumpType);
    }

    public bool CanDoAirMove()
    {
        return (true);
    }
    public bool CanDoBumpUp()
    {
        return (true);
    }

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
            Vector3 gravityAccel = playerGravity.FindAirGravity(pos, moveStep,
                gravityOrientation, applyForceUp, applyForceDown, false) * timestep;// * timestep;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;// * timestep;

            results[i] = pos;
            //ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 5f);
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
        //Debug.DrawRay(origin, dir * maxDist, Color.red, 5f);
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
                float realInputForward = entityJump.GetLastJumpForwardVelocity();
                hit = DoSphereCast(lastPoint, dirRaycast, distRaycastSIDE * realInputForward, entityController.layerMask);
                if (hit)
                {
                    //Debug.LogWarning("c'est ici qu'on dit true !");
                    longEndRaycastHit = true;
                }                    
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

                if (i == depth - 1 && hit)
                    infoJump.penultimateRaycastHit = true;
            }

            if (hit)
            {

                return (true);
            }

            prevPos = lastPoint;
        }
        return (false);
    }

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

                //hit = DoSphereCast(prevPos, infoJump.dirUltimatePlotPoint, distRaycastDOWN, entityController.layerMask);
                //if (hit)
                //    infoJump.lastRaycastHit = true;
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
        if (infoJump.jumpType == InfoJump.JumpType.TO_DOWN_NORMAL)
            return (true);
        return (false);
    }

    public bool CanApplyForceDown()
    {
        return (true);
    }
    public bool CanApplyForceUp()
    {
        return (true);
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
            if (!longEndRaycastHit)
            {
                Debug.Log("WE CANT DO SIDEJUMP for some reason, AND WE HIT IN THE TRAJECTORY, JUST DO BASE JUMP");
                infoJump.jumpType = InfoJump.JumpType.BASE;
                FinishCalculation();
                return;
            }
        }
        Debug.Log("Start Jump calculation detect nothing... Do endJumpCalculation");
        EndJumpCalculation(plots);
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
            infoJump.jumpType = InfoJump.JumpType.BASE;
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
                infoJump.jumpType = InfoJump.JumpType.BASE;
                return;
            }

            Debug.Log("Ok right side for TO_DOWN_NORMAL");
            infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
            return;
        }
        else if (rightToImpact == -1)
        {
            Debug.Log("LEFT SIDE !");
            infoJump.jumpType = InfoJump.JumpType.TO_DOWN_NORMAL;
            return;
        }

        Debug.Log("ok, just normal jump then...");
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
            infoJump.jumpType = InfoJump.JumpType.BASE;
        }
        FinishCalculation();
    }

    /// <summary>
    /// called just when we fall down !
    /// </summary>
    public void UltimeTestBeforeAttractor()
    {
        if (infoJump.jumpType == InfoJump.JumpType.BASE
            && playerGravity.GetOrientationPhysics() == EntityGravity.OrientationPhysics.NORMALS)
        {
            infoJump.Clear();
            Vector3[] ultimePlots = Plots(rb, rb.position, rb.velocity, 30, false, true);

            infoJump.SetDirLast(ultimePlots, rb.position);

            //here we know if we are in JUMP UP
            bool hit = DoLoopRaycastUltime(ultimePlots, 4);    //return true if we hit a wall in the first jump plot

            //isOkToCreateAttractor = false;
            if (hit)
            {
                entityAttractor.RetryCoolDown();
                Debug.Log("OK continiue BASE after a second test !");
            }
            return;
        }
    }

    /// <summary>
    /// we finaly finish calculation and choose our jump type, now apply this changement !
    /// </summary>
    private void FinishCalculation()
    {
        switch (infoJump.jumpType)
        {
            case (InfoJump.JumpType.TO_DOWN_NORMAL):
                if (entityGravityAttractorSwitch.IsAirAttractorLayer(infoJump.objHit.gameObject.layer)
                    && entityGravityAttractorSwitch.IsAirAttractorLayer(groundCheck.GetLastPlatform().gameObject.layer))
                {
                    Debug.Log("WE DESIDED: To_DOWN but both previous and next platform is GA, do BASE !");
                    infoJump.jumpType = InfoJump.JumpType.BASE;
                    break;
                }
                Debug.Log("WE DESIDED: TO_DOWN_NORMAL");
                Vector3 normalGravity = playerGravity.GetMainAndOnlyGravity();
                playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit, normalGravity);
                Debug.DrawRay(infoJump.pointHit, normalGravity, Color.black, 5f);
                break;

            case (InfoJump.JumpType.BASE):
                Debug.Log("WE DESIDED: BASE");
                break;
        }
    }
}
