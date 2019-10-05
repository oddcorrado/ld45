using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SPINE
public class HomingBullet : SimpleBullet
{
    [SerializeField]
    private float homingDelay = 0;
    [SerializeField]
    private float rotationSpeed = 0.05f;
    [SerializeField]
    private float velocity = 5;
    [SerializeField]
    private float duration = 5;

    private Transform target;
    private float homingStart;

    public override int PlayerId
    {
        get { return playerId;  }
        set
        {
            homingStart = Time.time;
            playerId = value;
            FindClosestTarget(value);
        }
    }

    void FindClosestTarget(int id)
    {
        target = null;
        foreach (var p in FightManager.Instance.Players)
        {
            if (p != null && p.playerId != id && p.gameObject != null)
            {
                if (target == null)
                    target = p.gameObject.transform;
                else if ((transform.position - target.position).magnitude > (transform.position - p.gameObject.transform.position).magnitude)
                    target = p.gameObject.transform;
            }
        }
    }

    new void FixedUpdate()
    {
        if (target == null) 
        {
            StartCoroutine(Die());
            base.FixedUpdate();
            return;
        }

        if(target != null && Time.time > homingStart + homingDelay && dieCoroutine == null)
        {
            Vector2 homingVector = target.position - transform.position;
            float homingAngle = Mathf.Atan2(homingVector.y, homingVector.x);
            Vector2 bodyVel = body.velocity;
            float velAngle = Mathf.Atan2(bodyVel.y, bodyVel.x);

            var delta = homingAngle - velAngle;
            if (delta > Mathf.PI) delta -= 2 * Mathf.PI; // BEURK
            if (delta < -Mathf.PI) delta += 2 * Mathf.PI; // BEURK

            if (velAngle < homingAngle)
                velAngle += Mathf.Min(delta, rotationSpeed);
            else
                velAngle += Mathf.Max(delta, -rotationSpeed);

            body.velocity = (Quaternion.Euler(0, 0, Mathf.Rad2Deg * velAngle) * Vector2.right) * velocity;
        }

        base.FixedUpdate();
    }

    new void OnCollisionEnter2D(Collision2D collision)
    {
        if(target == null) 
        {
            StartCoroutine(Die());
            return;
        }

        if (dieCoroutine != null) return;

        var po = collision?.collider?.gameObject.GetComponent<PlayerObject>();
        var ir = collision?.collider?.gameObject.GetComponent<InputRouter>();
        if ((po == null || po.PlayerId == PlayerId) && (ir == null || ir.PlayerId == PlayerId)) return;

        var life = collision?.collider?.gameObject.GetComponent<LifeManager>();
        body.velocity = Vector2.zero;
        collider.isTrigger = true;
        if (life != null)
        {
            life.Damage(damage, PlayerId);
        }
        dieCoroutine = StartCoroutine(Die());

    }
}
#endif