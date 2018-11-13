using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public float runSpeedMultiplier = 1.05f;
    public float maxSpeed = 1f; //The fastest the enemy can run
    public float attackCooldown = 3f; //The time between enemy attacks
    public float attackDistance = 1.25f; //The furthest the enemy can be before they will attack
    public float aggroDistance = 3.0f;   //The distance before the enemy will aggro to the player
    public GameObject[] enemies;

    private Vector3 playerRelation, forwardVector;
    private float originSpeed;
    EnemyData[] enemyList;      //Struct used for storing data about all the enemies in the scene

    struct EnemyData
    {
        public Vector2 move, lastMove;
        public Animator anim;
        public float enemyMoveTimer, attackTimer;
        public bool walking, newVector, attacking, running;
    }

    //Set up variables with initial values
    private void Awake()    
    {
        enemyList = new EnemyData[enemies.Length];

        for (int i = 0; i < enemyList.Length; i++)
        {
            enemyList[i].enemyMoveTimer = 0f;
            enemyList[i].walking = enemyList[i].newVector = true;
            enemyList[i].anim = enemies[i].GetComponent<Animator>();
            enemyList[i].attackTimer = attackCooldown;
        }
    }

    //Move the enemies based on the player's location
    public void MoveEnemies(Vector3 playerLoc)
    {
        SetTimer();       
        DecreaseTimer();

        for (int i = 0; i < enemies.Length; i++)
        {
            playerRelation = (playerLoc - enemies[i].transform.position).normalized;        //Get a vector from the enemy to the player
            forwardVector = new Vector3(enemyList[i].move.x, enemyList[i].move.y, 0);       //Get the enemies forward vector

            //Check if the player is within the required distance and if the player is in front of the enemy
            if (Vector3.Distance(enemies[i].transform.position, playerLoc) <= aggroDistance && Vector3.Dot(playerRelation, forwardVector) > 0)      
            {
                AggroPlayer(i, playerLoc);          //Aggro to the player if the player is close enough to the enemy
            }
            else
            {
                RoamArea(i);                        //Walk aimlessly if the player is not nearby
            }
        }
    }


    //Set up the timer for each individual enemy. If the enemy is waiting then wait for a random time between 2 to 4 seconds
    //If the enemy is walking, walk for a random amount of time between 1 and 3 seconds
    void SetTimer()
    {
        for (int i = 0; i < enemyList.Length; i++)
        {
            if (enemyList[i].enemyMoveTimer <= 0 && enemyList[i].walking == false)      //The enemy has finished the wait cycle and now needs time walking
            {
                enemyList[i].enemyMoveTimer = (float)(Random.Range(1, 4));        //Time Walking
                enemyList[i].walking = true;                               
            }
            else if (enemyList[i].enemyMoveTimer <= 0 && enemyList[i].walking == true)   //The enemy has finished the walk cycle and now needs time waiting
            {
                enemyList[i].enemyMoveTimer = (float)(Random.Range(2, 5));      //Time Waiting
                enemyList[i].walking = false;
                enemyList[i].newVector = true;
            }
        }
    }

    //This should be called after SetTimer()
    void DecreaseTimer()
    {
        for (int i = 0; i < enemyList.Length; i++)
        {
            enemyList[i].enemyMoveTimer -= Time.deltaTime;
        }
    }

    //Set up animation parameters to correctly animate the enemy move cycle
    void EnemyMoving(int i)
    {
        enemyList[i].anim.SetFloat("lastVert", enemyList[i].lastMove.x);      //lastVert variable in the animator controller to move.x
        enemyList[i].anim.SetFloat("lastHorz", enemyList[i].lastMove.y);      //lastHorz variable in the animator controller to move.y
        enemyList[i].anim.SetFloat("verticalSpeed", enemyList[i].move.x);     //verticalSpeed variable in the animator controller to move.x
        enemyList[i].anim.SetFloat("horizontalSpeed", enemyList[i].move.y);   //horizontalSpeed variable in the animator controller to move.y
        enemyList[i].anim.SetBool("playerMoving", true);            //Set the playerMoving parameter in the animator
 
    }

    //Set up animation parameters to correctly animate the enemy stop cycle
    void EnemyStopped(int i)
    {
        enemies[i].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        enemyList[i].anim.SetBool("playerMoving", false);            //Set the playerMoving parameter in the animator
        enemyList[i].anim.SetFloat("verticalSpeed", 0);     //verticalSpeed variable in the animator controller to move.x
        enemyList[i].anim.SetFloat("horizontalSpeed", 0);   //horizontalSpeed variable in the animator controller to move.y
    }

    //Call this with the enemy location in the array to set cause it to roam
    void RoamArea(int i)
    {
        if (enemyList[i].walking)
        {
            if (enemyList[i].newVector)     //A new vector is only needed once per walk cycle
            {
                enemyList[i].lastMove.x = enemyList[i].move.x = Mathf.Lerp(-1, 1, Random.Range(0, 1.0f));       //Interpolate between -1 and 1 by some random value from 0-1
                enemyList[i].lastMove.y = enemyList[i].move.y = Mathf.Lerp(-1, 1, Random.Range(0, 1.0f));
                enemyList[i].newVector = false;                                                                 //Enemy now has a vector to move along during his move time
            }
            EnemyMoving(i);     //Set anim variables for moving

            enemies[i].GetComponent<Rigidbody2D>().velocity = new Vector2(enemyList[i].move.x, enemyList[i].move.y);        //Move the enemy along their walk vector
        }
        else
        {
            EnemyStopped(i);    //Set anim variables for being stopped
        }
    }

    //Call this with the enemy location in the enemies array and the player's current position in the game
    //and the enemy will begin to move towards the player.
    void AggroPlayer(int i, Vector3 playerLoc)
    {
        enemyList[i].attackTimer -= Time.deltaTime;

        if(Vector3.Distance(enemies[i].transform.position, playerLoc) <= attackDistance && enemyList[i].attackTimer <= 0)
        {
            enemyList[i].anim.SetTrigger("attack");            
            enemyList[i].attackTimer = attackCooldown;          //Reset the attack timer
        }
        else if(Vector3.Distance(enemies[i].transform.position, playerLoc) <= attackDistance && enemyList[i].attackTimer > 0)
        {
            EnemyStopped(i);    //Set anim variables for being stopped
        }
        else
        {
            Vector2 playerVector = playerLoc - enemies[i].transform.position;       //Get a vector to the player (Destination - Source)

            enemyList[i].lastMove.x = enemyList[i].move.x = playerVector.x;
            enemyList[i].lastMove.y = enemyList[i].move.y = playerVector.y;

            enemies[i].GetComponent<Rigidbody2D>().velocity = new Vector2(playerVector.x, playerVector.y);      //Move the enemy towards the player

            EnemyMoving(i);     //Set anim variables for moving
        }
    }
}
