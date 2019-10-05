using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTriggerSpawn : TrapTrigger
{
    [SerializeField]
    private PooledBullet obj;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private bool destroyAfterSpawn = true;

    public override void Trigger()
    {
        Debug.Log("TRIGGER SPAWN");

        var go = obj.Get<PooledBullet>();
        go.transform.position = spawnPoint.position;
        if (destroyAfterSpawn) gameObject.SetActive(false);
    }
}
