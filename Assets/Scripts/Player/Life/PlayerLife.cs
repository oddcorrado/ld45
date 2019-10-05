using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : LifeManager {
    public override int Life
    {
        get { return life; }
        set
        {
            life = value;
            if (playerStatusDisplay != null)
            /* UiPlayerFinder.Instance.SetLife(input.PlayerId, value);
            else */
                playerStatusDisplay.Life = value;
        }
    }

    [SerializeField]
    private int lifeBase = 100;
    [SerializeField]
    private bool isDeactivate = false;
    [SerializeField]
    private PooledBullet pxHit;

    private InputRouter input;
    private PlayerStatusDisplay playerStatusDisplay;
    private TintManager tintManager;

    void Start()
    {
        input = GetComponent<InputRouter>();
        tintManager = GetComponent<TintManager>();
        playerStatusDisplay = transform.Find("StatusCanvas")?.GetComponent<PlayerStatusDisplay>();
        playerStatusDisplay.Life = Life;
        playerStatusDisplay.Player = "P" + input.PlayerId;
        // Life = lifeBase;
    }

    public override void Damage(int value, int playerId)
    {
        if(playerId != input.PlayerId)
        {
            Life -= value;

            PooledBullet bullet = pxHit.Get<PooledBullet>(true);
            var position = transform.position;
            position.z = bullet.transform.position.z;
            bullet.transform.position = position;
            tintManager.Tint(Color.red, 0.5f);
        }
 
        if (Life <= 0)
        {
            if (isDeactivate) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }

    public override void Cure(int value, int playerId)
    {
        if (playerId == input.PlayerId)
            Life += value;
    }
}
