using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPunch : Command {
    [SerializeField]
    protected GameObject punch;
    [SerializeField]
    protected float duration = 0.1f;
    [SerializeField]
    protected int damage = 1;
    [SerializeField]
    protected float pushForce = 10;
    [SerializeField]
    protected Vector2 extraVel = new Vector2(0, 0);

    Coroutine coroutine;

	IEnumerator punchTimeout()
    {
        punch.SetActive(true);
        yield return new WaitForSeconds(duration);
        punch.SetActive(false);
        ClearRunningCommand();
        coroutine = null;
    }

	public virtual void Hit(GameObject go)
	{
        var life = go.GetComponent<LifeManager>();
        if (life != null)
        {
            life.Damage(damage, input.PlayerId);

            var pc = go.GetComponent<PlayerCondition>();
            if (conditionData != null && pc != null)
            {
                pc.AddCondition(conditionData);
            }

            var body = go.GetComponent<Rigidbody2D>();
            if(body != null) {
                
                body.velocity = (pushForce * (go.transform.position - transform.position).normalized);
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
        coroutine = null;
    }

	private void Update()
	{
        if (!Check()) return;

        if(coroutine == null)
            coroutine = StartCoroutine(punchTimeout());
	}
}
