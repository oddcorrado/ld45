using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusDisplay : MonoBehaviour
{
    [SerializeField] private Image lifeFill;
    [SerializeField] private Image manaFill;
    [SerializeField] private GameObject manaMiss;
    [SerializeField] private TMP_Text player;

    public int Life
    {
        set { lifeFill.fillAmount = (float)value / 100; }
    }
       
    public int Mana
    {
        set { manaFill.fillAmount = (float)value / 100; }
    }

    public bool ManaMiss
    {
        set { manaMiss.SetActive(value); }
    }

    public string Player
    {
        set { player.text = value.ToString(); }
    }

	void Update()
	{
        transform.rotation = Quaternion.Euler(0, 0, 0);
	}
}
