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

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    bool isJumping=false;
    bool isAttacking = false;

    float lastHit;
    float direction;
    float LastDirection;
    float startRunAnim = 0.01f;

    Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.flipX=false;
    }

    void Update()
    {
        direction = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.F))
        {
            isAttacking = true;
            StartCombo();
        }
        if(!isAttacking)
        {
            MovementAnimation();
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
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce);
            isJumping = true;
        }
    }
    void MovementAnimation() // Probar despues estados con un switch
    {
        if ((direction > startRunAnim || direction < -startRunAnim) && !isAttacking)
        {
            ActivateAnim("IsRunning");
        }
        if(direction < startRunAnim && direction > -startRunAnim)
        {
            ActivateAnim("Idle");
        }
        if (isJumping)
        {
            ActivateAnim("IsJumping");
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
            isJumping = false;
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
