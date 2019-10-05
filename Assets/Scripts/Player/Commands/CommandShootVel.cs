using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputRouter))]
public class CommandShootVel : Command
{
    [Serializable]
    public class AngleRange
    {
        public float min;
        public float max;
    }
    [SerializeField]
    private PooledBullet prefab;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private AngleRange[] angleRanges;

    private Rigidbody2D body;
    private Vector2 dir;

    new void Start()
    {
        base.Start();

        foreach (var range in angleRanges)
        {
            if (range.min > range.max
                || Mathf.Abs(range.min) > 360
                || range.min < 0
                || Mathf.Abs(range.max) > 360
                || range.max < 0)
                Debug.LogError("unvalid range " + range.min + "/" + range.max);
        }
    }

    private Vector2 RestrainAngle()
    {
        if (angleRanges.Length == 0)
            return dir;


        float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        if (angle < 0) angle += 360;
        float best = 2000;

        foreach (var range in angleRanges)
        {
            if (angle >= range.min && angle <= range.max) return dir;
            float clamp = Mathf.Clamp(angle, range.min, range.max);

            if (Mathf.Abs(angle - clamp) < Mathf.Abs(angle - best)) best = clamp;
        }

        return Quaternion.Euler(0, 0, best) * Vector2.right;
    }

    void Place(PooledBullet bullet)
    {
        bullet.transform.position = transform.position + distance * new Vector3(dir.x, dir.y, 0).normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = dir * velocity;
    }

    void Update()
    {
        var newDir = new Vector2(body.velocity.x, body.velocity.y).normalized;
        if (newDir.magnitude > Mathf.Epsilon) dir = newDir;

        if (!Check()) return;

        PooledBullet bullet = prefab.Get<PooledBullet>(true);

        dir = RestrainAngle();
        Place(bullet);
    }
}
