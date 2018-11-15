﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public void SaveGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameController.control.Save();
    }

    public void LoadGame()
    {
        GameController.control.Load();
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    public void Options()
    {
               
    }

    public void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();

    }
}
