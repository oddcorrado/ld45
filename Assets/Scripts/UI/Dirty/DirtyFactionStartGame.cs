using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Persistent;

public class DirtyFactionStartGame : MonoBehaviour
{
    [SerializeField]
    private DirtyFactionSelect[] factionSelects;


    public void Play()
    {
        Debug.Log("PLAY");
        foreach (var fs in factionSelects)
        {
            Debug.Log($"SELECTED {fs.SelectedFaction}");
            if(fs.SelectedFaction != null)
            {
                var playerId = fs.PlayerId;
                PersistentData.Player player = new PersistentData.Player()
                {
                    playerId = playerId,
                    fighters = new List<Fighter>(),
                    name = $"PLAYER {playerId}"
                };
                foreach (var p in fs.SelectedFaction.prefabs)
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
        }
        PersistentData.Players.ForEach(p =>
        {
            Debug.Log($"NAME {p.name}/{p.playerId}");
            p.fighters.ForEach(f => Debug.Log($"fighter {f.name} {f.alive} {f.life} {f.prefab}"));
        });
        SceneManager.LoadScene("FactionFighterSelector");
    }
}