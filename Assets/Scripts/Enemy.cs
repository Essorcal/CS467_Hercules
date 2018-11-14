using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int health;


	// Use this for initialization
	void Update () {
		if(health <= 0)
        {
            Destroy(gameObject);
        }
	}

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("damage taken");
    }
}
