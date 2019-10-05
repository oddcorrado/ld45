using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour {
    protected int playerId = 0;
    public virtual int PlayerId { get { return playerId; } set { playerId = value; } }
}
