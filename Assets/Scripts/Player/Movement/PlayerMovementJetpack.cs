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
[RequireComponent(typeof(InputRouter))]
[RequireComponent(typeof(PlayerCondition))]
public class PlayerMovementJetpack : PlayerMovementGround
{
    // Properties
    public bool IsWallDetached { get; set; }

    // Editor Props
    [SerializeField]
    private float gasAirVel = 0; // OK
    [SerializeField]
    private float maxAirSpeed = 0; // TODO

    /* public bool GetIsGrounded() { return IsGrounded; }
    public void SetIsGrounded(bool value) { IsGrounded = value; }
    public bool GetIsWalled() { return IsWalled; }
    public void SetIsWalled(bool value) { IsWalled = value; }
    public Collider2D GetWallCollider() { return WallCollider; }
    public void SetWallCollider(Collider2D value) { WallCollider = value; }
    public Vector3 GetNormal() { return Normal; }
    public void SetNormal(Vector3 value) { Normal = value; }
    public bool GetIsStunned() { return IsStunned; }
    public void SetIsStunned(bool value) { IsStunned = value; } */

    // private fields
    private bool inFlight = false;

    override protected void Start()
    {
        base.Start();
    }

    protected override float JumpRatio()
    {
        float ratio = (/* jumpPreWarmRatio */ +(jumpPreWarmRatio) * ((Time.time - jumpStartDate) / jumpDeltaTime));

        return ratio;
    }

    override protected Vector3 Jump(Vector3 vel)
    {
        // JUMP
        if (!playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            if ((IsGroundedForJump || jumpLeft > 0) && (input.J && !prevJ))
            {
                if (IsGroundedForJump)
                {
                    jumpLeft = multiJump; // FIXME
                    jumpStartDate = Time.time;
                    vel.y = jumpGroundVel;
                    inJump = true;
                }
                else
                {
                    vel.y = jumpGroundVel;
                    inJump = true;
                    jumpStartDate = Time.time;
                }

                jumpLeft--;
                animationManager.CommandMovement = "jump";
            }
            else
            {
                if (inJump)
                {
                    if (!input.J || Time.time > jumpStartDate + jumpDeltaTime)
                    {
                        vel.y += jumpGroundVel * JumpRatio();
                        inJump = false;
                    }
                    else
                    {
                        vel.y += jumpGroundVel * JumpRatio();
                    }
                }
                else
                {
                    if (!IsGroundedForJump && Mathf.Abs(input.Y) > 0.5f)
                    {
                        // vel.y = input.Y * gasAirVel;
                        var vely = walkGroundSpeed * input.Y;
                        var ai = airInertia <= Mathf.Epsilon ? 0f : 0.9f + airInertia * 0.01f;
                        Debug.Log(ai);
                        vel.y = vely * (1 - ai) + vel.y * ai;

                        animationManager.CommandMovement = "air";
                    }
                    else if (!IsGroundedForJump)
                    {
                        var ai = airInertia <= Mathf.Epsilon ? 0f : 0.9f + airInertia * 0.01f;
                        vel.y = 0 * (1 - ai) + vel.y * ai;
                        animationManager.CommandMovement = "flyIdle";
                    }
                }

            }
        }
        return vel;
    }

    protected override void FixedUpdate()
    {
        var vel = body.velocity;

        if (!IsLocked)
        {
            if (!IsWalled)
                ScaleX();
            var hor = input.X;
            if (IsLocked)
            {
                hor = 0;
            }
            vel = Horizontal(vel, hor);
            // body.AddForce(gravity);
            vel = Jump(vel);
            // vel = Dive(vel);
        }
        else
        {
            inJump = false; // fixme
            body.AddForce(gravity);
        }

        string runningAnim = "";
#if SPINE
        skeletonAnimation?.state?.Tracks.ForEach(t => runningAnim = t.Animation.Name);
#endif

        prevJ = input.J;
        //Debug.Log(IsGrounded + "/" + IsWalled + "/" + IsWallDetached);
        // Debug.Log(jumpLeft);
        body.velocity = vel;
    }
}
