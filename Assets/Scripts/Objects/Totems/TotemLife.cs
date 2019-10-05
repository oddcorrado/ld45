using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TotemLife : LifeManager
{
    public override int Life
    {
        get { return life; }
        set
        {
            life = value;
        }
    }

    [SerializeField]
    private int lifeBase = 10;
    [SerializeField]
    private PooledBullet pxHit;
    [SerializeField]
    private Image fill;
    [SerializeField]
    private Image fillBg;
    [SerializeField]
    TMP_Text player;

    private PlayerObject playerObject;

    void Start()
    {
        playerObject = GetComponent<PlayerObject>();
        Life = lifeBase;
        player.text = "P" + GetComponent<Totem>().PlayerId;
    }

    void OnEnable()
    {
        Life = lifeBase;
        if (fillBg != null) fillBg.transform.localScale = new Vector2((float)Life / 80, fillBg.transform.localScale.y);
        if (fill != null)
        {
            fill.transform.localScale = new Vector2((float)Life / 80, fill.transform.localScale.y);
            fill.fillAmount = Life / lifeBase;
        }

    }

    public override void Damage(int value, int playerId)
    {
        if (playerId != playerObject.PlayerId)
            Life -= value;

        PooledBullet bullet = pxHit.Get<PooledBullet>(true);
        var position = transform.position;
        position.z = bullet.transform.position.z;
        bullet.transform.position = position;
        if (fill != null) fill.fillAmount = (float)Life / (float)lifeBase;
        

        if (Life <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public override void Cure(int value, int playerId)
    {
        if (playerId == playerObject.PlayerId)
            Life += value;
    }
}
