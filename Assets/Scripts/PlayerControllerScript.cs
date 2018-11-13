using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerScript : MonoBehaviour
{
    public GameObject character;
    public GameObject character2;
    public float runSpeedMultiplier = 1.05f;
    public float maxSpeed, attackTime;
    float originSpeed;
    private bool playerMoving, attacking;
    private float attackTimeCounter;
    new Rigidbody2D rigidbody2D;
    CapsuleCollider2D bodycollider;
    Vector2 move, lastMove;
    string action = "slashAttack";

    public bool isAlive = true;
    private GameObject chosenCharacter;

    Animator anim;


    /// CALEB ADDED
    public SimpleHealthBar healthBar;
    public SimpleHealthBar sanityBar;

    private GameObject _instance;

    public GameObject sanityUp;
    public GameObject attacked;

    private Animation maxAnimation;
    public float delay = 0f;

    void Start()
    {
        if (GameController.control.characterSelect == 1)
        {
            chosenCharacter = character2;
            Destroy(character);
            GameController.control.player = character2;
            Destroy(GameObject.Find("CM vcam Male"));
        }
        else
        {
            chosenCharacter = character;
            Destroy(character2);
            GameController.control.player = character;
            Destroy(GameObject.Find("CM vcam Female"));
        }
        anim = chosenCharacter.GetComponent<Animator>();
        originSpeed = maxSpeed;
        rigidbody2D = chosenCharacter.GetComponent<Rigidbody2D>();
        bodycollider = chosenCharacter.GetComponent<CapsuleCollider2D>();

    }

        void FixedUpdate()
    {
        if (!isAlive) { return; }
        move = Vector2.zero;
        playerMoving = false;

        if(Input.GetKeyDown("space"))
        {
            anim.SetBool("attacking", true); //Set the specified trigger in the animator
            anim.SetTrigger(action);
            attacking = true;
            attackTimeCounter = attackTime;
        }

        if (Input.GetKeyDown("q"))
        {
            _instance = Instantiate(sanityUp, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
            Destroy(_instance, _instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
        }

        if (attackTimeCounter > 0)
            attackTimeCounter -= Time.deltaTime;
        else
        {
            attacking = false;
            anim.SetBool("attacking", false);
        }


        if (!attacking)
        {
            if (Input.GetKey("left shift") || Input.GetKey("right shift"))  //Add the running multiplier
            {
                maxSpeed = 4 * runSpeedMultiplier;
                anim.speed = 2F;                        //Increase the animation speed for running
            }
            else
            {
                anim.speed = 1;
            }

            lastMove.x = move.x = Mathf.Lerp(0, Input.GetAxis("Horizontal") * maxSpeed, 0.8f); //Get the horizontal axis, interpolate between 0 and the input by 0.8
            lastMove.y = move.y = Mathf.Lerp(0, Input.GetAxis("Vertical") * maxSpeed, 0.8f);   //Get the vertical axis, interpolate between 0 and the input by 0.8
        }

        if (move.x != 0 || move.y != 0)
        {
            playerMoving = true;
            SetLastParams(lastMove);
        }

        anim.SetFloat("verticalSpeed", move.x);     //verticalSpeed variable in the animator controller to move.x
        anim.SetFloat("horizontalSpeed", move.y);   //horizontalSpeed variable in the animator controller to move.y
        anim.SetBool("playerMoving", playerMoving); //Set the playerMoving parameter in the animator

        rigidbody2D.velocity = new Vector2(move.x, move.y);    //Move the player
        maxSpeed = originSpeed;     //Reset the player's speed
        Spikes();
    }

    public bool Spikes()
    {
        if (bodycollider.IsTouchingLayers(LayerMask.GetMask("Spikes")))
        {
            isAlive = false;
            playerMoving = false;
            int speed = 0;
            
            //TODO: Fix terrible Spaghetti code
            anim.SetFloat("lastVert", speed);
            anim.SetFloat("lastHorz", speed);

            anim.SetFloat("verticalSpeed", speed);
            anim.SetFloat("horizontalSpeed", speed);
            anim.SetBool("playerMoving", playerMoving);
            rigidbody2D.velocity = new Vector2(speed, speed);
          
            anim.SetTrigger("Dying");

            return true;
        }
        return false;
    }

    public bool Attacked()
    {
        if (bodycollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            Debug.Log("Attacked");
            return true;
        }
        return false;
    }

    private void SetLastParams(Vector2 lastMove)
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
        
        anim.SetFloat("lastVert", lastParams.x);      //lastVert variable in the animator controller to move.x
        anim.SetFloat("lastHorz", lastParams.y);      //lastHorz variable in the animator controller to move.y
    }
}