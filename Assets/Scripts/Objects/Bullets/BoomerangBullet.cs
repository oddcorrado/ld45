using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SPINE
public class BoomerangBullet : SimpleBullet
{
    [SerializeField]
    private float returnDelay = 2;
    [SerializeField]
    private float velocity = 5;

    private Transform player;
    private float returnStart;

    public override int PlayerId
    {
        get { return playerId; }
        set
        {
            returnStart = Time.time + returnDelay;
            playerId = value;
            FindPlayer(value);
        }
    }

    void FindPlayer(int id)
    {
        
        player = null;
        foreach (var p in FightManager.Instance.Players)
        {
            if (p != null && p.playerId == id && p.gameObject != null)
            {
                player = p.gameObject.transform;
            }
        }
        Debug.Log($"find player id {id} {player}");
    }

    new void FixedUpdate()
    {
        if (player != null)
        {
            if (Time.time > returnStart && dieCoroutine == null)
            {
                ForceVelocity = (player.position - transform.position).normalized * velocity;
                body.velocity = ForceVelocity;
                if ((player.position - transform.position).magnitude < 1)
                   dieCoroutine = StartCoroutine(Die());
                else if (Time.time > returnStart + 3 * returnDelay)
                   dieCoroutine = StartCoroutine(Die());
            }
        }

        base.FixedUpdate();
    }
}
#endif