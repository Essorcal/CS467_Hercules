using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaPuzzleScript : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameController.control.plasmaPortalStatus = 1;
    }

}