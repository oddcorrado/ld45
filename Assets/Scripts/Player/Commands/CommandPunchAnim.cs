using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPunchAnim : CommandPunch
{
    enum ResetGravityMode {NONE, IMPULSE, CONTINUOUS}
    [SerializeField]
    AnimationCurve punchX = new AnimationCurve(new Keyframe[2]{new Keyframe(0, 0), new Keyframe(0.5f, 1)});
    [SerializeField]
    AnimationCurve punchY = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(0.5f, 1)});
    [SerializeField]
    private bool ForcedInput;
    [SerializeField]
    AnimationCurve ForcedInputX = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 3), new Keyframe(0.5f, 0) });
    [SerializeField]
    private ResetGravityMode resetGravity = ResetGravityMode.CONTINUOUS;
    [SerializeField]
    private bool lockMove = true;
    [SerializeField]
    private int maxResetGravity = 1;

    Coroutine coroutine;
    private Rigidbody2D body;

	override protected void Start()
	{
        base.Start();

        body = GetComponent<Rigidbody2D>();
	}

	IEnumerator TimeoutCoroutineEnumerator()
    {
        float startDate = Time.time;
        punch.SetActive(true);
        playerMovement.IsLocked |= lockMove;

        if (resetGravity == ResetGravityMode.IMPULSE && playerMovement.AirCommandCount <= maxResetGravity)
            body.velocity = new Vector2(extraVel.x + body.velocity.x, extraVel.y);
        else
            body.velocity += extraVel;

            
        
        while(Time.time < startDate + duration)
        {
            float date = Time.time - startDate;
            yield return new WaitForSeconds(0.016f);
            float x = punchX.Evaluate(date % punchX.keys[punchX.length - 1].time);
            float y = punchY.Evaluate(date % punchY.keys[punchY.length - 1].time);
            punch.transform.localPosition = new Vector2(x, y);

            if (resetGravity == ResetGravityMode.CONTINUOUS && playerMovement.AirCommandCount <= maxResetGravity)
                body.velocity = new Vector2(body.velocity.x, 0);

            if (ForcedInput)
            {
                float fx = ForcedInputX.Evaluate(date % ForcedInputX.keys[ForcedInputX.length - 1].time);
                fx *= transform.localScale.x;
                playerMovement.ForcedInput = new PlayerMovement.Input()
                {
                    X = fx,
                    Y = 0,
                    J = 0
                };
            }
            else
                playerMovement.ForcedInput = null;
        }
        playerMovement.IsLocked &= !lockMove;
        punch.SetActive(false);
        playerMovement.ForcedInput = null;
        ClearRunningCommand();
        coroutine = null;
    }

    public override void Hit(GameObject go)
    {
        if (!punch.activeSelf) return;
        var life = go.GetComponent<LifeManager>();
        if (life != null)
        {
            life.Damage(damage, input.PlayerId);

            var pc = go.GetComponent<PlayerCondition>();
            if (conditionData != null && pc != null)
            {
                pc.AddCondition(conditionData);
            }

            var otherBody = go.GetComponent<Rigidbody2D>();
            if (otherBody != null)
            {
                otherBody.velocity = (pushForce * (go.transform.position - transform.position).normalized);
            }
        }
    }

    public override void Stop()
    {
        if (coroutine != null)
        {
            punch.SetActive(false);
            StopCoroutine(coroutine);
        }
        playerMovement.ForcedInput = null;
        coroutine = null;
    }

    private void Update()
    {
        if (!Check()) return;

        if(coroutine == null) 
        {
            coroutine = StartCoroutine(TimeoutCoroutineEnumerator());
            playerMovement.AirCommandCount++;
        }
        // StopAllCoroutines();

    }
}
