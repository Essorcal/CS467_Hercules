using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{

    private GameObject player;
    public GameObject[] playerOptions;
    public Vector3 spawnPos = new Vector3(.004f, .004f, -0.04801377f);
    public static CharacterSelector instance;
    public int characterChoice;
    public string scene;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        

    }

    public void SelectCharacter(int choice)
    {
        characterChoice = choice;        
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(scene);
    }

    public PlayerController InstantiateCharacter()
    {
       
        if (player != null)
        {
            Destroy(player);
            player = null;

        }      
        return  Instantiate(playerOptions[characterChoice]).GetComponent<PlayerController>();

    }

}
