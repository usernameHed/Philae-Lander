using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected float jumpHeight = 1f;
    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float timeFeforeCalculateAgainJump = 0.5f;

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool stayHold = false;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool canJumpInAir = true;
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

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected bool hasJumped = false;
    public bool HasJumped { get { return (hasJumped); } }
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justJumpedTimer = 0.1f;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justGroundTimer = 0.1f;

    protected FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDownOnGround = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDowwnBeforeCalculateAgain = new FrequencyCoolDown();
    public bool IsReadyToTestCalculation() { return (coolDowwnBeforeCalculateAgain.IsStartedAndOver()); }

    protected bool jumpStop = false;

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


    public void JumpCalculation()
    {
        //reset jump first test timer
        coolDowwnBeforeCalculateAgain.StartCoolDown(timeFeforeCalculateAgainJump);
        normalGravityTested = false;
        dontApplyForceDownForThisRound = false;

        //first create 30 plot of the normal jump
        Vector3[] plots = Plot(rb, rb.transform.position, rb.velocity, 30, false, true);
        infoJump.Clear();
        infoJump.SetDirLast(plots);

        bool hit = DoRaycast(plots);    //return true if we hit a wall in the first jump plot
        //if (!hit)
        //    hit = ForwardRaycastJump(plots);    //return true if we hit a wall in the long raycast after the 30 plots

        //here, we hit something at some point !
        if (hit)
        {
            Vector3 normalJump = GetMainAndOnlyGravity();
            Vector3 normalHit = infoJump.normalHit;

            float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
            if (dotImpact > 0)
            {
                Debug.Log("we hit way !");
                currentOrientation = OrientationPhysics.OBJECT;
                mainAttractObject = infoJump.objHit;
                mainAttractPoint = infoJump.pointHit;
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
        Debug.Break();
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
        Vector3[] plots = Plot(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 16, false, true);

        infoJump.SetDirLast(plots);

        bool hit = DoRaycast(plots, 1);    //return true if we hit a wall in the first jump plot

        if (!hit)
        {
            infoJump.Clear();
            //create plot WITOUT force down
            plots = Plot(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 30, false, false);
            infoJump.SetDirLast(plots);

            hit = DoRaycast(plots, 1);
            if (hit)
            {
                Debug.Log("hit the long run ! desactive force down for this one !");
                dontApplyForceDownForThisRound = true;
            }
        }

        if (hit)
        {
            currentOrientation = OrientationPhysics.OBJECT;
            mainAttractObject = infoJump.objHit;
            mainAttractPoint = infoJump.pointHit;
        }


        //raycast

        //currentOrientation = OrientationPhysics.OBJECT;
        Debug.Break();
    }

    public virtual void OnGrounded()
    {
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
    /*
    /// <summary>
    /// we just jump
    /// if we hit something in the plot and raycast: pass to OBJECT ORIENTED (a kind of swithc planet)
    /// </summary>
    private void JustJump()
    {
        //reset jump first test timer
        timerBeforeTestingNewNormalGravity.StartCoolDown(timeBeforeTestingNewNormalGravity);
        normalGravityTested = false;
        dontApplyForceDownForThisRound = false;

        //first create 30 plot of the normal jump
        Vector3[] plots = Plot(rb, rb.transform.position, rb.velocity, 30, false, true);
        infoJump.Clear();
        infoJump.SetDirLast(plots);

        bool hit = DoRaycast(plots);    //return true if we hit a wall in the first jump plot
        //if (!hit)
        //    hit = ForwardRaycastJump(plots);    //return true if we hit a wall in the long raycast after the 30 plots

        //here, we hit something at some point !
        if (hit)
        {
            Vector3 normalJump = GetMainAndOnlyGravity();
            Vector3 normalHit = infoJump.normalHit;

            float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
            if (dotImpact > 0)
            {
                Debug.Log("we hit way !");
                currentOrientation = OrientationPhysics.OBJECT;
                mainAttractObject = infoJump.objHit;
                mainAttractPoint = infoJump.pointHit;
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
        Debug.Break();
    }
    */
}
