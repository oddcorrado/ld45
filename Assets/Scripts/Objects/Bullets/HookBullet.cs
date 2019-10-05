using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SPINE
public class HookBullet : SimpleBullet
{
    private Transform originator;
    private Rigidbody2D targetBody;
    private Transform targetTransform;
    enum State {FLY, DRAG}
    private State state;
    [SerializeField]
    private float dragSpeed = 10;
    [SerializeField]
    private float dragDuration = 5;
    [SerializeField]
    private PxLine pxLine;

    private AnimationManager animationManager;
    private float endDragDate;

    public override int PlayerId 
    { 
        get { return playerId; } 
        set
        { 
            playerId = value;
            originator = FightManager.Instance.FindPlayer(playerId)?.transform;
            state = State.FLY;
            dieOnHit = false;
        } 
    }

	protected override void OnEnable()
	{
        pxLine.gameObject.SetActive(false);
        endDragDate = Time.time + dragDuration;
        base.OnEnable();
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(state == State.FLY)
        {
            base.OnCollisionEnter2D(collision);

            var ir = collision?.collider?.gameObject.GetComponent<InputRouter>();
            if (ir!= null && ir?.PlayerId != PlayerId)
            {
                Debug.Log("HOOKED " + name + "-" + collision.collider.gameObject.name + " C");
                state = State.DRAG;
                targetTransform = collision.collider.transform;
                targetBody = collision.collider.gameObject.GetComponent<Rigidbody2D>();
                animationManager = collision.collider.gameObject.GetComponent<AnimationManager>();
                animationManager.Handicap = "ledge";
                body.velocity = Vector2.zero;
                pxLine.gameObject.SetActive(true);
            }
        }
    }

	protected override void OnTriggerEnter2D(Collider2D collision)
	{
        if (state == State.FLY)
        {
            base.OnTriggerEnter2D(collision);

            var ir = collision?.gameObject.GetComponent<InputRouter>();
            if (ir != null && ir?.PlayerId != PlayerId)
            {
                Debug.Log("HOOKED " + name + "-" + collision.gameObject.name + " C");
                state = State.DRAG;
                targetTransform = collision.transform;
                targetBody = collision.gameObject.GetComponent<Rigidbody2D>();
                transform.parent = targetTransform;
                if (targetBody == null) End();
                animationManager = collision.gameObject.GetComponent<AnimationManager>();
                if(animationManager !=  null) animationManager.Handicap = "ledge";
                body.velocity = Vector2.zero;
                pxLine.gameObject.SetActive(true);
            }
        }
	}

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (state == State.FLY)
        {
            base.FixedUpdate();
        }
        else
        {
            // Debug.Log($"DRAGGING {(originator.position - targetTransform.position).magnitude}");
            // if(animationManager != null && animationManager.Movement != "ledge") animationManager.Movement = "ledge";
            transform.localPosition = Vector3.zero;
            if ((originator.position - targetTransform.position).magnitude > 1 && Time.time < endDragDate)
            {
                pxLine.StartPos = originator.position;
                pxLine.EndPos = targetTransform.position;
                if(targetBody != null) targetBody.velocity = dragSpeed * (originator.position - targetTransform.position).normalized;
            }
            else
            {
                animationManager.Handicap = null;
                End();
            }
                
        }
    }
}
#endif