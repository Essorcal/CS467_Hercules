using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.GetComponentInParent<WeaponSlot>().PullTrigger(collision);
    }

}
