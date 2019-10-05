using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Persistent;
using System;

public class JoystickLevelSelector : MonoBehaviour
{
    [Serializable]
    public class Level
    {
        public string displayName;
        public string sceneName;
    }
    [SerializeField]
    private Level[] levels;

	public void OnSelect(int playerId, int index)
	{
        SceneManager.LoadScene(levels[index].sceneName);
	}
}
