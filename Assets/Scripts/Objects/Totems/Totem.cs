using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : PlayerObject
{
    [SerializeField]
    private int maxCount = 3;
    [SerializeField]
    private float cooldown = 3;
    [SerializeField]
    private PooledBullet prefab;
    [SerializeField]
    private float velocity = 1;
    [SerializeField]
    private bool selfTarget = true;
    [SerializeField]
    private Transform shootTransform = null;

    // privates
    private Collider2D coll;
    private float nextSpawnDate = 0;

    void Update()
    {
        if (prefab == null) return;
        if (nextSpawnDate < Time.time)
        {
            GameObject target = null;
            foreach (var p in FightManager.Instance.Players) // FIXME use LINQ
            {
                if (p != null)
                {
                    if ((selfTarget && p.playerId == PlayerId) || (!selfTarget && p.playerId != PlayerId))
                    {
                        target = p.gameObject;
                        break;
                    }
                }
            }

            if (target == null) return;

            PooledBullet go = prefab.Get<PooledBullet>(true);
            go.transform.position = shootTransform.position;
            go.GetComponent<PlayerObject>().PlayerId = PlayerId;
            go.GetComponent<Rigidbody2D>().velocity = velocity * (target.transform.position - shootTransform.position).normalized;
#if SPINE
            go.GetComponent<SimpleBullet>().ForceVelocity = velocity * (target.transform.position - shootTransform.position).normalized;
#endif
            nextSpawnDate = Time.time + cooldown;
        }
    }
}
