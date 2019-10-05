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
public class PlayerMovementOld : MonoBehaviour
{
    // enums
    enum MoveType {ground, gas, twod}

    // Privates
    private Rigidbody2D body;
    private new Collider2D collider;
    private InputRouter input;
    private readonly Vector3 gravity = new Vector2(0, -20);
    private int jumpLeft = 1;
    private bool prevJ = false;

    // Properties
    public bool IsGrounded { get; set; }
    public bool IsWalled { get; set; }
    public bool IsWallDetached { get; set; }
    public Collider2D WallCollider { get; set; }
    public Vector3 Normal { get; set; }

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
    private float gasAirVel;
    public float GasAirVel { get { return gasAirVel; } set { gasAirVel = value >= 0 ? value : gasAirVelBase; } }
    private float diveVel;
    public float DiveVel { get { return diveVel; } set { diveVel = value >= 0 ? value : diveVelBase; } }


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
    [SerializeField]
    private bool isSticky = false; // TODO
    [SerializeField]
    private MoveType moveType;
    [SerializeField]
    private float gasAirVelBase = 0; // OK
    [SerializeField]
    private float maxAirSpeed = 0; // TODO

	void Start () 
    {
        body = GetComponent<Rigidbody2D>();
        Debug.Assert(body != null, "could not find player collider");

        collider = GetComponent<Collider2D>();
        Debug.Assert(collider != null, "could not find player collider");

        input = GetComponent<InputRouter>();
        Debug.Assert(collider != null, "could not find input router");

        IsWallDetached = true;
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
        GasAirVel = gasAirVelBase;
        DiveVel = diveVelBase;
    }

    IEnumerator WallDetach()
    {
        IsWallDetached = false;
        yield return new WaitForSeconds(wallDetachCooldown);
        IsWallDetached = true;
    }

	void FixedUpdate ()
    {
        var hor = input.X;
        var vel = body.velocity;
        var newVel = Vector3.zero;

        // HORIZONTAL
        if(IsGrounded)
        {
            newVel = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(WalkGroundSpeed * hor, 0, 0);
            Debug.DrawLine(transform.position, transform.position + new Vector3(vel.x, vel.y, 0), Color.red);
            Debug.DrawLine(transform.position, transform.position + new Vector3(newVel.x, newVel.y, 0), Color.white);
            var gi = GroundInertia < 0.8f ? 0f : 0.9f + GroundInertia * 0.1f;
            vel.x = newVel.x * (1 - gi) + vel.x * gi;
            // vel.y = newVel.y;
            if (isSticky) body.AddForce(-Normal.normalized * 20); // FIXME
        }
        else if (moveType == MoveType.gas || moveType == MoveType.twod || (!IsWalled && IsWallDetached)) 
        {
            // Debug.Log(dbgcnt++);
            newVel = new Vector3(WalkGroundSpeed * hor, 0, 0);
            var gi = AirInertia < 0.8f ? 0f : 0.9f + AirInertia * 0.1f;
            vel.x = newVel.x * (1 - gi) + vel.x * gi;
            if(moveType != MoveType.twod)
            {
                if (isSticky)
                    body.AddForce(-Normal.normalized * 20); // FIXME
                else
                    body.AddForce(gravity);
            }
        }
        // else
        {
            if (isSticky)
                body.AddForce(-Normal.normalized * 20); // FIXME
            else
            {
                body.AddForce(gravity);
                if (IsWalled)
                    vel.y = Mathf.Max(-1, vel.y); 
            }
        }

//        Debug.Log(jumpLeft);

        // JUMP
        if (moveType == MoveType.twod)
        {
        }
        else if(moveType == MoveType.gas)
        {
            if (input.J)
            {
                if (vel.y < 0) vel.y = 0;
                vel.y = Mathf.Min(vel.y + GasAirVel, maxAirSpeed);
                Debug.Log(GasAirVel + " " + vel);
            }
        }
        else
        {
            if (WallCollider != null)
            {
                ColliderDistance2D distance2D = collider.Distance(WallCollider);
                // float angle = Mathf.Rad2Deg * Mathf.Atan2(distance2D.normal.y, distance2D.normal.x);

                if (input.J && !prevJ)
                {
                    vel.y += JumpGroundVel;
                    vel.x += -distance2D.normal.x * JumpWallVel;
                    StartCoroutine(WallDetach());
                    jumpLeft = multiJump - 1; // FIXME
                }
            }
            else
            {
                if ((IsGrounded || jumpLeft > 0) && (input.J && !prevJ))
                {
                    if (IsGrounded)
                    {
                        jumpLeft = MultiJump; // FIXME
                        vel.y += JumpGroundVel;
                    }
                    else
                        vel.y = JumpGroundVel;
                    jumpLeft--;
                }
            }
        }

        // DIVE
        if(moveType == MoveType.twod)
        {
            vel.y = input.Y * DiveVel;
        }
        else if(!IsGrounded) 
        {
            if(input.Y < -0.5f) 
            {
                vel.y = Mathf.Min(vel.y, -DiveVel);
            }
        }
        prevJ = input.J;
        // Debug.Log(IsGrounded + "/" + IsWalled + "/" + IsWallDetached);
        Debug.Log(jumpLeft);
        body.velocity = vel;

	}
}
