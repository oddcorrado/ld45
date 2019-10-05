using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Persistent;

public class DirtyPlay : MonoBehaviour
{
    [SerializeField]
    private DirtyFactionFighterSelect[] factionFighterSelects;
    [SerializeField]
    private string[] levels;
    [SerializeField]
    private GameObject[] prefabs;
    public GameObject[] Prefabs 
    {
        get { return prefabs; }
    }

    public void Play()
    {
        PersistentData.SelectedFighters.RemoveAll(f => true);
        foreach(var fs in factionFighterSelects)
        {
            if(fs.SelectedFighter != null)
            {
                PersistentData.SelectedFighters.Add(new Fighter()
                {
                    playerId = fs.PlayerId,
                    prefab = fs.SelectedFighter.prefab,
                    name = fs.SelectedFighter.prefab.GetComponent<UiData>().title
                });
                PersistentData.Players.Find(p => p.playerId == fs.PlayerId).currentFighter = fs.SelectedFighter;
            }
            else
            {
                PersistentData.SelectedFighters.Add(new Fighter()
                {
                    playerId = fs.PlayerId,
                    prefab = null,
                    name = "none"
                });
            }

        }
        PersistentData.SelectedFighters.ForEach(sf =>
        {
            // Debug.Log(sf.playerId + " " + sf.prefab + " " + sf.name);
        });
        string levelName = levels[PersistentData.Level];
        SceneManager.LoadScene(levelName);
    }
}
