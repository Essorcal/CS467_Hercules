using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour {

    public Weapon currentWeapon;

	void Start () {
        Instantiate(currentWeapon.weaponPreb, new Vector3(0,0,0), Quaternion.identity, gameObject.transform);
	}
	
	void Update () {

    }

    public void PullTrigger(Collider2D collision)
    {
        GameObject colObject = collision.gameObject;
        CharacterStats_SO stats;

        if (colObject.tag == "Player" || colObject.tag == "Enemy")
        {
            stats = colObject.GetComponent<CharacterStats>().characterDefinition_Template;
            print(collision.gameObject.GetComponent<CharacterStats>().characterDefinition_Template.currentHealth);
        }
    }
}
