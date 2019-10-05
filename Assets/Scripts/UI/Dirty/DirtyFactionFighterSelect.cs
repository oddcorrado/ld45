using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Persistent;

public class DirtyFactionFighterSelect : MonoBehaviour
{
    [SerializeField]
    private int playerId;
    [SerializeField]
    private Dropdown dropdown;

    private List<Fighter> fighters = new List<Fighter>();

    public int PlayerId { get { return playerId; } }

    public Fighter SelectedFighter
    {
        get
        {
            if (fighters.Count == 0) return null;
            return fighters[Index];
        }
    }

    public int Index { get; set; }

    public void Select(int i)
    {
        
        Index = i;
        Debug.Log(PlayerId + $" {Index} " + fighters[Index]);
    }

    public void Start()
    {
        var options = new List<string>();

        PersistentData.Players.ForEach(p =>
        {
            Debug.Log($"NAME {p.name}/{p.playerId}");
            p.fighters.ForEach(f => Debug.Log($"fighter {f.name} {f.alive} {f.life} {f.prefab}"));
        });

        var player = PersistentData.Players.Find(p => p.playerId == playerId);

        if (player == null) {
            options.Add("NOT PLAYING");
            return;
        }

        player.fighters.ForEach(f =>
        {
            if (f != null && f.life > 0 && f.prefab != null && f.alive)
            {
                options.Add(f.prefab.GetComponent<UiData>().title);
                fighters.Add(f);
            }
        });

        dropdown.AddOptions(options);
    }
}
