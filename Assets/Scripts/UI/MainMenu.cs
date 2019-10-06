﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Pause();
        }        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        transform.Find("MenuPause").gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        transform.Find("MenuPause").gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        transform.Find("GameOver").gameObject.SetActive(true);
    }
}
