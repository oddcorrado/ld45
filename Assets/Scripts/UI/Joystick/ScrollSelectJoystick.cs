using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if REWIRED
using Rewired;

[System.Serializable]
public class FocusEvent : UnityEvent<int, int>
{
}

public class ScrollSelectJoystick : MonoBehaviour
{
    [SerializeField] private int playerId;
    [SerializeField] private Transform content;
    [SerializeField] private FocusEvent onFocus;

    private Player player;
    private int index = 0;
    private List<float> contentPositions = new List<float>();
    private int count;
    private bool positionsUpdated;


    IEnumerator Start()
    {
        yield return null;
        player = ReInput.players.GetPlayer(playerId - 1);
        count = content.childCount;

        for (int i = 0; i < count; i++)
        {
            contentPositions.Add(-content.GetChild(i).localPosition.y);
            Debug.Log(content.GetChild(i).name + " "  + content.GetChild(i).localPosition);
        }
        positionsUpdated = true;
    }

    void Update()
    {
        if (!positionsUpdated) return;

        if (player.GetAxis("VER") > 0.1f && player.GetAxisPrev("VER") < 0.1f)
        {
            if (index == count - 1) content.localPosition += new Vector3(0, 50, 0);
            index = Mathf.Min(index + 1, count - 1);
            onFocus.Invoke(playerId, index);
        }
        if (player.GetAxis("VER") < -0.1f && player.GetAxisPrev("VER") > -0.1f)
        {
            if (index == 0) content.localPosition += new Vector3(0, -50, 0);
            index = Mathf.Max(index - 1, 0);
            onFocus.Invoke(playerId, index);
        }

        content.localPosition += 0.1f * new Vector3(0, contentPositions[index] - content.localPosition.y, 0);
    }

    public void AddItem(GameObject item)
    {
        item.transform.parent = content;
    }
}
#endif