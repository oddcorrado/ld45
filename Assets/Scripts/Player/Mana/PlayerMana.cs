using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerMana : MonoBehaviour
{
    [SerializeField]
    private bool airLoad = true;
    [SerializeField]
    private float regen = 25;

    private float mana;
    public float Mana
    { 
        get { return mana; }
        set 
        {
            mana = value;
            if (playerStatusDisplay == null)
                UiPlayerFinder.Instance.SetMana(input.PlayerId, (int)value);
            else
                playerStatusDisplay.Mana = (int)value;
            
        } 
    }

    [SerializeField]
    private int manaBase = 100;
    [SerializeField]
    private int startMana = 0;

    private InputRouter input;
    private PlayerMovement playerMovement;
    private PlayerStatusDisplay playerStatusDisplay;

	void Start()
	{
        input = GetComponent<InputRouter>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatusDisplay = transform.Find("StatusCanvas")?.GetComponent<PlayerStatusDisplay>();
        Mana = startMana;
	}

	void Update()
	{
        if (!airLoad && !playerMovement.IsGrounded && !playerMovement.IsWalled) return;
        Mana = Mathf.Min(manaBase, Mana + Time.deltaTime * regen);	
	}
}
