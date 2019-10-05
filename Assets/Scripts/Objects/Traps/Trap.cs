using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Trap : PlayerObject
{
    [SerializeField]
    private int maxCount = 3;
    [SerializeField]
    private bool dieOnTrigger = false;
    [SerializeField]
    private int enterDamage = 0;
    [SerializeField]
    private int stayDamage = 0;
    [SerializeField]
    private int duration = 0;
    [SerializeField]
    private float activationDelay = 2;
    [SerializeField]
    private ConditionData conditionData;

    // privates
    private Collider2D coll;
    private float endDate;
    private float activationDate;
    private SpriteRenderer sprite;


	void OnEnable()
	{
        sprite = GetComponent<SpriteRenderer>();
        if(duration != 0) endDate = Time.time + duration;
        activationDate = Time.time + activationDelay;
        sprite.color = new Color(1, 1, 1, 0.5f);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
        if (Time.time < activationDate) return;

        var inputRouter = other?.gameObject?.GetComponent<InputRouter>();
        if (inputRouter && inputRouter.PlayerId == PlayerId) return;

        if(enterDamage > 0)
        {
            var life = other?.gameObject?.GetComponent<LifeManager>();

            if (life) life.Damage(enterDamage, PlayerId);
        }

        if(dieOnTrigger)
        {
            gameObject.SetActive(false);
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < activationDate) return;
  
        var inputRouter = collision.collider?.gameObject?.GetComponent<InputRouter>();
        if (inputRouter && inputRouter.PlayerId == PlayerId) return;

        if (enterDamage > 0)
        {
            var life = collision.collider?.gameObject?.GetComponent<LifeManager>();

            var pc = collision.collider?.gameObject?.GetComponent<PlayerCondition>();
            if (conditionData != null && pc != null)
            {
                pc.AddCondition(conditionData);
            }

            if (life) life.Damage(enterDamage, PlayerId);
        }

        if (dieOnTrigger)
        {
            gameObject.SetActive(false);
        }
    }

	private void OnTriggerStay2D(Collider2D collision)
	{
        // FIXME playerid

        if (stayDamage > 0)
        {
            var life = collision?.gameObject?.GetComponent<LifeManager>();
            if (life) life.Damage(stayDamage, PlayerId);
        }
	}

	void Update()
	{
        if (duration != 0 && Time.time > endDate) gameObject.SetActive(false);
        if (sprite.color.a < 0.9f && Time.time > activationDate) sprite.color = new Color(1, 1, 1, 1);
	}
}
