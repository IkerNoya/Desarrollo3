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
    Rigidbody2D rb;
    SpriteRenderer sr;
    [SerializeField] BoxCollider2D col;
    Animator anim;
    bool isJumping=false;
    bool isRunning=false;
    bool isFalling=false;
    float distanceToGround;
    Vector3 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        distanceToGround = col.bounds.extents.y;
    }

    // Codigo de testeo
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            ActivateAnim("IsRunning_R");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            ActivateAnim("IsRunning_L");
        }
        else
        {
            ActivateAnim("Idle");
        }
        if (isJumping)
        {
            ActivateAnim("IsJumping");
        }
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, 0) * Time.deltaTime;
        transform.position += movement * speed;
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce);
            isJumping = true;
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
        } 
    }
}
