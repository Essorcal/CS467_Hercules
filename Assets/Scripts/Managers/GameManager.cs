﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;

public class GameManager : Manager<GameManager>
{
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        POSTGAME
    }

    public static GameManager instance;
    public CharacterStats[] playerStats;


    void Start()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
    
}
 