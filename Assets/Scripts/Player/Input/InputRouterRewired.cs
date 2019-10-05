using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if REWIRED
using Rewired;

public class InputRouterRewired : MonoBehaviour
{
    public enum Key { A, B, C, J, UP, DOWN, LEFT, RIGHT, HORIZONTAL, DIR, NEUTRAL }
    public float X { get; set; }
    public float Y { get; set; }
    public bool A { get; set; }
    public bool B { get; set; }
    public bool C { get; set; }
    public bool J { get; set; }
    public bool ForceMoveX { get; set; }
    [SerializeField]
    private int playerId;   
    public int PlayerId { get { return playerId; } set { playerId = value; } }
    private float prevX;
    public bool NeedsReset { get; set; }

    private Player player; // The Rewired Player

    void Start()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId - 1);
    }

    public bool Check(Key[] keys)
    {
        
        if (NeedsReset) {
            if (!A && !B && !C && Mathf.Abs(Y) < 0.01f && Mathf.Abs(X) < 0.01f)
                NeedsReset = false;
            else
                return false; 
        }

        foreach (var key in keys)
        {
            switch (key)
            {
                case Key.A:
                    if (!A) return false;
                    break;
                case Key.B:
                    if (!B) return false;
                    break;
                case Key.C:
                    if (!C) return false;
                    break;
                case Key.J:
                    if (!A) return false;
                    break;
                case Key.UP:
                    if (Y <= 0.9f) return false;
                    break;
                case Key.DOWN:
                    if (Y >= -0.9f) return false;
                    break;
                case Key.LEFT:
                    if (X >= -0.9f) return false;
                    break;
                case Key.RIGHT:
                    if (X <= 0.9f) return false;
                    break;
                case Key.HORIZONTAL:
                    if (Mathf.Abs(X) <= 0.5f) return false;
                    break;
                case Key.NEUTRAL:
                    if (Mathf.Abs(X) >= 0.5f || Mathf.Abs(Y) >= 0.5f) return false;
                    break;
                case Key.DIR:
                    if (Mathf.Abs(X) <= 0.5f && Mathf.Abs(Y) <= 0.5f) return false;
                    break;
                default:
                    return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        X = player.GetAxis("HOR"); // get input by name or action id
        Y = player.GetAxis("VER");
        A = player.GetButton("A");
        B = player.GetButton("B");
        C = player.GetButton("C");
        J = player.GetButton("JUMP");

        // X = Input.GetAxis("J" + PlayerId + "H") + Input.GetAxis("K" + PlayerId + "H");
        if (ForceMoveX && Mathf.Abs(X) < Mathf.Epsilon) X = prevX;
        prevX = X;
        // Debug.Log(X + "/" + Y + "/" + A + "/" + B + "/" + C + "/");
        // Debug.Log(ReInput.players.GetPlayer(0).GetButton("JUMP") + " " + ReInput.players.GetPlayer(1).GetButton("JUMP"));
    }
}
#endif