using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D playerRB;

    //Prevent duplicate players
    public static PlayerController instance;

    public float moveSpeed = 2f;
    public float runMultiplier = 2f;
    //public float maxSpeed = 3f;
    public float attackTime = 2f;
    public string levelTransitionName; //exit or entrance we just used
    protected bool attacking;
    protected float attackTimeCounter;

    public bool canMove = true;
    public bool playerMoving = false;
    
    CapsuleCollider2D bodycollider;

    /// CALEB ADDED
    public SimpleHealthBar healthBar;
    public SimpleHealthBar sanityBar;



     double timeBtwnSteps = 0.317;
     double ellapsedStepTime;
    /*
   [Header("Music")]
   public AudioClip twilightMusic;
   public AudioClip mainMusic;
   public AudioClip voidMusic;
   public AudioClip plasmaMusic;

   private AudioSource[] sources;
    */

    protected GameObject attackTarget;
    public bool isAlive = true;
    CharacterStats stats;

    public Animator playerAnim;

    
    //CharacterStats stats;
    void Awake()
    {
        playerAnim = GetComponent<Animator>();
        bodycollider = GetComponent<CapsuleCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
       
    }
   

    void Start()
    {
        if (instance == null) 
        {
            //When game starts, instance value set to this player
            instance = this;
        } else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }           
        }
        DontDestroyOnLoad(gameObject);
        /* Need to complete listeners first
        stats.characterDefinition.OnLevelUp.AddListener(GameManager.Instance.OnLevelUp);
        stats.characterDefinition.OnPlayerDMG.AddListener(GameManager.Instance.OnPlayerDMG);
        stats.characterDefinition.OnPlayerGainHP.AddListener(GameManager.Instance.OnPlayerGainHP);
        stats.characterDefinition.OnPlayerDeath.AddListener(GameManager.Instance.OnPlayerDeath);
        stats.characterDefinition.OnPlayerInit.AddListener(GameManager.Instance.OnPlayerInit);
        */
        //stats.characterDefinition.OnPlayerInit.Invoke();

       

        
        


    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            Death();
            return;           
        }

        /*

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Attack("slashAttack", 2.0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack("thrustAttack", 1.0f);
        }
        */
        Move();
        


    }

    public void Attack(string attackType, float attackTime) 
    {
        var weapon = stats.GetCurrentWeapon();
        if (weapon != null)
        {
            StopAllCoroutines();
        }
        playerAnim.SetBool("attacking", true);
        playerAnim.SetTrigger(attackType);
        attacking = true;
        attackTimeCounter = attackTime;

        while (attackTimeCounter > 0) 
        {
            attackTimeCounter -= Time.deltaTime;
        }

        attacking = false;
        playerAnim.SetBool("attacking", false);
    }


    public void Move()
    {
        playerMoving = false;
        if (canMove)
        {
            playerRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed;
            
        }
        else
        {
            playerRB.velocity = Vector2.zero;
        }
       

        playerAnim.SetFloat("moveX", playerRB.velocity.x);
        playerAnim.SetFloat("moveY", playerRB.velocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            playerMoving = true;
            ellapsedStepTime += Time.deltaTime;
            if (ellapsedStepTime >= timeBtwnSteps)
            {
                PlayWalkSound();
                ellapsedStepTime -= timeBtwnSteps;
            }


            if (canMove)
            {
                playerAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
                playerAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
            }
            
        }
    }

    public void Death()
    {
        canMove = false;
        playerAnim.SetBool("playerMoving", false);
        playerRB.velocity = Vector2.zero;
        playerAnim.SetTrigger("Dying");    
    
    }


    public void PlayWalkSound()
    {
        AudioManager.Instance.PlaySFX(0);
    }

}

