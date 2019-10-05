using System.Collections;
using System.Collections.Generic;
using Persistent;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoystickFighterSelectStart : MonoBehaviour
{
    private List<Fighter> fighters = new List<Fighter>();
    [SerializeField] GameObject[] selectors;

	void Start()
	{
        for (int i = 0; i < selectors.Length; i++)
            selectors[i].SetActive(i < PersistentData.playerCount);
	}

	public void Select(Fighter fighter)
	{
        int index = fighters.FindIndex(f => f.playerId == fighter.playerId);
        Debug.Log($"index for {fighter.playerId} is {index}");
        if (index == -1)
            fighters.Add(fighter);
        else
            fighters[index] = fighter;

        if (fighters.Count == PersistentData.playerCount) Play();
                    
	}



	public void Play()
    {
        PersistentData.SelectedFighters.RemoveAll(f => true);
        foreach (var fs in fighters)
        {
            if (fs != null)
            {
                PersistentData.SelectedFighters.Add(new Fighter()
                {
                    playerId = fs.playerId,
                    prefab = fs.prefab,
                    name = fs.prefab.GetComponent<UiData>().title,
                    life = fs.life
                });
                PersistentData.Players.Find(p => p.playerId == fs.playerId).currentFighter = fs;
                Debug.Log($"added player {fs.playerId} {fs.prefab}");
            }
            else
            {
                PersistentData.SelectedFighters.Add(new Fighter()
                {
                    playerId = fs.playerId,
                    prefab = null,
                    name = "none"
                });
                Debug.Log($"added player {fs.playerId} none");
            }

        }

        SceneManager.LoadScene("LevelSelectorJoystick");
    }
}
