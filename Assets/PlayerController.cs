using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CapsuleCollider2D playerCollider;
    public float moveSpeed = 1.05f;

    private Enemy enemyScript;

    private RaycastHit hit;
    private Ray ray;
    public float rayDistance = 4;

    private void Start()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
    }

}
