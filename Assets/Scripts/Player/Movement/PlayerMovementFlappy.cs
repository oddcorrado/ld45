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
public class PlayerMovementFlappy : PlayerMovementGround
{
    // Properties
    public bool IsWallDetached { get; set; }

    // Editor Props
    [SerializeField]
    private float gasAirVel = 0; // OK
    [SerializeField]
    private float maxAirSpeed = 0; // TODO

    // private fields
    private bool inFlight = false;

    override protected void Start()
    {
        base.Start();
    }

    protected override float JumpRatio()
    {
        float ratio = (/* jumpPreWarmRatio */ + (jumpPreWarmRatio) * ((Time.time - jumpStartDate) / jumpDeltaTime));

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
                    if (/*!IsGrounded && */ input.J)
                    {
                        vel.y = Mathf.Min(Mathf.Max(vel.y + gasAirVel, 0.1f), maxAirSpeed);
                        animationManager.CommandMovement = "flyIdle";
                        inFlight = true;
                    }
                    else if (/* !IsGrounded && */ !input.J && prevJ)
                    {
                        animationManager.CommandMovement = "air";
                        inFlight = false;
                    }
                }

            }
        }
        return vel;
    }
}
