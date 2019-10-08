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
[RequireComponent(typeof(PlayerCondition))]
public class PlayerMovementGroundSticky : PlayerMovement
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
    private bool isSticky = true; // TODO
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
    protected PlayerCondition playerCondition;
    protected AnimationManager animationManager;
    protected float jumpStartDate = 0;
    protected bool inJump = false;
    protected bool inWallJump = false;
    protected GroundTester groundTester;
    protected NormalTester normalTester;
    protected CrouchBlocker crouchBlocker;
    protected float horLock;
    private Vector3 jumpNormal;
    private ParticleSystem.MainModule partSystem;
    public bool CanJump { get; set; }

    private float sizeMul = 1;
    public float SizeMul
    {
        get { return sizeMul; }
        set 
        {
            if (Mathf.Abs(sizeMul-value) <= Mathf.Epsilon) return;
            sizeMul = value;
            transform.localScale = new Vector2(sizeMul, sizeMul);
        }
    }
    public float WalkGroundSpeed { get { return walkGroundSpeed; } set { walkGroundSpeed = value; } }
    public bool IsSticky
    {
        get { return isSticky;  }
        set
        {
            if (value == isSticky) return;
            isSticky = value;

            normalTester.MakeSticky(isSticky);
        }
    }
    override protected void Start()
    {
        base.Start();

        body = GetComponent<Rigidbody2D>();
        Debug.Assert(body != null, "could not find player collider");

        collider = GetComponent<Collider2D>();
        Debug.Assert(collider != null, "could not find player collider");

        input = GetComponent<InputRouter>();
        Debug.Assert(collider != null, "could not find input router");

        playerCondition = GetComponent<PlayerCondition>();
        Debug.Assert(playerCondition != null, "could not find playerCondition");

        animationManager = GetComponent<AnimationManager>();
        Debug.Assert(animationManager != null, "could not find animationManager");

        groundTester = transform.Find("GroundTester")?.GetComponent<GroundTester>();
        Debug.Assert(groundTester != null, "could not find groundTester");

        normalTester = transform.Find("NormalTester")?.GetComponent<NormalTester>();
        Debug.Assert(normalTester != null, "could not find NormalTester");

        crouchBlocker = transform.Find("CrouchBlocker")?.GetComponent<CrouchBlocker>();
        Debug.Assert(crouchBlocker != null, "could not find CrouchBlocker");

        Debug.Log(name + " " + walkGroundSpeed);

        //Search Particle system
        Transform pxBlob = transform.Find("PxBlobs");
        Debug.Assert(pxBlob != null, "could not find PxBlobs");
        Transform rabbits = pxBlob.transform.Find("Rabbits");
        Debug.Assert(rabbits != null, "could not find Rabbits");
        partSystem = rabbits.GetComponent<ParticleSystem>().main;

        //Physics2D.gravity = Vector2.zero;
        Debug.Log(name + " " + playerCondition);
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

    private bool CanCrouch()
    {
        if (crouchBlocker.Block) { /* Debug.Log("BLOCK"); */ return false; }


        if (Normal.magnitude < Mathf.Epsilon) { /* Debug.Log("ZERO NORMAL"); */ return false; }

        if (Mathf.Abs(Normal.x) < Mathf.Epsilon) { /* Debug.Log("VERTICAL NORMAL " + Normal); */ return true; }

        if (Mathf.Abs((Normal.y / Normal.x)) > 0.5f) { /* Debug.Log("OK NORMAL" + Mathf.Abs((Normal.y / Normal.x))); */ return true; }

        // Debug.Log("HOR NORMAL");

        return false;
    }

    protected virtual Vector3 Horizontal(Vector3 vel, float hor)
    {
        var newVel = Vector3.zero;
        if (ForcedInput != null)
        {
            hor = ForcedInput.X;
        }

        // if(!playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            if (IsGrounded && !IsWalled)
            {
                // newVel = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(WalkGroundSpeed * hor, 0, 0);
                var angle = Mathf.Rad2Deg * Mathf.Atan2(Normal.y, Normal.x) - 90;
                // Debug.Log(Normal + " "+ angle);
                Debug.DrawLine(transform.position, transform.position + new Vector3(Normal.x, Normal.y, 0) * 10, Color.yellow);
                if (input.Y < -0.1f && CanCrouch()) hor *= 0f;
                if(!isSticky)
                    newVel = Quaternion.Euler(0, 0, angle) * new Vector3(sizeMul * walkGroundSpeed * hor, 0, 0);
                else
                    //newVel = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(sizeMul * WalkGroundSpeed * hor, 0, 0);
                    newVel = sizeMul * walkGroundSpeed * new Vector3(input.X, input.Y, 0);
                // Debug.DrawLine(transform.position + new Vector3(0, 0.2f, 0), transform.position + new Vector3(0, 0.2f, 0) + new Vector3(vel.x, vel.y, 0), Color.red);
                Debug.DrawLine(transform.position, transform.position + new Vector3(newVel.x, newVel.y, 0), Color.white);
                var gi = groundInertia <= Mathf.Epsilon ? 0f : 0.9f + groundInertia * 0.01f;
                // if (input.Y > -0.1f)
                {
                    if (!isSticky)
                        vel.x = newVel.x * (1 - gi) + vel.x * gi;
                    else
                        vel = newVel * (1 - gi) + vel * gi; 
                }
                /* else
                {
                    if (!isSticky)
                        vel.x = 0;
                    else
                        vel = Vector3.zero;
                } */

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
                newVel = new Vector3(walkGroundSpeed * hor * sizeMul, 0, 0);
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


        vel.y = Mathf.Max(vel.y, -maxGravityVelocity *  sizeMul);

        if(!isSticky)
        {
            if (IsGrabbing && Mathf.Abs(hor) > 0.5f)
            {
                vel.y = jumpWallVel.y;
                animationManager.CommandMovement = animationLedgeJump;
            }  
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

        return Mathf.Pow(sizeMul, 0.75f) * ratio;
    }

    private Vector3 JumpVel(Vector3 vel, bool reset)
    {
        if (!isSticky)
            vel.y = jumpGroundVel * JumpRatio();
        else
        {
            if (reset) jumpNormal = Normal;
            if(jumpNormal.magnitude > Mathf.Epsilon)
                vel = jumpNormal.normalized * jumpGroundVel * JumpRatio();
            else
                vel.y = jumpGroundVel * JumpRatio();
        }

        return vel;
    }

    protected virtual Vector3 Jump(Vector3 vel)
    {
        if (!CanJump) return vel;

        if (IsGrabbing)
        {
            animationManager.Movement = animationLedge;
            return vel;
        }


        if (IsWalled && !isSticky)
        {
            animationManager.CommandMovement = null;
            animationManager.Movement = animationWall;
            vel.y = Mathf.Max(0, vel.y);
        }
        // JUMP
        if (!playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {

            if (WallCollider != null && !isSticky)
            {
                ColliderDistance2D distance2D = collider.Distance(WallCollider);
                if ((input.J && !prevJ)
                    //|| (Mathf.Abs(input.X) > 0.5f && !input.A && !input.B && !input.C)
                    || (input.Y < -0.5f && !input.A && !input.B && !input.C)
                   )
                {
                    var velx = -distance2D.normal.x * jumpWallVel.x * (distance2D.isOverlapped ? 1 : -1);
                    velx *= velx > 0 ? (input.X + 1.8f) : (1.8f - input.X);
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
                if ((IsGroundedForJump || jumpLeft > 0) && (input.J && !prevJ))
                {
                    if (IsGroundedForJump)
                    {
                        jumpLeft = multiJump; // FIXME
                        jumpStartDate = Time.time;
                        vel = JumpVel(vel, true);
                        inJump = true;
                    }
                    else
                    {
                        vel = JumpVel(vel, false);
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
                        if (!input.J || Time.time > jumpStartDate + jumpDeltaTime)
                        {
                            vel = JumpVel(vel, false);
                            inJump = false;
                        }
                        else
                        {
                            vel = JumpVel(vel, false);
                        }
                    }

                }
            }
        }
        return vel;
    }

    protected virtual Vector3 Dive(Vector3 vel)
    {
        if (!IsWalled && !playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            if (!IsGrounded)
            {
                if (input.Y < -0.5f && vel.y < 0 && Mathf.Abs(input.X) < 0.5f)
                {
                    vel.y = Mathf.Min(vel.y, -diveVel);
                }
            }
            else
            {
                
                if (/* Mathf.Abs(input.X) < 0.1f && */ input.Y < -0.5f && CanCrouch()) // && normalTester.SurfaceCount < 2)
                {
                    // Debug.Log("normalTester.SurfaceCount" + normalTester.SurfaceCount);
                    // groundTester.Traverse();
                    /* if(transform.localScale.y > 0.5f)
                        transform.localScale += new Vector3(0.05f * sizeMul * Mathf.Sign(transform.localScale.x), -0.05f * sizeMul, 1); */
                    transform.localScale = new Vector3(1 * Mathf.Sign(transform.localScale.x), 0.1f, 1) * sizeMul;
                }
                else
                {
                    
                    /* if (transform.localScale.y < sizeMul)
                        transform.localScale += new Vector3(-0.05f * sizeMul * Mathf.Sign(transform.localScale.x), 0.05f * sizeMul, 1);
                    else */
                        transform.localScale = new Vector3(1 * Mathf.Sign(transform.localScale.x), 1, 1) * sizeMul;
                }
            }
        }

        return vel;
    }

    protected virtual void FixedUpdate()
    {
        partSystem.startLifetimeMultiplier = sizeMul;
        partSystem.startSizeMultiplier = sizeMul;
        var vel = body.velocity;
        if (!IsWalled && !IsLocked) ScaleX();
        // Debug.Log(Normal);
        if (isSticky && Normal.magnitude > Mathf.Epsilon)
            gravity = -50 * Normal * Mathf.Pow(sizeMul, 0.5f);
        else
            gravity = new Vector3(0, -50, 0) * Mathf.Pow(sizeMul, 0.5f);
        if (!IsLocked && !playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            float horVel = playerCondition.Handicaps[(int)PlayerCondition.Handicap.SLOW] ? input.X * 0.2f : input.X;
            horVel = playerCondition.Handicaps[(int)PlayerCondition.Handicap.INVERT] ? -horVel : horVel;
            horLock = horVel;
            vel = Horizontal(vel, horVel);
            if (!IsGrabbing) body.AddForce(gravity * GravityMul);
            if (!playerCondition.Handicaps[(int)PlayerCondition.Handicap.JUMP]) vel = Jump(vel);
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
            
        prevJ = input.J;
        // Debug.Log(name + "/" + IsGrounded + "/" + IsWalled);
        // Debug.Log(jumpLeft);
        body.velocity = vel;
    }
}
