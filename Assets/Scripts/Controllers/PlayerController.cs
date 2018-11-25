using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 1.05f;
    public float runMultiplier = 2f;
    public float maxSpeed = 3f;
    public float attackTime = 1f;
    public bool isAlive = true;
    public bool attacking;

    protected bool playerMoving;
    protected GameObject attackTarget;

    public WeaponSlot weapon;

    Rigidbody2D body;
    CapsuleCollider2D bodycollider;
    Vector2 move, lastMove;
    CharacterStats stats;
    CharacterStats_SO currentStats;
    Animator anim;

    
    //CharacterStats stats;
    void Awake()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponentInChildren<WeaponSlot>();
        weapon.player = gameObject;     //Set the weapon child's player member variable to this game object
        bodycollider = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
    }

    /// CALEB ADDED
   public SimpleHealthBar healthBar;
   public SimpleHealthBar sanityBar;

    void Start()
    {
        /* Need to complete listeners first
        stats.characterDefinition.OnLevelUp.AddListener(GameManager.Instance.OnLevelUp);
        stats.characterDefinition.OnPlayerDMG.AddListener(GameManager.Instance.OnPlayerDMG);
        stats.characterDefinition.OnPlayerGainHP.AddListener(GameManager.Instance.OnPlayerGainHP);
        stats.characterDefinition.OnPlayerDeath.AddListener(GameManager.Instance.OnPlayerDeath);
        stats.characterDefinition.OnPlayerInit.AddListener(GameManager.Instance.OnPlayerInit);
        */
        stats.characterDefinition.OnPlayerInit.Invoke();
        currentStats = stats.characterDefinition;

    }

    void FixedUpdate()
    {
        if (!isAlive) { return; }
        move = Vector2.zero;
        playerMoving = false;

        if (Input.GetKeyDown("space") && !attacking)
        {
            Attack("slashAttack", 2.0f);
        }
        /*
        if (Input.GetKeyDown("space"))
        {
            Attack("thrustAttack", 1.0f);
        }
        */

        Move();

        Spikes();
    }

    void Update()
    {
        if (currentStats.currentHealth <= 0 && isAlive)        //Once the Player is dead destroy the game object
        {
            StartCoroutine(playerDead());
        }
    }


    public void Attack(string attackType, float attackTime) 
    {
        var currentWeapon = stats.GetCurrentWeapon();
        if (currentWeapon != null)
        {
            StopAllCoroutines();
        }


        anim.SetBool("attacking", true);
        anim.SetTrigger(attackType);
        attacking = true;
        weapon.isAttacking = true;

        Invoke("NotAttacking", attackTime);

    }

    void NotAttacking()
    {
        attacking = false;
        weapon.isAttacking = false;
        anim.SetBool("attacking", false);
    }


    public void Move()
    {


        if (Input.GetKey("left shift") || Input.GetKey("right shift"))  //Add the running multiplier
        {
            maxSpeed = 2 * runMultiplier;
            anim.speed = 2F;                        //Increase the animation speed for running
        }
        else {
            anim.speed = 1f;
        }

        lastMove.x = move.x = Mathf.Lerp(0, Input.GetAxis("Horizontal") * maxSpeed, 0.8f); //Get the horizontal axis, interpolate between 0 and the input by 0.8
        lastMove.y = move.y = Mathf.Lerp(0, Input.GetAxis("Vertical") * maxSpeed, 0.8f);   //Get the vertical axis, interpolate between 0 and the input by 0.8


        if (move.x != 0f || move.y != 0)
        {

            playerMoving = true;
            SetLastParams(lastMove);
        }

        anim.SetFloat("verticalSpeed", move.x);     //verticalSpeed variable in the anim controller to move.x
        anim.SetFloat("horizontalSpeed", move.y);   //horizontalSpeed variable in the anim controller to move.y
        anim.SetBool("playerMoving", playerMoving); //Set the playerMoving parameter in the anim
        
        body.velocity = new Vector2(move.x, move.y);    //Move the player
        maxSpeed = moveSpeed;     //Reset the player's speed
    }

    protected void SetLastParams(Vector2 lastMove)
    {
        Vector2 lastParams = new Vector2(lastMove.x, lastMove.y);

        lastMove.x = Mathf.Abs(lastMove.x);     //Set x and y to their absolute values
        lastMove.y = Mathf.Abs(lastMove.y);

        lastParams.x = lastParams.x >= 0 ? 1 : -1;  //The x and y values of lastParams must be either 1 or -1 to make the animation transitions work correctly
        lastParams.y = lastParams.y >= 0 ? 1 : -1;

        if (lastMove.x >= lastMove.y)       //For animation transistion to work correctly either x or y must be 0
            lastParams.y = 0;               //Look at the values of lastMove and keep the greater value as 1 or -1, change the lesser value to 0
        else
            lastParams.x = 0;

        anim.SetFloat("lastVert", lastParams.x);      //lastVert variable in the anim controller to move.x
        anim.SetFloat("lastHorz", lastParams.y);      //lastHorz variable in the anim controller to move.y
    }

    public void Spikes()
    {
        if (bodycollider.IsTouchingLayers(LayerMask.GetMask("Spikes")))
        {
            currentStats.currentHealth = 0;
            StartCoroutine(playerDead());
        }
        
    }

    IEnumerator playerDead()
    {
        isAlive = false;
        playerMoving = false;
        int speed = 0;
        anim.SetBool("playerMoving", playerMoving);
        body.velocity = new Vector2(speed, speed);
        anim.SetTrigger("Dying");
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

}

