using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BULLETS homing front autotarget
// PUNCHES
// AREAS
// CHARGE
// DIVE CHARGE
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class AiMovement : AiMovementBase
{
    // Editor Props
    [SerializeField]
    protected float walkGroundSpeed = 6; // OK
    [SerializeField]
    protected float jumpGroundVel = 13; // OK
    [SerializeField]
    private Vector2 jumpWallVel = new Vector2(3, 12); // OK
    [SerializeField]
    private float diveVel = 20; // OK
    [SerializeField]
    private float wallDetachCooldown = 1; // OK
    [SerializeField]
    protected float groundInertia = 0.1f; // OK
    [SerializeField]
    protected float airInertia = 0.2f; // OK
    [SerializeField]
    protected int multiJump = 1; // OK
    [SerializeField]
    private bool isSticky = false; // TODO
    [SerializeField]
    protected float jumpDeltaTime = 0.16f; // TODO
    [SerializeField]
    protected float jumpPreWarmRatio = 0.5f; // TODO
    [SerializeField]
    private float maxGravityVelocity = 8;
    [SerializeField] string animationRun = "run";
    [SerializeField] string animationIdle = "idle";
    [SerializeField] string animationJump = "jump";
    [SerializeField] string animationWall = "wall";
    [SerializeField] string animationWallJump = "wallJump";
    [SerializeField] string animationLedge = "ledge";
    [SerializeField] string animationLedgeJump = "ledgeJump";
    [SerializeField] string animationAir = "air";

    // private fields
    //protected PlayerCondition playerCondition;
    protected AnimationManager animationManager;
    protected float jumpStartDate = 0;
    protected bool inJump = false;
    protected bool inWallJump = false;
    protected AiGroundTester groundTester;
    protected float horLock;

    public float WalkGroundSpeed { get { return walkGroundSpeed; } set { walkGroundSpeed = value; } }

    override protected void Start()
    {
        base.Start();

        body = GetComponent<Rigidbody2D>();
        Debug.Assert(body != null, "could not find player collider");

        collider = GetComponent<Collider2D>();
        Debug.Assert(collider != null, "could not find player collider");

        input = GetComponent<AiInput>();
        Debug.Assert(collider != null, "could not find input router");

        /*
        playerCondition = GetComponent<PlayerCondition>();
        Debug.Assert(playerCondition != null, "could not find playerCondition");
        */

        animationManager = GetComponent<AnimationManager>();
        Debug.Assert(animationManager != null, "could not find animationManager");

        groundTester = transform.Find("GroundTester")?.GetComponent<AiGroundTester>();
        Debug.Assert(groundTester != null, "could not find groundTester");

        //Debug.Log(name + " " + walkGroundSpeed);

        //Physics2D.gravity = Vector2.zero;
        //Debug.Log(name + " " + playerCondition);
        WallDetachState = WallDetach.CONTROL;
        Rebase();
    }

    public void Rebase()
    {
    }

    IEnumerator WallDetachTimer()
    {
        WallDetachState = WallDetach.ATTACHED;
        yield return new WaitForSeconds(wallDetachCooldown);
        WallDetachState = WallDetach.DETACHED;
    }


    protected virtual Vector3 Horizontal(Vector3 vel, float hor)
    {
        // Debug.Log(vel + " " + hor);

        var newVel = Vector3.zero;

        // if(!playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            if (IsGrounded && !IsWalled)
            {
                // newVel = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(WalkGroundSpeed * hor, 0, 0);
                var angle = Mathf.Rad2Deg * Mathf.Atan2(Normal.y, Normal.x) - 90;
                Debug.DrawLine(transform.position, transform.position + new Vector3(Normal.x, Normal.y, 0) * 10, Color.yellow);
                newVel = Quaternion.Euler(0, 0, angle) * new Vector3(walkGroundSpeed * hor, 0, 0);
                // Debug.DrawLine(transform.position + new Vector3(0, 0.2f, 0), transform.position + new Vector3(0, 0.2f, 0) + new Vector3(vel.x, vel.y, 0), Color.red);
                Debug.DrawLine(transform.position, transform.position + new Vector3(newVel.x, newVel.y, 0), Color.white);
                var gi = groundInertia <= Mathf.Epsilon ? 0f : 0.9f + groundInertia * 0.01f;
                vel.x = newVel.x * (1 - gi) + vel.x * gi;
                // body.AddForce(new Vector2(newVel.x, 0));

                if (Mathf.Abs(angle) > 2)
                {
                    animationManager.Movement = animationLedge;
                }
                else
                {
                    if (Mathf.Abs(hor) < Mathf.Epsilon)
                        animationManager.Movement = animationIdle;
                    else
                        animationManager.Movement = animationRun;
                }

            }
            else if (!IsWalled && WallDetachState == WallDetach.CONTROL)
            {
                newVel = new Vector3(walkGroundSpeed * hor, 0, 0);
                var ai = airInertia <= Mathf.Epsilon ? 0f : 0.9f + airInertia * 0.01f;
                // Debug.Log(vel.x + " " + newVel.x);
                vel.x = newVel.x * (1 - ai) + vel.x * ai;
                animationManager.Movement = animationAir;
            }
            else if (!IsWalled && WallDetachState == WallDetach.DETACHED)
            {
                if (Mathf.Abs(hor) > Mathf.Epsilon)
                {
                    WallDetachState = WallDetach.CONTROL;
                }
                animationManager.Movement = animationAir;
            }
        }

        vel.y += ExtraVel.y;
        vel.x += ExtraVel.x;
        ExtraVel *= 0.9f; // FIXME

        vel.y = Mathf.Max(vel.y, -maxGravityVelocity);

        if (IsGrabbing && Mathf.Abs(hor) > 0.5f)
        {
            vel.y = jumpWallVel.y;
            animationManager.CommandMovement = animationLedgeJump;
        }

        return vel;
    }

    protected virtual void ScaleXForce(float hor)
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * hor > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z);
    }

    protected virtual float JumpRatio()
    {
        float ratio = (jumpPreWarmRatio + (1 - jumpPreWarmRatio) * ((Time.time - jumpStartDate) / jumpDeltaTime));

        return ratio;
    }

    protected virtual Vector3 Jump(Vector3 vel)
    {
        if (IsGrabbing)
        {
            animationManager.Movement = animationLedge;
            return vel;
        }


        if (IsWalled)
        {
            animationManager.CommandMovement = null;
            animationManager.Movement = animationWall;
            vel.y = Mathf.Max(0, vel.y);
        }
        // JUMP

        /*
        if (WallCollider != null)
        {
            ColliderDistance2D distance2D = collider.Distance(WallCollider);
            if ((input.J && !prevJ)
                //|| (Mathf.Abs(input.horizontalSpeed) > 0.5f && !input.A && !input.B && !input.C)
                || (input.verticalSpeed < -0.5f && !input.A && !input.B && !input.C)
               )
            {
                var velx = -distance2D.normal.x * jumpWallVel.x * (distance2D.isOverlapped ? 1 : -1);
                velx *= velx > 0 ? (input.horizontalSpeed + 1.8f) : (1.8f - input.horizontalSpeed);
                vel.y = jumpWallVel.y;
                vel.x += velx;
                ScaleXForce(velx);
                StartCoroutine(WallDetachTimer());
                jumpLeft = multiJump - 1; // FIXME
                animationManager.CommandMovement = animationWallJump;
                inWallJump = true;
                jumpStartDate = Time.time;
            }
        }
        else
        {
            */
        if ((IsGroundedForJump || jumpLeft > 0) && (input.jump && !prevJ))
        {
            if (IsGroundedForJump)
            {
                jumpLeft = multiJump; // FIXME
                jumpStartDate = Time.time;
                vel.y = jumpGroundVel * JumpRatio();
                inJump = true;
            }
            else
            {
                vel.y = jumpGroundVel * JumpRatio();
                inJump = true;
                jumpStartDate = Time.time;
            }

            jumpLeft--;
            animationManager.CommandMovement = animationJump;
        }
        else
        {
            if (inJump)
            {
                if (!input.jump || Time.time > jumpStartDate + jumpDeltaTime)
                {
                    vel.y = jumpGroundVel * JumpRatio();
                    inJump = false;
                }
                else
                {
                    vel.y = jumpGroundVel * JumpRatio();
                }
            }

        }
        //}
        return vel;
    }

    protected virtual Vector3 Dive(Vector3 vel)
    {
            if (!IsGrounded)
            {
                if (input.verticalSpeed < -0.5f && vel.y < 0 && Mathf.Abs(input.horizontalSpeed) < 0.5f)
                {
                    vel.y = Mathf.Min(vel.y, -diveVel);
                }
            }
            else
            {
                if (Mathf.Abs(input.horizontalSpeed) < 0.1f && input.verticalSpeed < -0.5f)
                {
                    groundTester.Traverse();
                }

            }
        return vel;
    }

    protected virtual void FixedUpdate()
    {
        var vel = body.velocity;
        vel.x = input.horizontalSpeed;

        if (!IsWalled && !IsLocked) ScaleX();


        if (!IsLocked)
        {
            float horVel = input.horizontalSpeed;
            horLock = horVel;
            vel = Horizontal(vel, horVel);
            if (!IsGrabbing) body.AddForce(gravity * GravityMul);
            vel = Jump(vel);
            vel = Dive(vel);
        }
        else
        {
            if (IsGrounded)
            {
                //horLock = 0;
                vel = Horizontal(vel, 0);
            }
            else
            {
                vel = Horizontal(vel, horLock);
            }
            if (!IsGrabbing) body.AddForce(gravity * GravityMul);
        }

#if SPINE
        string runningAnim = "";

        skeletonAnimation?.state?.Tracks.ForEach(t => runningAnim = t.Animation.Name);
#endif

        prevJ = input.jump;
        // Debug.Log(name + "/" + IsGrounded + "/" + IsWalled);
        // Debug.Log(jumpLeft);
        body.velocity = vel;
    }
}
