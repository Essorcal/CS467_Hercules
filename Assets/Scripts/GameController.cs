using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour {

    public static GameController control;
    public GameObject EnemyController, player, playerController;

    //Player Stats
    [Header("Player Stats")]
    public float playerHealth;
    public float playerSanity;
    public float characterSelect;
    public float gemsCollected;
    public float keysCollected;

    //Game Stats
    [Header("Game Stats")]
    public float voidPortalStatus;
    public float plasmaPortalStatus;
    public float twilightPortalStatus;
    public float gameTime;
    public float musicVol;
    public float soundFXVol;

    //Enemy Stats
    [Header("Enemy Stats")]
    public float goblinHealth;
    public float bossHealth;
    public float voidMonsterHealth;
    public float plasmaMonsterHealth;
    public float twilightMonsterHealth;
    public float goblinCount;
    public float voidCount;
    public float plasmaCount;
    public float twilightCount;
    
    //Statistics
    [Header("Statistics")]
    public float goblinKillCount;
    public float voidKillCount;
    public float plasmaKillCount;
    public float twilightKillCount;

    void Awake () {

        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if(control != this)
        {
            Destroy(gameObject);
        }
	}
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        EnemyController = GameObject.Find("EnemyController");
        playerController = GameObject.Find("PlayerController");
    }


    void FixedUpdate()
    {
        //if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("Game."))
        //{ 
            EnemyController.GetComponent<EnemyController>().MoveEnemies(player.transform.position);

            //if (!playerController.GetComponent<PlayerControllerScript>().isAlive)
            //{
                //playerController.GetComponent<PlayerControllerScript>().enabled = false;
            //}
       // }
    }

    void OnGUI()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("Game."))
        {
            GUI.Label(new Rect(10, 50, 100, 30), "Gems: " + gemsCollected + "/6");
            GUI.Label(new Rect(10, 70, 100, 30), "Keys: " + keysCollected + "/1");
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");

        GameData data = new GameData
        {
            playerHealth = playerHealth,
            playerSanity = playerSanity,
            voidPortalStatus = voidPortalStatus,
            plasmaPortalStatus = plasmaPortalStatus,
            twilightPortalStatus = twilightPortalStatus,
            gameTime = gameTime,
            goblinHealth = goblinHealth,
            bossHealth = bossHealth,
            voidMonsterHealth = voidMonsterHealth,
            plasmaMonsterHealth = plasmaMonsterHealth,
            twilightMonsterHealth = twilightMonsterHealth,
            goblinCount = goblinCount,
            voidCount = voidCount,
            plasmaCount = plasmaCount,
            twilightCount = twilightCount,
            goblinKillCount = goblinKillCount,
            voidKillCount = voidKillCount,
            plasmaKillCount = plasmaKillCount,
            twilightKillCount = twilightKillCount,
            characterSelect = characterSelect,
            musicVol = musicVol,
            soundFXVol = soundFXVol,
            gemsCollected = gemsCollected,
            keysCollected = keysCollected
    };

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            playerHealth = data.playerHealth;
            playerSanity = data.playerSanity;
            voidPortalStatus = data.voidPortalStatus;
            plasmaPortalStatus = data.plasmaPortalStatus;
            twilightPortalStatus = data.twilightPortalStatus;
            gameTime = data.gameTime;
            goblinHealth = data.goblinHealth;
            bossHealth = data.bossHealth;
            voidMonsterHealth = data.voidMonsterHealth;
            plasmaMonsterHealth = data.plasmaMonsterHealth;
            twilightMonsterHealth = data.twilightMonsterHealth;
            goblinCount = data.goblinCount;
            voidCount = data.voidCount;
            plasmaCount = data.plasmaCount;
            twilightCount = data.twilightCount;
            goblinKillCount = data.goblinKillCount;
            voidKillCount = data.voidKillCount;
            plasmaKillCount = data.plasmaKillCount;
            twilightKillCount = data.twilightKillCount;
            characterSelect = data.characterSelect;
            musicVol = data.musicVol;
            soundFXVol = data.soundFXVol;
            gemsCollected = data.gemsCollected;
            keysCollected = data.keysCollected;
        }
    }
}

[Serializable]
class GameData
{
    //Player Stats
    public float playerHealth;
    public float playerSanity;
    public float characterSelect;
    public float gemsCollected;
    public float keysCollected;

    //Game Stats
    public float voidPortalStatus;
    public float plasmaPortalStatus;
    public float twilightPortalStatus;
    public float gameTime;
    public float musicVol;
    public float soundFXVol;

    //Enemy Stats
    public float goblinHealth;
    public float bossHealth;
    public float voidMonsterHealth;
    public float plasmaMonsterHealth;
    public float twilightMonsterHealth;
    public float goblinCount;
    public float voidCount;
    public float plasmaCount;
    public float twilightCount;

    //Statistics
    public float goblinKillCount;
    public float voidKillCount;
    public float plasmaKillCount;
    public float twilightKillCount;
}