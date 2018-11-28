using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {

    public static GameController control;
    CharacterStats playerStats;
    EnemyManager enemyManager;

    [Header("Inventory")]
    public string[] itemsInventory;
    public int[] numberOfItems;
    public Item[] refItems;
    public Weapon[] refWeapons;

   
    //Game Stats
    [Header("Game Stats")]
    public int gemsCollected;
    public int weaponsCollected;
    public int voidPortalStatus;
    public int plasmaPortalStatus;
    public int twilightPortalStatus;
    public int enemiesKilled;
    public int bossesKilled;
    public bool gameWon;

    //Spawn Enemies
    public int currentEnemyCount = 0;
    private bool mainScene = true;       //The game initially starts at the main scene
    private List<GameObject> PortalList = new List<GameObject>();
    private int currentWaveNumber = 0;
    private int currentPortalIndex = -1;


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
        playerStats = CharacterStats.instance;
        enemyManager = FindObjectOfType<EnemyManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;                              //Register the OnSceneLoaded with the SceneManager sceneLoaded events handler
        PortalList.AddRange(GameObject.FindGameObjectsWithTag("Portal"));       //Create a list of all the portals in the main scene
        DisablePortals();
    }

    void Update()
    {
        GameMenu.instance.SetItemButtons();
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddItem("Health Potion");
            AddItem("Sanity Potion");

            //RemoveItem("Sanity Potion");
        }
        
        if(mainScene)
        {
            if(currentEnemyCount <= 0)  //Attempt to spawn a new wave
            {
                if(!CreatePortal())     //No portal was able to be created the player has won the game
                {
                    gameWon = true;
                }
                else
                {
                    enemyManager.SpawnWave(currentWaveNumber);  //Spawn a new wave
                    currentWaveNumber++;
                }
            }
        }
    }

    private bool CreatePortal()
    {
        if (PortalList.Count == 0)
            return false;   //No more portals left to spawn the player has won the game
        if(currentPortalIndex >= 0)
        {
            PortalList[currentPortalIndex].SetActive(false);
            PortalList.RemoveAt(currentPortalIndex);
        }

        currentPortalIndex = Random.Range(0, PortalList.Count);    //Get a random portal to spawn enemies from
        PortalList[currentPortalIndex].SetActive(true);                //Activate that portal and its spawn point
        enemyManager.currentSpawn = PortalList[currentPortalIndex].GetComponentInChildren<Spawn>().transform; //Set current spawn in the enemy manager
        return true;
    }

    private void DisablePortals()
    {
        foreach(GameObject portal in PortalList)
        {
            portal.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        enemyManager = FindObjectOfType<EnemyManager>();

        if (aScene.name == "Game.MainScene")
        {
            mainScene = true;
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void QuitMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");

        GameData data = new GameData
        {
            playerHealth = playerStats.GetCurrentHealth(),
            playerSanity = playerStats.GetCurrentSanity(),
            playerLvl = playerStats.GetLevel(),
            playerXP = playerStats.GetXP(),
            voidPortalStatus = voidPortalStatus,
            plasmaPortalStatus = plasmaPortalStatus,
            twilightPortalStatus = twilightPortalStatus,
            gemsCollected = gemsCollected,
            gameWon = gameWon
        
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

            playerStats.characterDefinition.currentHealth = data.playerHealth;
            playerStats.characterDefinition.currentSanity = data.playerSanity;
            //playerStats.characterDefinition.baseDamage
            voidPortalStatus = data.voidPortalStatus;
            plasmaPortalStatus = data.plasmaPortalStatus;
            twilightPortalStatus = data.twilightPortalStatus;
            gameWon = data.gameWon;
            gemsCollected = data.gemsCollected;
           
        }
    }

    public Item GetItemInfo(string item)
    {
        for (int i = 0; i < refItems.Length; i++)
        {
            if (refItems[i].itemName == item)
            {
                return refItems[i];
            }
        }

        return null;
    }

    public void SortItems() //Make sure there are no gaps in Inventory
    {
        bool itemPresent = true;

        while (itemPresent)
        {
            itemPresent = false;
            for (int i = 0; i < itemsInventory.Length - 1; i++)
            {
                if (itemsInventory[i] == "")
                {
                    itemsInventory[i] = itemsInventory[i + 1];
                    itemsInventory[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1];
                    numberOfItems[i + 1] = 0;

                    if (itemsInventory[i] != "")
                    {
                        itemPresent = true;
                    }
                }
            }
        }
    }

    //Add item to inventory
    public void AddItem(string item)
    {
        int itemPos = 0;
        bool foundSlot = false;

        for (int i = 0; i < itemsInventory.Length; i++)
        {
            if (itemsInventory[i] == "" || itemsInventory[i] == item)
            {
                itemPos = i;
                foundSlot = true;
                break;
                
            }
        }
        if (foundSlot)
        {
            bool isItem = false;
            for (int i = 0; i < refItems.Length; i++)
            {
                if (refItems[i].itemName == item)
                {
                    isItem = true;
                    break;
                }
            }

            if (isItem)
            {
                itemsInventory[itemPos] = item;
                numberOfItems[itemPos]++;
            }
            else
            {
                Debug.LogError(item + " does not exist");
            }
        }
        GameMenu.instance.SetItemButtons();

    }

    public void RemoveItem(string item)
    {
        bool foundItem = false;
        int itemPos = 0;

        for(int i = 0; i < itemsInventory.Length; i++)
        {
            if (itemsInventory[i] == item)
            {
                foundItem = true;
                itemPos = i;
                break;
            }
        }
        if (foundItem)
        {
            numberOfItems[itemPos]--;
            if (numberOfItems[itemPos] <= 0)
            {
                itemsInventory[itemPos] = "";
            }

            GameMenu.instance.SetItemButtons();

        } else
        {
            Debug.LogError("Unable to locate " + item);
        }
    }
}

[Serializable]
class GameData
{
    public int playerHealth;
    public int playerSanity;
    public int playerXP;
    public int playerXPRequired;
    public int playerLvl;   
    public int gemsCollected;
    public int weaponsCollected;
    public int enemiesKilled;
    public int bossesKilled;
    public int voidPortalStatus;
    public int twilightPortalStatus;
    public int plasmaPortalStatus;
    public bool gameWon;
}