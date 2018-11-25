using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour {

    public Weapon currentWeapon;
    public bool isAttacking = false;
    public GameObject player;

    private List<string> unattackable = new List<string>();
    private Attack attackCreated;


    void Start () {
        currentWeapon.weaponPreb.transform.position = new Vector3(0, 0, 0);
        currentWeapon.weaponPreb.transform.rotation = Quaternion.identity;
        Instantiate(currentWeapon.weaponPreb, gameObject.transform, false);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(!isAttacking && unattackable.Count > 0)  //After the attack is over clear out the list
        {
            unattackable.Clear();
        }
    }

    //Triggered by the child weapon prefab instantiated in Start()
    public void PullTrigger(Collider2D collision)
    {
        GameObject colObject = collision.gameObject;
        CharacterStats_SO stats;

        if (colObject.tag == "Player" || colObject.tag == "Enemy" && isAttacking && Attackable(colObject.name))
        {
            AddEnemy(colObject.name);   //Add the enemies name to the list of enemies already attacked

            attackCreated = currentWeapon.ExecuteAttack(player);        //Create a new attack for this collision with the current player

            print(gameObject.name + " did " + attackCreated.Damage + " damage to " + colObject.name);

            stats = colObject.GetComponent<EnemyStats>().characterDefinition;
            stats.TakeDamage(attackCreated.Damage);
        }
    }

    //Check if the enemy name is in the list of enemies that have already been dealt damage to
    //Return false if it is
    bool Attackable(string enemyName)
    {
        foreach(string name in unattackable)
        {
            if (enemyName == name)
                return false;
        }
        return true;
    }

    //Add the enemy name to the list of enemies that cannot be attacked at this time
    void AddEnemy(string enemyName)
    {
        foreach (string name in unattackable)
        {
            if (enemyName == name)
                return;
        }
        unattackable.Add(enemyName);
    }
}

