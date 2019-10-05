using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotzone : MonoBehaviour
{
    [SerializeField]
    private float duration = 1;

    private float endDate;

	void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.layer == 11) collision.gameObject.SetActive(false);
        endDate = Time.time + duration;
	}

    void OnEnable()
	{
        endDate = Time.time + duration;
	}

	void Update()
	{
        if (Time.time > endDate) gameObject.SetActive(false);
	}
}
