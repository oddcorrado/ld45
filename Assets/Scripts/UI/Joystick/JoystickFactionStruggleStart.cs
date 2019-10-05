using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Persistent;
using System;

public class JoystickFactionStruggleStart : MonoBehaviour
{
    [SerializeField] private Faction[] factions;
    [SerializeField] private int playerCount = 2;
    [SerializeField] GameObject[] selectors;

    public int PlayerCount {
        get { return playerCount; }
        set 
        {
            playerCount = Mathf.Clamp(value, 2, 4);
            for (int i = 0; i < selectors.Length; i++) selectors[i].SetActive(i < playerCount);
        }
    }

    private List<Selection> selections = new List<Selection>();

    [Serializable]
    public class Faction
    {
        public string name;
        public GameObject[] prefabs;
    }


    private class Selection
    {
        public int playerId;
        public int index;
    }

	void Start()
	{
        PlayerCount = PersistentData.playerCount;
	}

	public void OnSelected(int playerId, int index)
	{
        var selection = selections.Find(s => s.playerId == playerId);

        if(selection == null)
        {
            selection = new Selection() { playerId = playerId, index = index };
            selections.Add(selection);
        }
        else
        {
            selection.playerId = playerId;
            selection.index = index;
        }

        if (selections.Count == playerCount) Play();
	}

	public void Play()
    {
        Debug.Log("PLAY");
        selections.ForEach(selection =>
        {
            Debug.Log($"SELECTED {selection.index}");
            Faction faction = factions[selection.index];
            if (faction!= null)
            {
                var playerId = selection.playerId;
                PersistentData.Player player = new PersistentData.Player()
                {
                    playerId = playerId,
                    fighters = new List<Fighter>(),
                    name = $"PLAYER {playerId}"
                };
                foreach (var p in faction.prefabs)
                {
                    Debug.Log($"name: {p.name}");
                    Debug.Log($"uidata: {p.GetComponent<UiData>()}");
                    player.fighters.Add(new Fighter()
                    {
                        life = 100,
                        alive = true,
                        name = p.GetComponent<UiData>().title,
                        playerId = playerId,
                        prefab = p
                    });
                }
                PersistentData.Players.Add(player);
            }
        });
        PersistentData.playerCount = PlayerCount;
        PersistentData.Players.ForEach(p =>
        {
            Debug.Log($"NAME {p.name}/{p.playerId}");
            p.fighters.ForEach(f => Debug.Log($"fighter {f.name} {f.alive} {f.life} {f.prefab}"));
        });
        SceneManager.LoadScene("FighterSelectorJoystick");
    }
}
