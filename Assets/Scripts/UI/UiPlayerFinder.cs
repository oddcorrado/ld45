using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayerFinder : MonoBehaviour {
    static private UiPlayerFinder instance;
    static public UiPlayerFinder Instance
    { 
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UiPlayerFinder>();
                if(instance == null)
                {
                    var go = new GameObject();
                    go.AddComponent<UiPlayerFinder>();
                    instance = go.GetComponent<UiPlayerFinder>();
                    instance.Setup();
                }
            }
            return instance;
        }
    }
    protected class PlayerDisplay
    {
        public Text life;
        public Text mana;
        public Image lifeBar;
        public Image manaBar;
    }

    private List<PlayerDisplay> playerDisplays = new List<PlayerDisplay>();

    void Setup()
    {
        int count = 2;
        for (int i = 1; i <= count; i++)
        {
            var life = GameObject.Find("Player" + i + "Life")?.GetComponent<Text>();
            var lifeBar = GameObject.Find("Player" + i + "LifeBar")?.GetComponent<Image>();
            var manaBar = GameObject.Find("Player" + i + "ManaBar")?.GetComponent<Image>();
            var mana = GameObject.Find("Player" + i + "Mana")?.GetComponent<Text>();
            if(life != null && mana != null) playerDisplays.Add(new PlayerDisplay() { life = life, mana = mana, lifeBar = lifeBar, manaBar = manaBar });
        }
    }

    public void SetMana(int playerId, int mana)
    {
        playerDisplays[playerId - 1].mana.text = (mana >= 0 ? mana.ToString() : "DEAD");
        playerDisplays[playerId - 1].manaBar.fillAmount = Mathf.Max(0, (float)mana / 100);
    }

    public void SetLife(int playerId, int life)
    {
        playerDisplays[playerId - 1].life.text = (life >= 0 ? life.ToString() : "DEAD");
        playerDisplays[playerId - 1].lifeBar.fillAmount = Mathf.Max(0, (float)life / 100);

    }
}
