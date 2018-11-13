using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int health;

    private GameObject _instance;
    public GameObject GolDamaged;
    public float delay = 0f;

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
        _instance = Instantiate(GolDamaged, GameObject.Find("Golem (1)").transform.position, Quaternion.identity);
        Destroy(_instance, _instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
        Debug.Log("damage taken");
    }
}