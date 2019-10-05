using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCharge : Command
{
    [SerializeField]
    private float speedBoost;
    [SerializeField]
    private float duration;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float pushForce;
    [SerializeField]
    private Vector3 pushVector;
    [SerializeField]
    private ConditionData chargeCondition;


    private PlayerMovementGround playerMovementGround;
    private PlayerCondition playerCondition;
    private Coroutine coroutine;
    bool isCharging = false;
    private float groundSpeed;

    protected new void Start()
    {
        base.Start();
        playerMovementGround = GetComponent<PlayerMovementGround>();
        playerCondition = GetComponent<PlayerCondition>();
        Debug.Assert(playerMovementGround != null, "CommandCharge needs player movement ground");
    }

    IEnumerator ChargeTimeout()
    {
        groundSpeed = playerMovementGround.WalkGroundSpeed;
        GetComponent<AnimationManager>().ContinuousCommand = "charge";
        playerMovementGround.WalkGroundSpeed += speedBoost;
        isCharging = true;
        input.ForceMoveX = true;
        playerCondition.AddCondition(chargeCondition);
        yield return new WaitForSeconds(duration);
        playerCondition.RemoveCondition(chargeCondition);
        EndCoroutine();
    }

    private void EndCoroutine()
    {
        input.ForceMoveX = false;
        playerMovementGround.WalkGroundSpeed = groundSpeed;
        isCharging = false;
        GetComponent<AnimationManager>().ContinuousCommand = null;
        coroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging) return;

        var go = collision?.collider?.gameObject;
        if (go == null) return;

        var life = go.GetComponent<PlayerLife>();
        if (life != null)
        {
            life.Damage(damage, input.PlayerId);

            var pc = collision?.gameObject.GetComponent<PlayerCondition>();
            if (conditionData != null && pc != null)
            {
                pc.AddCondition(conditionData);
            }

            var body = go.GetComponent<Rigidbody2D>();
            if (body != null)
            {

                body.velocity = pushVector + (pushForce * (go.transform.position - transform.position).normalized);
            }
        }
    }

    public override void Stop()
    {
        base.Stop();
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            EndCoroutine();
        }
    }

    void Update()
    {
        if (!Check()) return;
        if (coroutine == null)
        {
            coroutine = StartCoroutine(ChargeTimeout());
        }
    }
}
