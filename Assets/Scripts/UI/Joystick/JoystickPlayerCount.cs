using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Persistent;

public class JoystickPlayerCount : MonoBehaviour
{
    public void OnSelected(int playerId, int index)
    {
        PersistentData.playerCount = index + 2;
        SceneManager.LoadScene("FactionSelectorJoystick");
    }
}
