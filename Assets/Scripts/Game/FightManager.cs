using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Persistent;
using UnityEngine.SceneManagement;
using TMPro;

public class FightManager : MonoBehaviour {
    static private FightManager instance;
    static public FightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FightManager>();
                Debug.Assert(instance != null, "cannot find fight manager");
            }
            return instance;
        }
    }

    [System.Serializable]
    public class Player
    {
        public int playerId;
        public GameObject prefab;
        public GameObject gameObject;
    }
    [SerializeField]
    private Player[] players;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private Rect bounds;
    public Rect Bounds { get { return bounds; }}
    [SerializeField]
    private PooledBullet hotzone;
    public PooledBullet Hotzone { get { return hotzone; }}
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TMP_Text winnerText;

    public Player[] Players { get { return players; }}
    private bool gameOver = false;

	void Start ()
    {
        if(PersistentData.SelectedFighters.Count == 0)
            UseEditorFighters();

        players = new Player[PersistentData.SelectedFighters.Count];
        for (int i = 0; i < PersistentData.SelectedFighters.Count; i++)
        {
            var f = PersistentData.SelectedFighters[i];
            if(f.prefab != null)
            {
                var go = Instantiate(f.prefab);
                go.transform.position = spawnPoints[i].position;
                go.GetComponent<InputRouter>().PlayerId = f.playerId;
                if (f.life != -1) go.GetComponent<PlayerLife>().Life = f.life;
                players[i] = new Player()
                {
                    playerId = f.playerId,
                    prefab = f.prefab,
                    gameObject = go
                };
                // TODO spawn point
            }
        }
        Debug.Log($"players length: {players.Length}");
	}
	
    void UseEditorFighters()
    {
        Debug.Log("use editor fighters");
        PersistentData.SelectedFighters.RemoveAll(f => true);
        for (int i = 0; i < players.Length; i++)
        {
            var p = players[i];
            var f = new Fighter()
            {
                name = p.prefab.name,
                prefab = p.prefab,
                playerId = p.playerId
            };
            PersistentData.SelectedFighters.Add(f);
        }
    }
	void Update () 
    {
        int living = players.Length;
        int winnerId = 0;
        int winnerLife = 0;

        if (gameOver) return;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null || players[i].gameObject == null)
                living--;
            else
            {
                winnerId = players[i].playerId; // i + 1;
                winnerLife = players[i].gameObject.GetComponent<PlayerLife>().Life;
            }
        }
            
        if (living == 1)
        {
            Debug.Log("winner is player " + winnerId);
            PersistentData.Players.ForEach(player =>
            {
                if (winnerId != player.playerId)
                {
                    if(player.currentFighter != null)
                    {
                        player.currentFighter.alive = false;
                        player.currentFighter.life = 0;
                    }

                }
                else
                {
                    player.currentFighter.life = winnerLife;
                }
            });
            gameOver = true;
            StartCoroutine(WinnerPanelCoroutine(winnerId));
        }
	}

    IEnumerator WinnerPanelCoroutine(int winnerId)
    {
        string[] winnerString = new string[] { "one", "two", "three", "four", "five", "six" };

        string text = $"Player {winnerString[winnerId - 1]} wins";
        winnerPanel.SetActive(true);
        winnerText.text = text;
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("FighterSelectorJoystick");
    }

    public GameObject FindPlayer(int id)
    {
        GameObject go = null;

        foreach(var p in players)
            if (p.playerId == id)
                go = p.gameObject;

        return go;
    }
}
