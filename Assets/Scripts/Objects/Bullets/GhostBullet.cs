using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SPINE
public class GhostBullet : SimpleBullet
{
    [SerializeField]
    private float duration = 2;
    [SerializeField]
    private float rotSpeed = 0;

    private float endDate;
    private float startdate;

    new protected void OnEnable()
    {
        base.OnEnable();
        endDate = Time.time + duration;
        startdate = Time.time;
        if(collider != null) collider.isTrigger = true;
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if(dieCoroutine == null && Time.time > endDate) dieCoroutine = StartCoroutine(Die());
        if (rotSpeed > 0) transform.rotation = Quaternion.Euler(0, 0, (Time.time - startdate) * rotSpeed);
    }

    new void OnCollisionEnter2D(Collision2D collision)
    {
    }

	protected void OnTriggerEnter2D(Collider2D collider)
	{
        var po = collider?.gameObject.GetComponent<PlayerObject>();
        var ir = collider?.gameObject.GetComponent<InputRouter>();
        if ((po == null || po.PlayerId == PlayerId) && (ir == null || ir.PlayerId == PlayerId)) return;

        var life = collider?.gameObject.GetComponent<LifeManager>();
        if (life != null) life.Damage(damage, PlayerId);
	}
}
#endif