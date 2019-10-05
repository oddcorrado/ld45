using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField]
    int damage = 10;

	private void OnCollisionEnter2D(Collision2D collision)
	{
        collision.gameObject.GetComponent<PlayerLife>()?.Damage(damage, -1);
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
        collision.gameObject.GetComponent<PlayerLife>()?.Damage(damage, -1);
	}
}
