using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Persistent;
using UnityEngine.UI;
#if REWIRED
public class JoystickFactionStruggleFighterFiller : MonoBehaviour
{
    [SerializeField]
    private int playerId;
    [SerializeField]
    private ScrollSelectJoystick scrollSelector;
    [SerializeField]
    private SelectJoystick selector;
    [SerializeField]
    private GameObject scrollItemPrefab;
    [SerializeField]
    private GameObject selectItemPrefab;
    [SerializeField]
    private JoystickFighterSelectStart joystickFighterSelectStart;

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

    public void Select(int playerId, int i)
    {

        Index = i;
        Debug.Log(PlayerId + $" {Index} " + fighters[Index]);
        joystickFighterSelectStart.Select(fighters[Index]);
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

        if (player == null)
        {
            options.Add("NOT PLAYING");
            return;
        }

        player.fighters.ForEach(f =>
        {
            if (f != null && f.life > 0 && f.prefab != null && f.alive)
            {
                f.playerId = playerId;
                fighters.Add(f);

                var item = Instantiate(scrollItemPrefab);
                item.transform.Find("Text").GetComponent<Text>().text = f.prefab.GetComponent<UiData>().title;
                scrollSelector.AddItem(item);

                var selectItem = Instantiate(selectItemPrefab);
                selectItem.GetComponent<Image>().sprite = f.prefab.GetComponent<UiData>().badge;
                selector.AddVisualizer(selectItem);
            }
        });
    }
}
#endif