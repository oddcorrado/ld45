using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
    [SerializeField]
    private PooledBullet prefab;
    [SerializeField]
    private float cooldown = 0.3f;

    private float dateOk = 0;
    private int cnt = 0;
    private InputRouter input;
    void Start()
	{
        input = GetComponent<InputRouter>();
	}

	void Update () 
    {
        if(Time.time > dateOk)
        {
            if(input.A) 
            {
                PooledBullet bullet = prefab.Get<PooledBullet>(true);
                bullet.transform.position += transform.position + Vector3.up;
                bullet.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * 10;
                bullet.name = "b" + cnt++;
                dateOk += cooldown;
            }
        }
	}
}
