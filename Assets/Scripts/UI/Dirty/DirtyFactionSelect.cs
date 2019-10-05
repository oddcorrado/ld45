using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DirtyFactionSelect : MonoBehaviour
{
    [Serializable]
    public class Faction
    {
        public string name;
        public GameObject[] prefabs;
    }

    [SerializeField]
    private int playerId;
    [SerializeField]
    private Dropdown dropdown;
    [SerializeField]
    private Faction[] factions;

    public int PlayerId { get { return playerId; } }

    public int Index { get; set; }

    public Faction SelectedFaction
    {
        get
        {
            if (Index > 0) return factions[Index - 1];
            return null;
        }    
    }

    public void Select(int index)
    {
        Debug.Log($"select {index}");
        Index = index;
    }

    public void Start()
    {
        var options = new List<string>();

        options.Add("NOT PLAYING");
        foreach (var faction in factions)
        {
            options.Add(faction.name);
        }

        dropdown.AddOptions(options);
    }
}
