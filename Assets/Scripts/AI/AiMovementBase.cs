using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Spine.Unity;

public class AiMovementBase : MonoBehaviour
{
    // Privates
    protected Rigidbody2D body;
    protected new Collider2D collider;
    protected AiInput input;
    protected int jumpLeft = 1;
    protected bool prevJ = false;
    protected Coroutine stunCoroutine = null;
    protected Coroutine animationCoroutine = null;
    // protected SkeletonAnimation skeletonAnimation;
    [SerializeField]
    protected Vector3 gravity = new Vector2(0, -20);
    public int GravityMul { get; set; }

    //[SerializeField]
    protected float ungroundDelay = 0.1f;

    protected virtual void Start()
    {
        Debug.Log("Start " + name);
        // skeletonAnimation = transform?.Find("Spine")?.GetComponent<SkeletonAnimation>();
        GravityMul = 1;
    }

    public class Input
    {
        public float X;
        public float Y;
        public float J;
    }

    // enums
    public enum WallDetach { ATTACHED, DETACHED, CONTROL }

    // Properties
    private bool isGrounded;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            if (value == false && isGrounded)
            {
                ungroundCoroutine = StartCoroutine(UngroundDelay());
                isGrounded = false;
            }

            if (value == true && !isGrounded)
            {
                isGrounded = true;
                IsGroundedForJump = true;
                if (ungroundCoroutine != null)
                {
                    StopCoroutine(ungroundCoroutine);
                    ungroundCoroutine = null;
                }
            }
        }
    }

    public bool IsGroundedForJump { get; set; }
    private Coroutine ungroundCoroutine;

    public bool IsWalled { get; set; }
    public bool IsGrabbing { get; set; }
    public bool IgnoreBounce { get; set; }
    public WallDetach WallDetachState { get; set; }
    public bool IsStunned { get; set; }
    public Collider2D WallCollider { get; set; }
    public Vector3 Normal { get; set; }
    public Vector2 ExtraVel { get; set; }
    public Input ForcedInput = null;
    public bool IsLocked { get; set; }
    public int AirCommandCount { get; set; }

    protected void ScaleX()
    {
        var hor = input.horizontalSpeed;
        // Debug.Log(name + "scalex");
        // SCALE
        if (!IsStunned && Mathf.Abs(hor) > Mathf.Epsilon)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * hor > 0 ? 1 : -1,
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    IEnumerator StunCoroutine(float duration)
    {
        IsStunned = true;
        yield return new WaitForSeconds(duration);
        IsStunned = false;
    }

    IEnumerator UngroundDelay()
    {
        yield return new WaitForSeconds(ungroundDelay);
        IsGroundedForJump = false;
    }

    public void Stun(float duration)
    {
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    /* protected IEnumerator SpineOneShot(string now, string next, bool loop)
    {
        skeletonAnimation.state.ClearTrack(0);
        var track = skeletonAnimation.state.SetAnimation(0, now, false);
        yield return new WaitForSpineAnimationComplete(track);
        skeletonAnimation.state.SetAnimation(0, next, loop);
        animationCoroutine = null;
    }

    protected IEnumerator SpineOneShot(string now)
    {
        var loop = skeletonAnimation.state.GetCurrent(0).Loop;
        var next = skeletonAnimation.state.GetCurrent(0).Animation.Name;
        return SpineOneShot(now, next, loop);
    } */
}
