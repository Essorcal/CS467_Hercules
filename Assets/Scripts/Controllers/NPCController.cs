﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NPCController : MonoBehaviour {

    public float patrolTime = 5; // time in seconds to wait before seeking a new patrol destination
    public float aggroRange = 3; 

    public AudioClip spellAudio;
     
    Rigidbody2D body;
    CapsuleCollider2D bodyCollider;

//Control movement 
    Transform playerTransform;
    private Animator animator;
    bool aggro = false;
    bool waiting = false;
    bool nextWaypoint = true;
    private float distanceFromTarget;
    public bool inView;

    public AttackSystem attack;

    Vector3 direction;
    Vector2 move, lastMove;
    public float walkSpeed = 2f;
    private int curTarget;
    public Waypoint currentWaypoint;

    int index = 0;

    float speed, enemySpeed;
    public float runSpeedMultiplier = 1.05f;
    //public float maxSpeed = 1f; //The fastest the enemy can run
    private float lastAttack;
    private bool playerAlive, attackOnCooldown;
    public float attackDistance = 1.25f; //The furthest the enemy can be before they will attack

    private void Awake()    
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();

        playerTransform= GameObject.FindGameObjectWithTag("Player").transform;

        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
      
        
        InvokeRepeating("Tick", Random.Range(0, 1), 0.5f);

        lastAttack = float.MinValue;
        playerAlive = true;

        //playerTransform.gameObject.GetComponent<DestroyedEvent>().IDied += PlayerDeath;   TODO
    }

    private void PlayerDeath()
    {
        playerAlive = false;
    }

    void Update()
    {
        float timeSinceLastAttack = Time.time - lastAttack;
        attackOnCooldown = timeSinceLastAttack > attack.Cooldown;  
    }

    public void useAttack()
    {
        if (!playerAlive)
            return;
        if (attack is Weapon) //TODO
        {
            return;
            //((Weapon)attack).Swing(gameObject, playerTransform.gameObject);
        } 
       // else if (attack is Spell) 
       // {
            return; //TODO
      //  }
    }
    
    void Patrol()
    {
        currentWaypoint = currentWaypoint.Next;
        nextWaypoint = true;
    }

    void Tick()
    {
        //Move enemies to waypoint unless player is near, then aggro
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < aggroRange)
        {
            AggroPlayer(); //Move to player position
        }
        else
        {
            MoveToWaypoint();   //Move to the current waypoint;
        }

    }

    void MoveToWaypoint()
    {
        Vector2 waypointVector = currentWaypoint.transform.position - transform.position;       //Get a vector to the waypoint (Destination - Source)

        lastMove.x = move.x = waypointVector.x;
        lastMove.y = move.y = waypointVector.y;

 
        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < 2)       //If the NPC has reached the waypoint invoke a countdown to get the next waypoint
        {
            body.velocity = new Vector2(0, 0);
            EnemyStopped();
            if (nextWaypoint)
            {
                nextWaypoint = false;
                Invoke("Patrol", Random.Range(patrolTime - 1, patrolTime));
            }
        }
        else
        {
            body.velocity = new Vector2(waypointVector.x, waypointVector.y).normalized * walkSpeed;      //Move the enemy towards the waypoint
            EnemyMoving();     //Set anim variables for moving
        }

    }

    //Set up animation parameters to correctly animate the enemy move cycle
    void EnemyMoving()
    {
        animator.SetFloat("lastVert", lastMove.x);      //lastVert variable in the animator controller to move.x
        animator.SetFloat("lastHorz", lastMove.y);      //lastHorz variable in the animator controller to move.y
        animator.SetFloat("verticalSpeed", move.x);     //verticalSpeed variable in the animator controller to move.x
        animator.SetFloat("horizontalSpeed", move.y);   //horizontalSpeed variable in the animator controller to move.y
        animator.SetBool("playerMoving", true);            //Set the playerMoving parameter in the animator

    }

    //Set up animation parameters to correctly animate the enemy stop cycle
    void EnemyStopped()
    {
        body.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        animator.SetBool("playerMoving", false);            //Set the playerMoving parameter in the animator
        animator.SetFloat("verticalSpeed", 0);     //verticalSpeed variable in the animator controller to move.x
        animator.SetFloat("horizontalSpeed", 0);   //horizontalSpeed variable in the animator controller to move.y
    }


    //Call this and the enemy will begin to move towards the player.
    void AggroPlayer()
    {
        if (Vector3.Distance(body.transform.position, playerTransform.position) <= attackDistance && attackOnCooldown)  //The player is close enough for the enemy to attack
        {
            animator.SetTrigger("attack");
            lastAttack = Time.time;
        }
        else if (Vector3.Distance(body.transform.position, playerTransform.position) <= attackDistance && !attackOnCooldown)  //The player is close enough to attack the the timer needs to cooldown
        {
            EnemyStopped();    //Set anim variables for being stopped
        }
        else                   //The player is to far away, move the enemy towards the player
        {
            Vector2 playerVector = playerTransform.position - body.transform.position;       //Get a vector to the player (Destination - Source)

            lastMove.x = move.x = playerVector.x;
            lastMove.y = move.y = playerVector.y;

            body.velocity = new Vector2(playerVector.x, playerVector.y);      //Move the enemy towards the player

            EnemyMoving();     //Set anim variables for moving
        }
    }


}



    