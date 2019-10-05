using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerCondition))]
public class CommandDiveBlast : Command
{
    [SerializeField]
    private PooledBullet prefab;
    [SerializeField]
    private float velocity = 10;
    [SerializeField]
    private float lockDuration = 3;

    private float minDuration = 0.06f;

    private PlayerCondition playerCondition;
    private Rigidbody2D body;
    private bool isWaitingCrash = false;
    private float startDate;
    private AnimationManager animationManager;

    new void Start()
	{
        base.Start();
        playerCondition = GetComponent<PlayerCondition>();
        body = GetComponent<Rigidbody2D>();
        animationManager = GetComponent<AnimationManager>();
	}

    IEnumerator Locker()
    {
        playerMovement.IsLocked = true;
        yield return new WaitForSeconds(lockDuration);
        playerMovement.IsLocked = false;
        ClearRunningCommand();
        playerMovement.IgnoreBounce = false;
    }

	private void Update()
    {
        if (!Check()) return;

        playerMovement.IgnoreBounce = true;
        isWaitingCrash = true;
        body.velocity = new Vector3(0, -velocity, 0);
        startDate = Time.time;
        // FIXME
        /* playerCondition.AddCondition(new PlayerCondition.Condition()
        {
            originator = gameObject,
            handicap = PlayerCondition.Handicap.MOVE,
            type = PlayerCondition.Type.DIVE,
            endDate = Time.time + 10,
            priority = 10,
            retrigger = PlayerCondition.Retrigger.IGNORE
        }); */

    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if(isWaitingCrash)
        {
            isWaitingCrash = false;
            if(Time.time < startDate + minDuration) 
            {
                animationManager.Command = "jump";
                playerMana.Mana += cost * 0.5f;
                ClearRunningCommand();
                return;
            }
            StartCoroutine(Locker());
            /* playerCondition.AddCondition(new PlayerCondition.Condition()
            {
                originator = gameObject,
                handicap = PlayerCondition.Handicap.MOVE,
                type = PlayerCondition.Type.DIVERECOVER,
                endDate = Time.time + 2,
                cancels = new PlayerCondition.Type[] {PlayerCondition.Type.DIVE},
                priority = 10,
                retrigger = PlayerCondition.Retrigger.IGNORE
            }); */
            PooledBullet blast = prefab.Get<PooledBullet>(true);

            blast.transform.position = transform.position;
            body.velocity = Vector3.zero;
            blast.GetComponent<PlayerObject>().PlayerId = input.PlayerId;
            animationManager.Command = postAnimation;
        }
	}
}