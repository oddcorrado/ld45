using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SPINE
using Spine.Unity;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleBullet : PlayerObject
{
    [SerializeField]
    protected int damage = 1;
    [SerializeField]
    protected bool orient = true;
    [SerializeField]
    protected bool mirror = false;
    [SerializeField]
    protected bool forceVelocity = true;
    [SerializeField]
    protected bool ignorePlayerId = false;
    [SerializeField]
    protected ConditionData conditionData;
    [SerializeField]
    protected int bounce = 0;
    [SerializeField]
    protected bool isInfiniteBounce = false;
    [SerializeField]
    protected float duration = 120;
    [SerializeField]
    protected bool dieOnHit = true;

    protected Rigidbody2D body;
    new protected ParticleSystem particleSystem;
    new protected Collider2D collider;
    SkeletonAnimation skeletonAnimation;
    protected Coroutine dieCoroutine;
    public Vector2 ForceVelocity { get; set; }
    private bool isTrigger;
    protected int bounceCount = 0;
    protected float dieDate;
    private Vector2 preBouncePos;

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        isTrigger = collider.isTrigger;
        particleSystem = transform?.Find("Particles").GetComponent<ParticleSystem>();
        skeletonAnimation = transform?.Find("Spine")?.GetComponent<SkeletonAnimation>();
    }


    protected IEnumerator Die()
    {
        Debug.Log("DIE");
        ForceVelocity = Vector2.zero;
        if (particleSystem != null) particleSystem.Stop();
        collider.isTrigger = true;
        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.SetAnimation(0, "die", false);
            yield return new WaitForSeconds(2);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }

        gameObject.SetActive(false);
        transform.position += new Vector3(1000, 1000, 0);
        if (skeletonAnimation != null) skeletonAnimation.state.SetAnimation(0, "fly", true);
        if (particleSystem != null) particleSystem.Play();
        collider.isTrigger = isTrigger;
        dieCoroutine = null;
    }

    protected virtual void OnEnable()
    {
        if (collider != null) collider.isTrigger = isTrigger;
        if (skeletonAnimation != null) skeletonAnimation.state.SetAnimation(0, "fly", true);
        if (particleSystem != null) particleSystem.Play();
        bounceCount = 0;
        dieDate = Time.time + duration;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collider.isTrigger) return;
        Debug.Log("simple bullet trigger " + name + "-" + collision.gameObject.name + " A");
        if (collision.gameObject.layer == 17) return;

        var po = collision?.gameObject.GetComponent<PlayerObject>();
        if (po != null && po.PlayerId == PlayerId)
        {
            Physics2D.IgnoreCollision(collider, collision);
            return;
        }


        if (dieCoroutine != null) return;

        if (collision.gameObject.layer == 11)
        {
            /* if (PlayerId > po.PlayerId)
            {
                PooledBullet hz = FightManager.Instance.Hotzone.Get<PooledBullet>(true);
                hz.transform.position = transform.position;
            } */
            Physics2D.IgnoreCollision(collider, collision);
            return;
        }

        if (!ignorePlayerId)
        {
            var ir = collision?.gameObject.GetComponent<InputRouter>();
            if (ir != null && ir.PlayerId == PlayerId)
            {
                Physics2D.IgnoreCollision(collider, collision);
                return;
            }

            var pc = collision?.gameObject.GetComponent<PlayerCondition>();
            if (conditionData != null && pc != null)
            {
                pc.AddCondition(conditionData);
                if (dieOnHit) End();
            }

            var life = collision?.gameObject.GetComponent<LifeManager>();
            collider.isTrigger = true;
            if (life != null)
            {
                life.Damage(damage, PlayerId);
                if (dieOnHit) End();
            }
        }

        Debug.Log($"{gameObject.name} TRIG {bounceCount}");
        if (bounceCount >= bounce)
        {
            if (isInfiniteBounce)
            {
                transform.position = preBouncePos;
                ForceVelocity = -ForceVelocity;
                bounceCount = 0;
            }
            else
            {
                // FIXME WHY DID HAVE THIS ???
                if (dieOnHit) End();
            }
        }
        else
        {
            ForceVelocity = body.velocity.normalized * ForceVelocity.magnitude;
            bounceCount++;
            Debug.Log($"BC {bounceCount}");
        }
    }

    protected void End()
    {
        Debug.Log("END");
        if (dieCoroutine == null)
        {
            collider.isTrigger = true;
            dieCoroutine = StartCoroutine(Die());
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("simple bullet collision " + name + "-" + collision.collider.gameObject.name + " A");
        if (collision.gameObject.layer == 17) return;

        var po = collision?.collider?.gameObject.GetComponent<PlayerObject>();
        if (po != null && po.PlayerId == PlayerId)
        {
            Debug.Log(name + "/" + PlayerId + "-" + collision.collider.gameObject.name + "/" + po.PlayerId + " B");
            Physics2D.IgnoreCollision(collider, collision.collider);
            return;
        }

        if (dieCoroutine != null) return;

        /* if (collision.gameObject.layer == 11)
        {
            if (PlayerId > po.PlayerId)
            {
                PooledBullet hz = FightManager.Instance.Hotzone.Get<PooledBullet>(true);
                hz.transform.position = transform.position;
            }
                
        } */

        if (!ignorePlayerId)
        {
            var ir = collision?.collider?.gameObject.GetComponent<InputRouter>();
            if (ir != null && ir.PlayerId == PlayerId)
            {
                // Debug.Log(name + "-" + collision.collider.gameObject.name + " C");
                Physics2D.IgnoreCollision(collider, collision.collider);
                return;
            }

            var life = collision?.collider?.gameObject.GetComponent<LifeManager>();

            if (life != null)
            {
                life.Damage(damage, PlayerId);
                if (dieOnHit) End();
            }


            var pc = collision?.collider?.gameObject.GetComponent<PlayerCondition>();
            if (conditionData != null && pc != null)
            {
                pc.AddCondition(conditionData);
                if (dieOnHit) End();
            }
        }

        Debug.Log($"{gameObject.name} COLL {bounceCount}");
        if (bounceCount >= bounce)
        {
            if (isInfiniteBounce)
            {
                transform.position = preBouncePos;
                ForceVelocity = -ForceVelocity;
                bounceCount = 0;
            }
            else
            {
                // FIXME WHY DID HAVE THIS ???
                if (dieOnHit) End();
            }
        }
        else
        {
            ForceVelocity = body.velocity.normalized * ForceVelocity.magnitude;
            bounceCount++;
        }

    }

    protected virtual void FixedUpdate()
    {
        if (Time.time > dieDate) End();

        if (forceVelocity)
        {
            body.velocity = ForceVelocity;
        }

        if (orient)
        {
            var vel = body.velocity;
            var angle = Mathf.Rad2Deg * Mathf.Atan2(vel.y, vel.x);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        if (mirror)
        {
            if (body.velocity.x < 0)
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
            else
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        }
        preBouncePos = transform.position;
    }
}
#endif