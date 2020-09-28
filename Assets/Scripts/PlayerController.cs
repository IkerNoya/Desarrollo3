using System;
using System.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float wallStickiness;
    [SerializeField] float jumpForce;
    [SerializeField] float shakeDuration; 
    [SerializeField] float shakeMagnitude;
    [SerializeField] float hpShakeDuration;
    [SerializeField] float hpShakeMagnitude;
    [SerializeField] float distanceToGround = 0.01f;
    [SerializeField] float distanceToWall = 0.01f;
    [SerializeField] string[] animNames;
    [SerializeField] string playerAxis;
    [SerializeField] string joystickAxis;
    [SerializeField] int NoOfJumps;
    [SerializeField] int damage;
    [SerializeField] KeyCode attackButtonKM;
    [SerializeField] KeyCode attackButtonJoystick;
    [SerializeField] KeyCode jumpButtonKM;
    [SerializeField] KeyCode jumpButtonJoystick;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] ComboController comboController;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] CameraShake HealthBarShake;
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] Vector3 InitialPos;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask layerWallL;
    [SerializeField] LayerMask layerWallR;
    public enum PlayerSelect
    {
        player1, player2
    }
    public enum State
    {
        Grounded, Jumping, Falling, InWall, WallJump
    }
    public PlayerSelect playerSelect;
    State state;

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

    int hp = 100;
    int jumpAmmount;

    float direction;
    float LastDirection;
    float lastVelocity;
    readonly float runAxisLimit = 0.10f;

    Vector2 movement;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        jumpAmmount = NoOfJumps;
        comboController.hitCol.SetActive(false);
        transform.position = new Vector3(InitialPos.x, InitialPos.y,InitialPos.z);
        InitialPos = cam.WorldToScreenPoint(transform.localPosition);
        wallJump = true;
    }

    void Update()
    {
        if (isDead)
            return;
        if (canMove)
        {
            direction = Input.GetAxis(playerAxis) + Input.GetAxis(joystickAxis);
            movement = new Vector2(direction, 0) * speed;
        }
        Inputs();
        if (!comboController.isAttacking)
        {
            MovementAnimations();
        }
        if (direction != 0)
        {
            LastDirection = direction;
        }
        if (LastDirection > 0)
        { 
            player.transform.eulerAngles = new Vector3(0,0,0);
        }
        else
        {
            player.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
        if(direction > 0 && wallJump)
        {
            isInWall = Physics2D.Raycast(transform.position, Vector2.right, distanceToWall, layerWallR);
        }
        else if(direction < 0 && wallJump)
        {
            isInWall = Physics2D.Raycast(transform.position, Vector2.left, distanceToWall, layerWallL);
        }
        else if (!wallJump)
        {
            isInWall = false;
        }
        if (isGrounded && !wasGrounded)
        {
            wasGrounded = true;
            comboController.canAttack = true;
            jumpAmmount = NoOfJumps;
            canWallJump = true;
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
        }
        if (isGrounded && !jumped) state = State.Grounded;
        else if (jumped) state = State.Jumping;
        else if (!isGrounded && isInWall) state = State.InWall;
        wasGrounded = isGrounded;
        healthBar.fillAmount = HitPercentage(hp, 1f);
        Debug.Log(state);
    }
   
    void FixedUpdate()
    {
        if (canMove && rigidBody !=null )
        {
            switch (state)
            {
                case State.Grounded:
                    rigidBody.velocity = new Vector2(movement.x, rigidBody.velocity.y);
                    break;

                case State.Jumping:
                        lastVelocity = 0;
                        rigidBody.velocity = new Vector2(0, jumpForce);
                        jumped = false;
                        state = State.Falling;
                    break;

                case State.Falling:
                    rigidBody.velocity = new Vector2(movement.x + lastVelocity, rigidBody.velocity.y);
                    if (Mathf.Abs(rigidBody.velocity.x) >= speed)
                        rigidBody.velocity = new Vector2(speed * Mathf.Sign(rigidBody.velocity.x), rigidBody.velocity.y);
                    break;

                case State.InWall:
                    rigidBody.velocity = new Vector2(movement.x, wallStickiness);
                    if (jumpInWall)
                    {
                        StartCoroutine(WallJumpCoolDown());
                    }
                    break;
            }
        }
        
    }
    void Inputs()
    {
        if ((Input.GetKeyDown(attackButtonKM) || Input.GetKeyDown(attackButtonJoystick)) && isGrounded && comboController.canAttack)
        {
            comboController.isAttacking = true;
            canMove = false;
            rigidBody.velocity = Vector2.zero;
            comboController.StartCombo();
        }
        if ((Input.GetKeyDown(jumpButtonKM) || Input.GetKeyDown(jumpButtonJoystick)) && jumpAmmount > 0 && canMove && !comboController.isAttacking)
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        if((Input.GetKeyDown(jumpButtonKM) || Input.GetKeyDown(jumpButtonJoystick)) && isInWall)
        {
            jumpInWall = true;
        }
    }
    void MovementAnimations()
    {
        if ((direction > runAxisLimit || direction < -runAxisLimit) && !comboController.isAttacking && isGrounded && canMove)
        {
            ActivateAnim("IsRunning");
        }
        if(direction < runAxisLimit && direction > -runAxisLimit && isGrounded)
        {
            ActivateAnim("Idle");
        }
        if (rigidBody.velocity.y > 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsJumping");
            comboController.canAttack = false;
        }
        if(rigidBody.velocity.y < 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsFalling");
            comboController.canAttack = false;
        }

    }

    void ActivateAnim(string name)
    {
        for(int i = 0; i < animNames.Length; i++)
        {
            if (animNames[i] == name)
            {
                anim.SetBool(animNames[i], true);
            }
            else
            {
                anim.SetBool(animNames[i], false);
            }
        }
    }
    void Dead()
    {
        isDead = true;
        anim.SetBool("Dead", true);
        rigidBody.isKinematic = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(slowMotion.ActivateSlowMotion(1.5f, 0.5f));
    }
    float HitPercentage(int damage, float barSize)
    {
        float maxPercentage = 100;
        float result = barSize * damage / maxPercentage;
        return result;
    }
    void Respawn()
    {
        rigidBody.isKinematic = false;
        hp = 100;
        isDead = false;
        transform.position = cam.ScreenToWorldPoint(new Vector3(InitialPos.x, InitialPos.y, InitialPos.z));
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        anim.SetBool("Dead", false);
    }
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OutofBounds"))
        {
            hp = 0;
            healthBar.fillAmount = 0f;
            StartCoroutine(HealthBarShake.Shake(hpShakeDuration, hpShakeMagnitude));
            Dead();
            StartCoroutine(RespawnPlayer());
        }
        if (collision.gameObject.CompareTag("Walls"))
        {
            if (!isGrounded && canWallJump)
            {
                jumpAmmount = 2;
                canWallJump = false;
            }
            if(transform.position.x > collision.transform.position.x)
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
        if (collision.gameObject.CompareTag("HitCollider"))
        {
            anim.SetBool("Hit", true);
            canMove = false;
            hp -= damage;
            StartCoroutine(HitCooldown());
            StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
            StartCoroutine(HealthBarShake.Shake(hpShakeDuration, hpShakeMagnitude));
            if (hp <= 0)
            {
                 Dead();
                 healthBar.fillAmount = 0f;
                 StartCoroutine(RespawnPlayer());
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            comboController.canAttack = false;
        }
        if (collision.gameObject.CompareTag("Walls"))
        {
            state = State.Falling;
        }
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
        anim.SetBool("Hit", false);
        yield return null;
    }
    IEnumerator WallJumpCoolDown()
    {
        wallJump = false;
        if (jumpAmmount > 0)
        {
            if (leftOrRighWall)
            {
                rigidBody.velocity = new Vector2(jumpForce / 2 + movement.x, jumpForce);
                LastDirection = 1;
            }
            else
            {
                rigidBody.velocity = new Vector2(-jumpForce/2 + movement.x, jumpForce);
                LastDirection = -1;
            }
            lastVelocity = rigidBody.velocity.x;
        }
        yield return new WaitForSeconds(0.2f);
        state = State.Falling;
        jumpInWall = false;
        wallJump = true;
        yield return null;
    }
}
