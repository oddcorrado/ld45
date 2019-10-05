using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CureBullet : PlayerObject
{
    [SerializeField]
    private int cure = 1;

    protected Rigidbody2D body;

    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        var pl = collision?.collider?.gameObject.GetComponent<PlayerLife>();
        var pi = collision?.collider?.gameObject.GetComponent<InputRouter>();

        if (pl != null && pi != null && pi.PlayerId == PlayerId)
        {
            pl.Cure(cure, PlayerId);
        }

        // var po = collision?.collider?.gameObject.GetComponent<PlayerObject>();
        // if (po == null || po.PlayerId != PlayerId)
            gameObject.SetActive(false);
    }

    protected void Update()
    {
        var vel = body.velocity;
        var angle = Mathf.Rad2Deg * Mathf.Atan2(vel.y, vel.x);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
