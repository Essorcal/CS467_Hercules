using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidPuzzleScript : MonoBehaviour {

    public GameObject portal;

	// Use this for initialization
	void Start () {
        portal.SetActive(false);
        
	}

    private void FixedUpdate()
    {
        if (GameObject.FindGameObjectsWithTag("PuzzleItem").Length == 0)
        {
            GameController.control.voidPortalStatus = 1;
            portal.SetActive(true);
        }
    }

}
