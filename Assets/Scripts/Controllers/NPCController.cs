using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NPCController : MonoBehaviour {

    public float patrolTime = 5; // time in seconds to wait before seeking a new patrol destination
    public float aggroRange = 3; 

//Control movement 
    Transform playerTransform;
    private Animator animator;
    bool aggro = false;
    bool waiting = false;
    bool nextWaypoint = true;
    private float distanceFromTarget;
    public bool inView;

    public AttackSystem attack;
    Rigidbody2D body;
    CapsuleCollider2D bodyCollider;
    CharacterStats_SO currentStats, playerStats;
    private Attack attackCreated;

    Vector3 direction;
    Vector2 move, lastMove;
    public float walkSpeed = 2f;
    private int curTarget;
    public Waypoint currentWaypoint;

    int index = 0;

    float speed, enemySpeed;
    public float runSpeedMultiplier = 1.05f;
    //public float maxSpeed = 1f; //The fastest the enemy can run
    private float lastAttack, enemyMoveTimer;
    private bool attackOnCooldown, walking, newVector;
    public float attackDistance = 1.25f; //The furthest the enemy can be before they will attack
    public bool charAlive;
    private bool stopped = true;
    private EnemyManager enemyManager;



    private void Awake()    
    {
        enemyMoveTimer = 0;
        walking = newVector = true;
       
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        enemyManager = FindObjectOfType<EnemyManager>();
      
        
        InvokeRepeating("Tick", Random.Range(0, 1), 0.5f);

        lastAttack = float.MinValue;
        charAlive = true;

        //playerTransform.gameObject.GetComponent<DestroyedEvent>().IDied += PlayerDeath;   TODO
    }

    void Start()
    {
        currentStats = GetComponent<EnemyStats>().characterDefinition;
        playerStats = GameObject.FindWithTag("Player").GetComponent<CharacterStats>().characterDefinition;
        attack = gameObject.GetComponent<NPCController>().attack;
    }

    void Update()
    {
        if (currentStats == null)
        {
            print("currentStats is null");
        }
        float timeSinceLastAttack = Time.time - lastAttack;
        attackOnCooldown = timeSinceLastAttack > attack.Cooldown;

        SetTimer();
        enemyMoveTimer -= Time.deltaTime;

        if (currentStats.currentHealth <= 0 && charAlive)        //Once the NPC is dead destroy it
        {
            charAlive = false;
            StartCoroutine(blinkyDeath(0.25f));
        }
    }


    IEnumerator blinkyDeath(float blinkTime)    //Blink 3 times and then destroy the NPC game object
    {
        for (int i = 0; i < 6; i++)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(blinkTime);
        }
        if(enemyManager)
            enemyManager.OnEnemyDeath();
        playerStats.charExperience += gameObject.GetComponent<EnemyStats>().experiencePoints;
        Destroy(gameObject);
    }
    

    private void PlayerDeath()
    {
        charAlive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colObject = collision.gameObject;

        if (colObject.tag == "Player")
        {
            
            attackCreated = attack.CreateAttack(gameObject.GetComponent<EnemyStats>().characterDefinition);        //Create a new attack for this collision with the current player

            print("did " + attackCreated.Damage + " damage to " + colObject.name);

            playerStats.TakeDamage(attackCreated.Damage);
        }
    }

    //If the enemy is waiting then wait for a random time between 2 to 4 seconds
    //If the enemy is walking, walk for a random amount of time between 1 and 3 seconds
    void SetTimer()
    {
        if (enemyMoveTimer <= 0 && walking == false)      //The enemy has finished the wait cycle and now needs time walking
        {
            enemyMoveTimer = (float)(Random.Range(1, 4));        //Time Walking
            walking = true;
        }
        else if (enemyMoveTimer <= 0 && walking == true)   //The enemy has finished the walk cycle and now needs time waiting
        {
            enemyMoveTimer = (float)(Random.Range(2, 5));      //Time Waiting
            walking = false;
            newVector = true;
        }
    }

   
    
    void Patrol()
    {
        currentWaypoint = currentWaypoint.Next;
        nextWaypoint = true;
    }

    void Tick()
    {
        if (charAlive)
        {
            //Move enemies to waypoint unless player is near, then aggro
            if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < aggroRange)
            {
                AggroPlayer(); //Move to player position
            }
            else
            {
                if (currentWaypoint == null)      //No waypoints given, the NPC will roam their current area
                {
                    RoamArea();
                }
                else
                {
                    float randomNum = Random.Range(1.0f, 10.0f);

                    if (stopped && !waiting)
                    {
                        StartCoroutine(WaitToMove(randomNum));
                    }
                    else if (!waiting)
                    {
                        MoveToWaypoint();   //Move to the current waypoint;
                    }
                    else
                    {
                        body.velocity = new Vector2(0, 0);
                    }
                    
                }
            }
        }

    }

    IEnumerator WaitToMove(float time)
    {
        waiting = true;
        yield return new WaitForSeconds(time);
        waiting = false;
        MoveToWaypoint();
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

    void RoamArea()
    {
        if (walking)
        {
            if (newVector)     //A new vector is only needed once per walk cycle
            {
                lastMove.x = move.x = Mathf.Lerp(-1, 1, Random.Range(0, 1.0f));       //Interpolate between -1 and 1 by some random value from 0-1
                lastMove.y = move.y = Mathf.Lerp(-1, 1, Random.Range(0, 1.0f));
                newVector = false;                                                                 //Enemy now has a vector to move along during his move time
            }
            EnemyMoving();     //Set anim variables for moving

            GetComponent<Rigidbody2D>().velocity = new Vector2(move.x, move.y);        //Move the enemy along their walk vector
        }
        else
        {
            EnemyStopped();    //Set anim variables for being stopped
        }
    }

    //Set up animation parameters to correctly animate the enemy move cycle
    void EnemyMoving()
    {
        stopped = false;
        animator.SetFloat("lastVert", lastMove.x);      //lastVert variable in the animator controller to move.x
        animator.SetFloat("lastHorz", lastMove.y);      //lastHorz variable in the animator controller to move.y
        animator.SetFloat("verticalSpeed", move.x);     //verticalSpeed variable in the animator controller to move.x
        animator.SetFloat("horizontalSpeed", move.y);   //horizontalSpeed variable in the animator controller to move.y
        animator.SetBool("playerMoving", true);            //Set the playerMoving parameter in the animator

    }

    //Set up animation parameters to correctly animate the enemy stop cycle
    void EnemyStopped()
    {
        stopped = true;
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



    