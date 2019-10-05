using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTeleport : Command
{
    [SerializeField]
    private float distance = 3;
    [SerializeField]
    private PooledBullet prefab;

    void Update()
    {
        if (!Check()) return;

        var dir = new Vector3(input.X, input.Y, 0).normalized;
        var position = transform.position + dir * distance;

        position.x = Mathf.Clamp(position.x, FightManager.Instance.Bounds.xMin + 1, FightManager.Instance.Bounds.xMax - 1);
        position.y = Mathf.Clamp(position.y, FightManager.Instance.Bounds.yMin + 1, FightManager.Instance.Bounds.yMax - 1);

        var go = prefab.Get<PooledBullet>(false);
        var pxline = go.GetComponent<PxLine>();
        pxline.StartPos = transform.position;
        pxline.EndPos = position;
        go.gameObject.SetActive(true);
        transform.position = position;

       
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;


    }
}
