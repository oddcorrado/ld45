using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapWaspBomb : MonoBehaviour
{
    [SerializeField]
    private float yRange = 10;
    [SerializeField]
    private float yDropSpeed = 0.01f;

    private float topY = 10;
    private float bottomY = 0;
    new private Rigidbody2D rigidbody2D;

	void Start()
	{
        topY = FightManager.Instance.Bounds.yMax;
        bottomY = FightManager.Instance.Bounds.yMin;
        rigidbody2D = GetComponent<Rigidbody2D>();
	}

	void OnEnable()
	{
//        rigidbody2D.bodyType = RigidbodyType2D.Static;
	}

	void Update()
    {
        if (transform.position.y < topY - yRange)
        {
          //  rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
          //  rigidbody2D.velocity = new Vector2(0, yDropSpeed);
        }
        if (transform.position.y < bottomY - yRange) gameObject.SetActive(false);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        var inputRouter = collision.collider?.gameObject?.GetComponent<InputRouter>();
        if (inputRouter && inputRouter.PlayerId == GetComponent<PlayerObject>().PlayerId)
        {
            Physics2D.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider2D>());
            return;
        }

        gameObject.SetActive(false);
	}
}
