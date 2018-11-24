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

    void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
