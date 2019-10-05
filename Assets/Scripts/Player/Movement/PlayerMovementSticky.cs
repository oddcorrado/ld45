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
public class PlayerMovementSticky : PlayerMovement
{
    // Rebasables
    private float groundInertia;
    public float GroundInertia { get { return groundInertia; } set { groundInertia = value >= 0 ? value : groundInertiaBase; } }
    private float airInertia;
    public float AirInertia { get { return airInertia; } set { airInertia = value >= 0 ? value : airInertiaBase; } }
    private float walkGroundSpeed;
    public float WalkGroundSpeed { get { return walkGroundSpeed; } set { walkGroundSpeed = value >= 0 ? value : walkGroundSpeedBase; } }
    private float jumpGroundVel;
    public float JumpGroundVel { get { return jumpGroundVel; } set { jumpGroundVel = value >= 0 ? value : jumpGroundVelBase; } }
    private float jumpWallVel;
    public float JumpWallVel { get { return jumpWallVel; } set { jumpWallVel = value >= 0 ? value : jumpWallVelBase; } }
    private int multiJump;
    public int MultiJump { get { return multiJump; } set { multiJump = value >= 0 ? value : multiJump; } }
    private float diveVel;
    public float DiveVel { get { return diveVel; } set { diveVel = value >= 0 ? value : diveVelBase; } }

    private bool isJumping = false;

    // Editor Props
    [SerializeField]
    private float walkGroundSpeedBase = 10; // OK
    [SerializeField]
    private float jumpGroundVelBase = 15; // OK
    [SerializeField]
    private float jumpWallVelBase = 10; // OK
    [SerializeField]
    private float diveVelBase = 40; // OK
    [SerializeField]
    private float wallDetachCooldown = 1; // OK
    [SerializeField]
    private float groundInertiaBase = 0; // OK
    [SerializeField]
    private float airInertiaBase = 0; // OK
    [SerializeField]
    private int multiJumpBase = 1; // OK


    // private fields
    PlayerCondition playerCondition;

    new void Start()
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

        Normal = -gravity;

        Rebase();
    }

    public void Rebase()
    {
        GroundInertia = groundInertiaBase;
        AirInertia = airInertiaBase;
        WalkGroundSpeed = walkGroundSpeedBase;
        JumpGroundVel = jumpGroundVelBase;
        JumpWallVel = jumpWallVelBase;
        MultiJump = multiJumpBase;
        DiveVel = diveVelBase;
    }


    Vector3 Horizontal(Vector3 vel)
    {
        var hor = input.X;
        var newVel = Vector3.zero;

        // Debug.Log(hor);
        if (IsGrounded && !playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            newVel = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(WalkGroundSpeed * hor, 0, 0);
            Debug.DrawLine(transform.position, transform.position + new Vector3(vel.x, vel.y, 0), Color.red);
            Debug.DrawLine(transform.position, transform.position + new Vector3(newVel.x, newVel.y, 0), Color.white);

            var gi = GroundInertia < 0.8f ? 0f : 0.9f + GroundInertia * 0.1f;
            vel = newVel * (1 - gi) + vel * gi;
            isJumping = false;
        }
        /* else
        {
            newVel = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(WalkGroundSpeed * hor, 0, 0);
            var ai = AirInertia < 0.8f ? 0f : 0.9f + AirInertia * 0.1f;
            vel += newVel * (1 - ai) + vel * ai;
        } */ // FIXME

        Debug.DrawLine(transform.position, transform.position + new Vector3(Normal.x, Normal.y, 0) * 10, Color.yellow);
        if (IsGrounded)
            body.AddForce(-Normal.normalized * 20); // FIXME
        else
            body.AddForce(-Normal.normalized * (isJumping ? 20 : 100 * WalkGroundSpeed)); // FIXME

        return vel;
    }

    Vector3 Jump(Vector3 vel)
    {
        // JUMP
        if (!playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            if ((IsGrounded || jumpLeft > 0) && (input.J && !prevJ))
            {
                if(IsGrounded)
                {
                    jumpLeft = MultiJump; 
                    vel += Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(Normal.y, Normal.x) - 90) * new Vector3(0, jumpGroundVel, 0);
                }
                else
                    vel = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(Normal.y, Normal.x) - 90) * new Vector3(0, jumpGroundVel, 0);
                
                // vel.y = JumpGroundVel;
                jumpLeft--;
                isJumping = true;
            }
        }
        return vel;
    }

    Vector3 Dive(Vector3 vel)
    {
        if (!playerCondition.Handicaps[(int)PlayerCondition.Handicap.MOVE])
        {
            if (!IsGrounded)
            {
                if (input.Y < -0.5f)
                {
                    vel.y = Mathf.Min(vel.y, -DiveVel);
                }
            }
        }
        return vel;
    }

    void FixedUpdate()
    {
        var vel = body.velocity;


        ScaleX();
        vel = Horizontal(vel);
        vel = Jump(vel);
        vel = Dive(vel);

        prevJ = input.J;
        // Debug.Log(IsGrounded + "/" + IsWalled + "/" + IsWallDetached);
        // Debug.Log(jumpLeft);
        body.velocity = vel;
    }
}
