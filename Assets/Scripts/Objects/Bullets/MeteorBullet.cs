using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if SPINE
[RequireComponent(typeof(Rigidbody2D))]
public class MeteorBullet : SimpleBullet
{
    [SerializeField]
    private PooledBullet blast = null;


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if(blast != null)
        {
            var go = blast.Get<PooledBullet>(true);

            go.transform.position = transform.position;
            go.GetComponent<PlayerObject>().PlayerId = PlayerId;

            go.transform.position = transform.position;
            go.GetComponent<PlayerObject>().PlayerId = PlayerId;
        }
       
    }
}
#endif