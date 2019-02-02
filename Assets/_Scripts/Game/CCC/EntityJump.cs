using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InfoJump
{
    public bool didWeHit;
    public Vector3 pointHit;
    public Vector3 normalHit;
    public Transform objHit;
    public Vector3 dirUltimatePlotPoint;
    public Vector3 ultimatePlotPoint;

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
    }
}

public class EntityJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected float jumpHeight = 1f;
    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float timeFeforeCalculateAgainJump = 0.5f;
    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastForNormalSwitch = 5f;


    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool stayHold = false;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool canJumpInAir = true;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Transform playerLocalyRotate;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityAction entityAction;
    
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityAttractor entityAttractor;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected GroundCheck groundCheck;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected bool hasJumped = false;
    public bool HasJumped { get { return (hasJumped); } }
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justJumpedTimer = 0.1f;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justGroundTimer = 0.1f;
    [FoldoutGroup("Debug"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1.4f;

    protected FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDownOnGround = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDowwnBeforeCalculateAgain = new FrequencyCoolDown();
    public bool IsReadyToTestCalculation() { return (coolDowwnBeforeCalculateAgain.IsStartedAndOver()); }
    private bool normalGravityTested = false;   //know if we are in the 0.5-0.8 sec between norma and attractor

    private InfoJump infoJump = new InfoJump();
    protected bool jumpStop = false;
    private RaycastHit hitInfo;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        jumpStop = false;
        hasJumped = false;
    }

    public bool IsJumpCoolDebugDownReady()
    {
        return (coolDownWhenJumped.IsReady());
    }

    public bool IsJumpedAndNotReady()
    {
        return (hasJumped && !IsJumpCoolDebugDownReady());
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
            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 0.1f);
        }
        return (results);
    }

    /// <summary>
    /// do raycast along the Plots, and reutrn true if we hit
    /// </summary>
    /// <param name="infoPlot"></param>
    /// <returns></returns>
    public bool DoRaycast(Vector3[] infoPlot, int depth = 2)
    {
        Vector3 prevPos = rb.transform.position;

        for (int i = 0; i <= depth; i++)
        {
            Vector3 dirRaycast;
            Vector3 lastPoint;

            if (i == depth)
            {
                dirRaycast = infoJump.dirUltimatePlotPoint.normalized * distRaycastForNormalSwitch;
                lastPoint = infoJump.ultimatePlotPoint;
                //lastPoint pas utile de le mettre ?
            }
            else
            {
                int indexPoint = ((infoPlot.Length / depth) * (i + 1)) - 1;
                Debug.Log("index: " + indexPoint + "(max: " + infoPlot.Length);
                lastPoint = infoPlot[indexPoint];

                dirRaycast = lastPoint - prevPos;
            }

            Debug.DrawRay(prevPos, dirRaycast, Color.red, 5f);
            if (Physics.SphereCast(prevPos, 0.3f, dirRaycast, out hitInfo,
                                   dirRaycast.magnitude, entityController.layerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("find something ! keep going with normal gravity");
                infoJump.didWeHit = true;
                infoJump.normalHit = hitInfo.normal;
                infoJump.objHit = hitInfo.transform;
                infoJump.pointHit = hitInfo.point;
                Debug.DrawRay(hitInfo.point, infoJump.normalHit, Color.magenta, 5f);
                return (true);
            }
            prevPos = lastPoint;
        }
        return (false);
    }

    private bool RaycastForward(Vector3 pos, Vector3 dir, float length, float radius)
    {
        Debug.DrawRay(pos, dir * length, Color.cyan, 5f);

        if (Physics.SphereCast(pos, radius, dir, out hitInfo,
                                length, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("ATTRACTOR find something in stage 2 ! normal gravity !!");
            return (true);
        }
        return (false);
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

        bool hit = DoRaycast(plots);    //return true if we hit a wall in the first jump plot
        //if (!hit)
        //    hit = ForwardRaycastJump(plots);    //return true if we hit a wall in the long raycast after the 30 plots

        //here, we hit something at some point !
        if (hit)
        {
            Vector3 normalJump = playerGravity.GetMainAndOnlyGravity();
            Vector3 normalHit = infoJump.normalHit;

            float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
            if (dotImpact > 0)
            {
                Debug.Log("we hit way !");
                playerGravity.SetObjectAttraction(infoJump.objHit, infoJump.pointHit);
            }
            else
            {
                Debug.Log("No way we climb That !, Obstacle to inclined");
            }

        }
        else
        {
            Debug.Log("no hit... fack");
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

        bool hit = DoRaycast(plots, 1);    //return true if we hit a wall in the first jump plot

        if (!hit)
        {
            infoJump.Clear();
            //create plot WITOUT force down
            plots = Plots(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 30, false, false);
            infoJump.SetDirLast(plots);

            hit = DoRaycast(plots, 1);
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

    public virtual void OnGrounded()
    {
        normalGravityTested = false;
        coolDowwnBeforeCalculateAgain.Reset();
        Debug.Log("Grounded !");
        
        coolDownWhenJumped.Reset();
        //here, we just were falling, without jumping
        if (!hasJumped)
        {
            coolDownOnGround.StartCoolDown(justGroundTimer);
        }
        //here, we just on grounded after a jump
        else
        {
            //rb.ClearVelocity();
            coolDownOnGround.StartCoolDown(justGroundTimer);
            hasJumped = false;
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.

        //reduce height when max speed
        float jumpBoostHeight = jumpHeight / (1 + (1 * entityAction.GetMagnitudeInput()));

        //Debug.Log("boost height: " + jumpBoostHeight);
        return Mathf.Sqrt(2 * jumpBoostHeight * playerGravity.Gravity);
    }

    private Vector3 AddJumpHeight(Vector3 normalizedDirJump, float boost = 1f)
    {
        float jumpSpeedCalculate = CalculateJumpVerticalSpeed() * boost;
        Vector3 jumpForce = normalizedDirJump * jumpSpeedCalculate;

        Debug.DrawRay(rb.position, jumpForce, Color.red, 5f);
        return (jumpForce);
    }

    /// <summary>
    /// return the normalized jump dir()
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNormalizedJumpDir()
    {
        Vector3 normalizedNormalGravity = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalizedForwardPlayer = playerLocalyRotate.forward * entityAction.GetMagnitudeInput();

        Debug.DrawRay(rb.position, normalizedNormalGravity, Color.yellow, 5f);
        Debug.DrawRay(rb.position, normalizedForwardPlayer, Color.green, 5f);

        return (normalizedNormalGravity + normalizedForwardPlayer);
    }

    /// <summary>
    /// do a jump
    /// </summary>
    protected void DoJump()
    {
        Vector3 dirJump = GetNormalizedJumpDir();
        Vector3 orientedStrenghtJump = AddJumpHeight(dirJump);

        rb.velocity = orientedStrenghtJump;
        //playerGravity.JustJump();
        //JustJump();

        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Jump, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);
    }
}
