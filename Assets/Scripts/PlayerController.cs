using System;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES
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
    [SerializeField] KeyCode dashButtonKM;
    [SerializeField] KeyCode dashButtonJoystick;
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
    [SerializeField] float knockBackForce;
    [SerializeField] float criticalKnockBackForce;
    #endregion

    #region PUBLIC_VARIABLES
    public SlowMotion slowMotion;
    public enum PlayerSelect
    {
        player1, player2
    }
    public enum State
    {
        Grounded, Jumping, Falling, InWall, WallJump, Dash
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
    #endregion

    #region PRIVATE_VARIABLES
    bool isGrounded = false;
    bool wasGrounded = false;
    bool jumped = false;
    bool isDead = false;
    bool canMove = true;
    bool canWallJump = true;
    bool isInWall = false;
    bool jumpInWall = false;
    bool leftOrRighWall = false; // true = left, false = right;
    bool wj = false;
    bool isCriticalHit;
    bool canDash = true;
    bool isDashing = false;

    ParryController parryController;

    [HideInInspector]
    public float hp = 100;

    int jumpAmmount;

    float direction;
    float LastDirection;
    float lastVelocity;
    float lastDashVelocity;
    readonly float runAxisLimit = 0.10f;
    float dashCooldown = 0;
    float dashDuration = 0;

    Vector2 movement;

    Camera cam;
    #endregion

    #region Actions
    public static Action<PlayerController> takeDamage;
    public static Action<PlayerController> EmptyHP;
    #endregion

    #region BASE_FUNCTIONS
    void Start()
    {
        cam = Camera.main; 
        jumpAmmount = noOfJumps;
        comboController.hitCol.SetActive(false);
        transform.position = new Vector3(InitialPos.x, InitialPos.y, InitialPos.z);
        InitialPos = cam.WorldToScreenPoint(transform.localPosition);
        parryController = GetComponentInChildren<ParryController>();
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
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
        if ((Physics2D.Raycast(transform.position, Vector2.right, distanceToWall, layerWallR) || Physics2D.Raycast(transform.position, Vector2.left, distanceToWall, layerWallL)) && !isGrounded)
        {
            canWallJump = false;
            jumpAmmount = 2;
            isInWall = true;
        }

        else isInWall = false;
        if (isGrounded && !wasGrounded)
        {
            ResetWallJump();
            anim.ResetTrigger("Jump");
            canWallJump = true;
            wasGrounded = true;
            jumpAmmount = noOfJumps;
        }
        if (!canDash)
        {
            dashCooldown += Time.deltaTime;
            if(dashCooldown > 2)
            {
                dashCooldown = 0;
                canDash = true;
            } 
        }
        if (isDashing)
        {
            dashDuration += Time.deltaTime;
            if(dashDuration > 0.15f)
            {
                dashDuration = 0;
                if (isGrounded) state = State.Grounded;
                else state = State.Falling;
            }
        }
        if (isGrounded && !jumped && !isDashing) state = State.Grounded;
        else if (jumped && !isDashing) state = State.Jumping;
        else if (!isGrounded && isInWall) state = State.InWall;
        else if (rigidBody.velocity.y < wallStickiness && !isInWall && !isGrounded && !isDashing) state = State.Falling;
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
                    isDashing = false;
                    jumpInWall = false;
                    ResetWallJump();
                    if (LastDirection > 0)
                    {
                        player.transform.eulerAngles = new Vector3(0, 0, 0);
                        lastDashVelocity = 1;
                    }
                    else if (LastDirection < 0)
                    {
                        player.transform.eulerAngles = new Vector3(0, 180, 0);
                        lastDashVelocity = -1;
                    }
                    if (canMove)
                    {
                        jumpAmmount = 2;
                        rigidBody.velocity = new Vector2(movement.x, rigidBody.velocity.y);
                    }
                    break;

                case State.Jumping:
                    if (LastDirection > 0)
                    {
                        player.transform.eulerAngles = new Vector3(0, 0, 0);
                        lastDashVelocity = 1;
                    }
                    else if (LastDirection < 0)
                    {
                        player.transform.eulerAngles = new Vector3(0, 180, 0);
                        lastDashVelocity = -1;
                    }
                    anim.SetTrigger("Jump");
                    lastVelocity = 0;
                    rigidBody.velocity = new Vector2(movement.x, jumpForce);
                    jumped = false;
                    state = State.Falling;
                    break;

                case State.Falling:
                    jumpInWall = false;
                    ResetWallJump();
                    if (LastDirection > 0)
                    {
                        player.transform.eulerAngles = new Vector3(0, 0, 0);
                        lastDashVelocity = 1;
                    }
                    else if (LastDirection < 0)
                    {
                        player.transform.eulerAngles = new Vector3(0, 180, 0);
                        lastDashVelocity = -1;
                    }
                    rigidBody.velocity = new Vector2(movement.x + lastVelocity, rigidBody.velocity.y);
                    if (Mathf.Abs(rigidBody.velocity.x) >= speed)
                        rigidBody.velocity = new Vector2(speed * Mathf.Sign(rigidBody.velocity.x), rigidBody.velocity.y);
                    break;

                case State.InWall:
                    anim.ResetTrigger("Jump");
                    rigidBody.velocity = new Vector2(0, wallStickiness); // limit movement to the right side
                    StartCoroutine(WallSlideTransition(0.1403281f));
                    if (jumpInWall && jumpAmmount>0)
                    {
                        StartCoroutine(WallJumpCoolDown(0.2f));
                    }
                    break;

                case State.Dash:
                    rigidBody.velocity = new Vector2(jumpForce * 3 * Mathf.Sign(lastDashVelocity), 0);
                    lastVelocity = rigidBody.velocity.x;
                    isDashing = true;
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
        if(Input.GetKeyDown(dashButtonKM) || Input.GetKeyDown(dashButtonJoystick) && !isInWall && canDash)
        {
            canDash = false;
            isDashing = true;
            anim.SetTrigger("Dash");
            state = State.Dash;
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
        rigidBody.velocity = Vector2.zero;
        hp = 100;
        isDead = false;
        isDashing = false;
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
        if (collision.gameObject.CompareTag("HitCollider") && collision.gameObject.layer != gameObject.layer && !parryController.GetBlockDamage() && !isDashing && collision.gameObject != gameObject)
        {
            isCriticalHit = collision.gameObject.GetComponentInParent<CombatController>().GetCriticalDamageValue();
            Vector3 direction = collision.gameObject.transform.position - transform.position;
            direction.Normalize();
            Debug.Log(direction);
            anim.SetTrigger("Damage");
            StartCoroutine(TakeDamage(0.75f));
            hp -= collision.gameObject.GetComponentInParent<CombatController>().GetDamage();
            if (isCriticalHit)
            {
                rigidBody.AddForce(new Vector2(-direction.x * criticalKnockBackForce, rigidBody.velocity.y));
                StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
            }
            else
            {
                rigidBody.AddForce(new Vector2(-direction.x * knockBackForce, rigidBody.velocity.y));
            }
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
        ResetWallJump();
        if (leftOrRighWall)
        {
            LastDirection = 1;
            rigidBody.velocity = new Vector2(jumpForce / 2, jumpForce);
        }
        else
        {
            LastDirection = -1;
            rigidBody.velocity = new Vector2(-jumpForce / 2, jumpForce);
        }
        lastVelocity = rigidBody.velocity.x;
        yield return new WaitForSeconds(0.1f);
        state = State.Falling;
        wj = false;
        jumpInWall = false;
        yield return null;
    }
    IEnumerator TakeDamage(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        yield return null;
    }
    IEnumerator CriticalHit() // Complete when i download the animation
    {
        //animacion de critico
        yield return null;
    }
    #endregion COROUTINES

}
