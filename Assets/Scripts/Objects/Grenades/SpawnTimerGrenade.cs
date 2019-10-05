using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTimerGrenade : MonoBehaviour
{
    [SerializeField]
    private float duration = 3;
    [SerializeField]
    private PooledBullet spawnObject;


    IEnumerator OnEnableTimer()
    {
        yield return new WaitForSeconds(duration);

        var so = spawnObject.Get<PooledBullet>(true);
        so.transform.position = transform.position;
        gameObject.SetActive(false);
    }

	void OnEnable()
	{
        StartCoroutine(OnEnableTimer());
	}
}
