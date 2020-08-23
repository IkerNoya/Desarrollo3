using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{   
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] string[] animNames;
    [SerializeField] float hitCooldown;
    [SerializeField] int hitCounter;
    [SerializeField] int NoOfJumps;
    [SerializeField] int hp;
    [SerializeField] int lives;
    [SerializeField] int damage;
    [SerializeField] float joystickAxis;
    [SerializeField] GameObject hitCol;
    [SerializeField] Vector2 InitialPos;

    public enum PlayerSelect
    {
        player1, player2
    }
    public PlayerSelect playerSelect;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    bool isGrounded=false;
    bool jumped=false;
    bool isAttacking = false;
    bool canAttack = true;
    bool isDead = false;
    bool canMove = true;

    int jumpAmmount;

    float lastHit;
    float direction;
    float LastDirection;
    float startRunAnim = 0.0001f;

    Vector3 movement;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        if (playerSelect == PlayerSelect.player1 && canMove)
        {
            if (Input.GetAxis("P1_Horizontal") >= 0 || Input.GetAxis("P1_Horizontal") <= 0)
                direction = Input.GetAxis("P1_Horizontal");
            //else if (Input.GetAxis("P1_Joystick_Horizontal") > joystickAxis || Input.GetAxis("P1_Joystick_Horizontal") < -joystickAxis) // por si el control esta rotito
            //{
            //    direction = Input.GetAxis("P1_Joystick_Horizontal");
            //    Debug.Log(Input.GetAxis("P1_Joystick_Horizontal"));
            //    if (Input.GetAxis("P1_Joystick_Horizontal") < 0.3f && Input.GetAxis("P1_Joystick_Horizontal") > -0.3f)
            //    {
            //        direction = 0;
            //    }
            //}
        }
        else if (playerSelect == PlayerSelect.player2 && canMove)
        {
            if (Input.GetAxis("P2_Horizontal") >= 0 || Input.GetAxis("P2_Horizontal") <= 0)
                direction = Input.GetAxis("P2_Horizontal");
            //else if (Input.GetAxis("P2_Joystick_Horizontal") > joystickAxis || Input.GetAxis("P2_Joystick_Horizontal") < -joystickAxis) // por si el control esta rotito
            //{
            //    direction = Input.GetAxis("P2_Joystick_Horizontal");
            //    if (Input.GetAxis("P2_Joystick_Horizontal") < 0.3f && Input.GetAxis("P2_Joystick_Horizontal") > -0.3f)
            //    {
            //        direction = 0;
            //    }
            //}
        }

        if (Input.GetKeyDown(KeyCode.F) && isGrounded && playerSelect == PlayerSelect.player1 && canAttack)
        {
            isAttacking = true;
            StartCombo();
        }
        else if(Input.GetKeyDown(KeyCode.J) && isGrounded && playerSelect == PlayerSelect.player2 && canAttack)
        {
            isAttacking = true;
            StartCombo();
        }
        if(!isAttacking)
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
    void StartCombo()
    {
        if(Time.time - lastHit > hitCooldown)
        {
            isAttacking = false;
            hitCounter = 0;
        }

        lastHit = Time.time;
        hitCounter++;
        if (hitCounter == 1)
        {
            anim.SetBool("IsAttacking_1", true);
        }
        hitCounter = Mathf.Clamp(hitCounter, 0, 3); 
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
        if (Input.GetKeyDown(KeyCode.Space) && jumpAmmount > 0 && playerSelect == PlayerSelect.player1 && canMove)
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        else if (Input.GetKeyDown(KeyCode.RightAlt) && jumpAmmount > 0 && playerSelect == PlayerSelect.player2 && canMove)
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        if (rb.velocity.y > 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsJumping");
        }
        if(rb.velocity.y < 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsFalling");
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
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
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

    // Animation Events
    public void Attack1()
    {
        hitCol.SetActive(false);
        if (hitCounter >= 2)
        {
            anim.SetBool("IsAttacking_2", true);
        }
        else
        {
            anim.SetBool("IsAttacking_1", false);
            hitCounter = 0;
            isAttacking = false;
        }
    }
    public void Attack2()
    {
        hitCol.SetActive(false);
        if (hitCounter >= 3)
        {
            anim.SetBool("IsAttacking_3", true);
        }
        else
        {
            anim.SetBool("IsAttacking_2", false);
            hitCounter = 0;
            isAttacking = false;
        }
    }
    public void Attack3()
    {
        hitCol.SetActive(false);
        anim.SetBool("IsAttacking_1", false);
        anim.SetBool("IsAttacking_2", false);
        anim.SetBool("IsAttacking_3", false);
        isAttacking = false;
        hitCounter = 0;
    }
    public void ActivateHit()
    {
        hitCol.SetActive(true);
    }
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
        yield return null;
    }
}
