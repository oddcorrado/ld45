using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirtyFighterSelect : MonoBehaviour
{
    [SerializeField]
    private int playerId;
    [SerializeField]
    private Dropdown dropdown;
    [SerializeField]
    private DirtyPlay dirtyPlay;

    public int PlayerId { get { return playerId; }}

    public int Index { get; set; }

	public void Select(int index)
	{
        Debug.Log(index);
        Index = index;
	}

	public void Start()
	{
        var options = new List<string>();

        foreach (var p in dirtyPlay.Prefabs)
        {
            if (p != null)
                options.Add(p.GetComponent<UiData>().title);
            else
                options.Add("NOT PLAYING");
        }
            
        dropdown.AddOptions(options);
	}
}
