using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Px : MonoBehaviour
{
    [SerializeField]
    private float duration = 2;

    private float startDate;
    new private ParticleSystem particleSystem;
    private Coroutine timer;

    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(duration);
        particleSystem.Stop();
        gameObject.SetActive(false);
    }

	void OnEnable()
	{
        if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();
        timer = StartCoroutine(TimerCoroutine());
	}
}
