using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dealDamage : MonoBehaviour {

    public float stormTimer = 2;
    private float timeLeft;
    private bool canDamage = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        if (!canDamage)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                canDamage = true;
                timeLeft = stormTimer;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDamage)
        {
            if (PlayerController.instance.isAlive == true) //make sure sfx only plays once
            {
                AudioManager.Instance.PlaySFX(9);
            }
            PlayerController.instance.takeDamage(30);
            canDamage = false;
        }
    }
}
