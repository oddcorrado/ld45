using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField]
    Vector2 direction;

	void OnTriggerEnter2D(Collider2D collision)
	{
        var pl = collision.gameObject.GetComponent<PlayerMovement>();
        if (pl != null && pl.IgnoreBounce) return;
        var body = collision.gameObject?.GetComponent<Rigidbody2D>();
        if (body) body.velocity = direction;	
	}
}
