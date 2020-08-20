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

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    bool isGrounded=true;
    bool jumped=false;
    bool isAttacking = false;

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
    }

    void Update()
    {
        if (gameObject.CompareTag("Player_1"))
            direction = Input.GetAxis("P1_Horizontal");
        else if (gameObject.CompareTag("Player_2"))
            direction = Input.GetAxis("P2_Horizontal");

        if (Input.GetKeyDown(KeyCode.F) && isGrounded && gameObject.CompareTag("Player_1"))
        {
            isAttacking = true;
            StartCombo();
        }
        else if(Input.GetKeyDown(KeyCode.J) && isGrounded && gameObject.CompareTag("Player_2")) // control auxiliar, cambiar despues
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
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
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
        if ((direction > startRunAnim || direction < -startRunAnim) && !isAttacking && isGrounded)
        {
            ActivateAnim("IsRunning");
        }
        if(direction < startRunAnim && direction > -startRunAnim && isGrounded)
        {
            ActivateAnim("Idle");
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpAmmount > 0 && gameObject.CompareTag("Player_1"))
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        else if (Input.GetKeyDown(KeyCode.RightAlt) && jumpAmmount > 0 && gameObject.CompareTag("Player_2"))
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        if (rb.velocity.y > 0 && !isGrounded)
        {
            ActivateAnim("IsJumping");
        }
        if(rb.velocity.y < 0 && !isGrounded)
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
    }

    // Animation Events
    public void Attack1()
    {
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
        anim.SetBool("IsAttacking_1", false);
        anim.SetBool("IsAttacking_2", false);
        anim.SetBool("IsAttacking_3", false);
        isAttacking = false;
        hitCounter = 0;
    }
}
