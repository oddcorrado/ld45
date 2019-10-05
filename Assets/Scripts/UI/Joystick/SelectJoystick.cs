using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if REWIRED
using Rewired;
using UnityEngine.Events;

[System.Serializable]
public class SelectEvent : UnityEvent<int, int>
{
}

public class SelectJoystick : MonoBehaviour
{
    [SerializeField] List<GameObject> visualizers;
    [SerializeField] int playerId;
    [SerializeField] SelectEvent selectEvent;
    [SerializeField] Transform visualizerPosition;
    [SerializeField] GameObject selectedFeedback;
    private Player player;
    private int viewedIndex = 0;
    private int selectedIndex = -1;
    public int SelectedIndex { get { return selectedIndex; }}


    IEnumerator Start()
    {
        yield return null;
        while(player == null){
            player = ReInput.players.GetPlayer(playerId - 1);   
            yield return null;
        }
        OnFocus(playerId, viewedIndex);
    }

	public void OnFocus(int playerId, int index)
	{
        viewedIndex = index;
        foreach (var go in visualizers) go.SetActive(false);
        visualizers[viewedIndex].SetActive(true);
        if(selectedFeedback != null)
        {
            if (viewedIndex == selectedIndex) selectedFeedback.SetActive(true);
            else selectedFeedback.SetActive(false);
        }
	}

	void Update()
	{
        if (player != null && player.GetButtonDown("JUMP")) 
        {
            selectedIndex = viewedIndex;
            if(selectedFeedback != null) selectedFeedback.SetActive(true);
            selectEvent.Invoke(playerId, viewedIndex);
        }
	}

    public void AddVisualizer(GameObject visualizer)
    {
        visualizer.transform.parent = transform;
        visualizer.transform.position = visualizerPosition.position;
        visualizers.Add(visualizer);
    }
}
#endif