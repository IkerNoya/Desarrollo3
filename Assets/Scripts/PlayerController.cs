using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] float speed;
    [SerializeField] float wallStickiness;
    [SerializeField] float jumpForce;
    [SerializeField] float distanceToGround = 0.01f;
    [SerializeField] float distanceToWall = 0.01f;
    [Space]
    [SerializeField] string[] animNames;
    [Space]
    [SerializeField] string playerAxis;
    [SerializeField] string joystickAxis;
    [SerializeField] int noOfJumps;
    [Space]
    [SerializeField] KeyCode jumpButtonKM;
    [SerializeField] KeyCode jumpButtonJoystick;
    [Space]
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] BoxCollider2D playerCollider;
    [Space]
    [SerializeField] Vector3 InitialPos;
    [Space]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask layerWallL;
    [SerializeField] LayerMask layerWallR;
    [Space]
    [SerializeField] GameObject enemy;
    [Space]
    public SlowMotion slowMotion;
    public enum PlayerSelect
    {
        player1, player2
    }
    public enum State
    {
        Grounded, Jumping, Falling, InWall, WallJump
    }
    public float shakeDuration;
    public float shakeMagnitude;
    public CombatController comboController;
    public CameraShake cameraShake;
    [Space]
    public PlayerSelect playerSelect;
    State state;
    [Space]
    public Animator anim;

    bool isGrounded = false;
    bool wasGrounded = false;
    bool jumped = false;
    bool isDead = false;
    bool canMove = true;
    bool canWallJump = true;
    bool isInWall = false;
    bool jumpInWall = false;
    bool wallJump;
    bool leftOrRighWall = false; // true = left, false = right;
    bool wj = false;

    ParryController parryController;

    [HideInInspector]
    public float hp = 100;

    int jumpAmmount;

    float direction;
    float LastDirection;
    float lastVelocity;
    readonly float runAxisLimit = 0.10f;

    Vector2 movement;

    Camera cam;
    #endregion

    #region Actions
    public static Action<PlayerController> takeDamage;
    public static Action<PlayerController> EmptyHP;
    #endregion

    #region BASE_FUNCTIONS
    private void Awake()
    {
     
    }
    void Start()
    {
        cam = Camera.main; 
        jumpAmmount = noOfJumps;
        comboController.hitCol.SetActive(false);
        transform.position = new Vector3(InitialPos.x, InitialPos.y, InitialPos.z);
        InitialPos = cam.WorldToScreenPoint(transform.localPosition);
        wallJump = true;
        parryController = GetComponentInChildren<ParryController>();
        Time.timeScale = 1; // Dont touch for now
    }

    void Update()
    {
        if (isDead)
            return;
        if (!canMove)
        {
            direction = 0;
            movement = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
        }
            
        direction = Input.GetAxis(playerAxis) + Input.GetAxis(joystickAxis);
        movement = new Vector2(direction, 0) * speed;
        Inputs();
        if (direction != 0)
        {
            LastDirection = direction;
        }
        if (LastDirection > 0/* && !isInWall*/)
        {
            player.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (LastDirection < 0/* && !isInWall*/)
        {
            player.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
        if (direction > 0 && wallJump)
        {
            isInWall = Physics2D.Raycast(transform.position, Vector2.right, distanceToWall, layerWallR);
        }
        else if (direction < 0 && wallJump)
        {
            isInWall = Physics2D.Raycast(transform.position, Vector2.left, distanceToWall, layerWallL);
        }
        else if (!wallJump)
        {
            isInWall = false;
        }
        if (!isInWall && !isGrounded)
        {
            ResetWallJump();
        }
        if (isGrounded && !wasGrounded)
        {
            ResetWallJump();
            anim.ResetTrigger("Jump");
            canWallJump = true;
            wasGrounded = true;
            jumpAmmount = noOfJumps;
        }
        if (isGrounded && !jumped) state = State.Grounded;
        else if (jumped) state = State.Jumping;
        else if (!isGrounded && isInWall) state = State.InWall;
        wasGrounded = isGrounded;
        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("VelocityY", rigidBody.velocity.y);
        anim.SetFloat("VelocityX", Mathf.Abs(direction));
        if (wj) anim.SetTrigger("WJ");
    }

    void FixedUpdate()
    {
        if (canMove && rigidBody != null)
        {
            switch (state)
            {
                case State.Grounded:
                    if (canMove)
                    {
                        ResetWallJump();
                        jumpAmmount = 2;
                        rigidBody.velocity = new Vector2(movement.x, rigidBody.velocity.y);
                    }
                    break;

                case State.Jumping:
                    anim.SetTrigger("Jump");
                    lastVelocity = 0;
                    rigidBody.velocity = new Vector2(movement.x, jumpForce);
                    jumped = false;
                    state = State.Falling;
                    break;

                case State.Falling:
                    rigidBody.velocity = new Vector2(movement.x + lastVelocity, rigidBody.velocity.y);
                    if (Mathf.Abs(rigidBody.velocity.x) >= speed)
                        rigidBody.velocity = new Vector2(speed * Mathf.Sign(rigidBody.velocity.x), rigidBody.velocity.y);
                    break;

                case State.InWall:
                    if (!isGrounded) 
                    {
                        anim.ResetTrigger("Jump");
                        StartCoroutine(WallSlideTransition(0.1403281f));
                        rigidBody.velocity = new Vector2(movement.x, wallStickiness);
                        if (jumpInWall)
                        {
                            StartCoroutine(WallJumpCoolDown(0.1f));
                        }
                    }
                        break;

            }
        }

    }
    #endregion

    #region FUNCTIONS

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    
    void Inputs()
    {

        if ((Input.GetKeyDown(jumpButtonKM) || Input.GetKeyDown(jumpButtonJoystick)) && jumpAmmount > 0 && canMove && !comboController.IsAttacking)
        {
            state = State.Jumping; 
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        if ((Input.GetKeyDown(jumpButtonKM) || Input.GetKeyDown(jumpButtonJoystick)) && isInWall)
        {
            jumpInWall = true;
        }
    }
    void ResetWallJump()
    {
        anim.SetBool("WallSlide", false);
        anim.SetBool("WallStick", false);
        anim.SetBool("WallJump", false);
        anim.ResetTrigger("WJ");
    }
    void Dead()
    {
        isDead = true;
       
        rigidBody.simulated = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //StartCoroutine(slowMotion.ActivateSlowMotion(1.5f, 0.5f));
    }

    void Respawn()
    {
        rigidBody.simulated = true;
        hp = 100;
        isDead = false;
        transform.position = cam.ScreenToWorldPoint(new Vector3(InitialPos.x, InitialPos.y, InitialPos.z));
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
       
    }
    public bool GetCanMove()
    {
        return canMove;
    }
    public bool GetGrounded()
    {
        return isGrounded;
    }
    #endregion

    #region COLLISIONS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OutofBounds"))
        {
            hp = 0;
            Dead();
            EmptyHP(this);
            StartCoroutine(RespawnPlayer());
        }
        if (collision.gameObject.CompareTag("Walls"))
        {
            if (!isGrounded && canWallJump)
            {
                jumpAmmount = 2;
                canWallJump = false;
            }
            if (transform.position.x > collision.transform.position.x)
            {
                leftOrRighWall = true;
            }
            else
            {
                leftOrRighWall = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitCollider") && collision.gameObject.layer != gameObject.layer && !parryController.GetBlockDamage())
        {
            // add force later
            anim.SetTrigger("Damage");
            StartCoroutine(TakeDamage(0.75f));
            hp -= collision.gameObject.GetComponentInParent<CombatController>().GetDamage();
            StartCoroutine(HitCooldown());
            takeDamage(this);
            if (hp <= 0)
            {
                Dead();
                EmptyHP(this);
                StartCoroutine(RespawnPlayer());
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("Walls"))
        {
            state = State.Falling;
        }
    }
    #endregion

    #region COROUTINES

    IEnumerator WallSlideTransition(float animTime)
    {
        anim.SetBool("WallStick", true);
        yield return new WaitForSeconds(animTime);
        anim.SetBool("WallStick", false);
        anim.SetBool("WallSlide", true);
        yield return null;
    }
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2.0f);
        Respawn();
        yield break;
    }
    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
       
        yield return null;
    }
    IEnumerator WallJumpCoolDown(float JumpTimer)
    {
        wj = true;
        anim.SetTrigger("Jump");
        wallJump = false;
        yield return new WaitForSeconds(JumpTimer);
        if (jumpAmmount > 0) 
        {
            ResetWallJump();
            if (leftOrRighWall)
            {
                rigidBody.velocity = new Vector2(jumpForce / 2, jumpForce);
            }
            else
            {
                rigidBody.velocity = new Vector2(-jumpForce / 2, jumpForce);

            }
            lastVelocity = rigidBody.velocity.x;
        }
        yield return new WaitForSeconds(0.2f);
        state = State.Falling;
        wj = false;
        jumpInWall = false;
        wallJump = true;
        yield return null;
    }
    IEnumerator TakeDamage(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        yield return null;
    }
    #endregion COROUTINES

}
