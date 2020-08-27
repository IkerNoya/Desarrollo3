using System.Collections;
using UnityEngine;

public class PlayerController : ComboController
{   
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] string[] animNames;
    [SerializeField] int NoOfJumps;
    [SerializeField] int hp;
    [SerializeField] int lives;
    [SerializeField] int damage;
    [SerializeField] float joystickAxis;
    [SerializeField] Vector2 InitialPos;
    [SerializeField] string playerAxis;
    [SerializeField] KeyCode attackButton;
    [SerializeField] KeyCode jumpButton;

    public enum PlayerSelect
    {
        player1, player2
    }
    public PlayerSelect playerSelect;

    Rigidbody2D rb;
    SpriteRenderer sr;

    bool isGrounded=false;
    bool jumped=false;
    bool isDead = false;
    bool canMove = true;

    int jumpAmmount;

    float direction;
    float LastDirection;
    float startRunAnim = 0.0001f;

    Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.flipX=false;
        jumpAmmount = NoOfJumps;
        hitCol.SetActive(false);
        transform.position = new Vector3(InitialPos.x, InitialPos.y,0);
    }

    void Update()
    {
        if (isDead)
            return;
        if (canMove)
            direction = Input.GetAxis(playerAxis);
        if (Input.GetKeyDown(attackButton) && isGrounded && canAttack)
        {
            isAttacking = true;
            StartCombo();
        }
        if (!isAttacking)
        {
            MovementAnimations();
            movement = new Vector3(direction, 0, 0) * Time.deltaTime;
            transform.position += movement * speed;
        }
        if (direction != 0)
        {
            LastDirection = direction;
        }
        if (LastDirection > 0)
        { 
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
   
    void FixedUpdate()
    {
        if (jumped)
        {
            jumped = false;
            rb.velocity = new Vector2(0, jumpForce) * Time.fixedDeltaTime;
        }
    }
    void MovementAnimations()
    {
        if ((direction > startRunAnim || direction < -startRunAnim) && !isAttacking && isGrounded && canMove)
        {
            ActivateAnim("IsRunning");
        }
        if(direction < startRunAnim && direction > -startRunAnim && isGrounded)
        {
            ActivateAnim("Idle");
        }
        if (Input.GetKeyDown(jumpButton) && jumpAmmount > 0 && playerSelect == PlayerSelect.player1 && canMove && !isAttacking)
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        if (rb.velocity.y > 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsJumping");
            canAttack = false;
        }
        if(rb.velocity.y < 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsFalling");
            canAttack = false;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            canAttack = true;
            jumpAmmount = NoOfJumps;
            anim.SetBool("IsJumping", false);
        }
        if (collision.collider.CompareTag("HitCollider"))
        {
            anim.SetBool("Hit",true);
            canMove = false;
            hp -= damage;
            StartCoroutine(HitCooldown());
            if (hp <= 0)
            {
                Dead();
                StartCoroutine(RespawnPlayer());
            }
        }
        if (collision.collider.CompareTag("OutofBounds"))
        {
            hp = 0;
            Dead();
            StartCoroutine(RespawnPlayer());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            canAttack = false;
        }
    }
    void Dead()
    {
        isDead = true;
        anim.SetBool("Dead", true);
        rb.isKinematic = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(SlowMotion());
    }
    IEnumerator SlowMotion()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 1;
        StopCoroutine(SlowMotion());
    }
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2.0f);
        Respawn();
        StopCoroutine(RespawnPlayer());
    }
    void Respawn()
    {
        rb.isKinematic = false;
        hp = 100;
        isDead = false;
        lives--;
        transform.position = new Vector3(InitialPos.x, InitialPos.y, 0);
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        anim.SetBool("Dead", false);
    }
    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        anim.SetBool("Hit", false);
        yield return null;
    }
}
