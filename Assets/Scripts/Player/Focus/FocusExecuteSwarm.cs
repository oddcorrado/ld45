using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusExecuteSwarm : FocusExecute
{
    [SerializeField]
    private PooledBullet prefab = null;
    [SerializeField]
    private int count = 10;
    [SerializeField]
    private float period = 0.1f;
    [SerializeField]
    private float xRange = 20;
    [SerializeField]
    private float vxRange = 5;

    IEnumerator SwarmTimer()
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(period);
            var go = prefab.Get<PooledBullet>(true);
            go.transform.position = transform.position + new Vector3(Random.Range(-xRange, xRange), 30);
            go.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-vxRange, vxRange), -10);
            go.GetComponent<PlayerObject>().PlayerId = gameObject.GetComponent<InputRouter>().PlayerId;
        }  
    }

    override public void Execute(Vector2 dir)
    {
        StartCoroutine(SwarmTimer());
    }
}
