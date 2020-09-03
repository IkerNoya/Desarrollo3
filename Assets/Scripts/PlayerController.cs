using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] string[] animNames;
    [SerializeField] int NoOfJumps;
    [SerializeField] int lives;
    [SerializeField] int damage;
    [SerializeField] Vector2 InitialPos;
    [SerializeField] string playerAxis;
    [SerializeField] KeyCode attackButton;
    [SerializeField] KeyCode jumpButton;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] ComboController cc;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeMagnitude;
    public enum PlayerSelect
    {
        player1, player2
    }
    public PlayerSelect playerSelect;


    bool isGrounded=false;
    bool jumped=false;
    bool isDead = false;
    bool canMove = true;

    int hp =     100;
    int jumpAmmount;

    float direction;
    float LastDirection;
    float startRunAnim = 0.0001f;

    Vector3 movement;
    Vector3 healthBarSize;
    Vector3 initialHealthBarSize;

    public delegate void OutOfLives();
    public static event OutOfLives playerIsDead;

    void Start()
    {
        jumpAmmount = NoOfJumps;
        cc.hitCol.SetActive(false);
        transform.position = new Vector3(InitialPos.x, InitialPos.y,0);
        healthBarSize = healthBar.rectTransform.localScale;
        initialHealthBarSize = healthBar.rectTransform.localScale;
    }

    void Update()
    {
        if (isDead)
            return;
        if (canMove)
            direction = Input.GetAxis(playerAxis);
        if (Input.GetKeyDown(attackButton) && isGrounded && cc.canAttack)
        {
            cc.isAttacking = true;
            cc.StartCombo();
        }
        if (!cc.isAttacking)
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
            player.transform.eulerAngles = new Vector3(0,0,0);
        }
        else
        {
            player.transform.eulerAngles = new Vector3(0, 180, 0);
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
        if ((direction > startRunAnim || direction < -startRunAnim) && !cc.isAttacking && isGrounded && canMove)
        {
            ActivateAnim("IsRunning");
        }
        if(direction < startRunAnim && direction > -startRunAnim && isGrounded)
        {
            ActivateAnim("Idle");
        }
        if (Input.GetKeyDown(jumpButton) && jumpAmmount > 0 && canMove && !cc.isAttacking)
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
        }
        if (rb.velocity.y > 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsJumping");
            cc.canAttack = false;
        }
        if(rb.velocity.y < 0 && !isGrounded && canMove)
        {
            ActivateAnim("IsFalling");
            cc.canAttack = false;
        }

    }

    void ActivateAnim(string name)
    {
        for(int i = 0; i < animNames.Length; i++)
        {
            if (animNames[i] == name)
            {
                cc.anim.SetBool(animNames[i], true);
            }
            else
            {
                cc.anim.SetBool(animNames[i], false);
            }
        }
    }
    void Dead()
    {
        isDead = true;
        cc.anim.SetBool("Dead", true);
        rb.isKinematic = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(SlowMotion());
    }
    float hitPercentage(int damage, int barSize)
    {
        float maxPercentage = 100;
        float result = barSize * damage / maxPercentage;
        return result;
    }
    void Respawn()
    {
        rb.isKinematic = false;
        hp = 100;
        isDead = false;
        lives--;
        transform.position = new Vector3(InitialPos.x, InitialPos.y, 0);
        healthBarSize = initialHealthBarSize;
        healthBar.rectTransform.localScale = healthBarSize;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        cc.anim.SetBool("Dead", false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            cc.canAttack = true;
            jumpAmmount = NoOfJumps;
            cc.anim.SetBool("IsJumping", false);
        }
        if (collision.collider.CompareTag("OutofBounds"))
        {
            hp = 0;
            Dead();
            StartCoroutine(RespawnPlayer());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitCollider"))
        {
            cc.anim.SetBool("Hit", true);
            healthBarSize.x -= hitPercentage(damage, 1);
            healthBar.rectTransform.localScale = healthBarSize;
            canMove = false;
            hp -= damage;
            StartCoroutine(HitCooldown());
            StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
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
            cc.canAttack = false;
        }
    }
    IEnumerator LooseEvent()
    {
        StartCoroutine(SlowMotion());
        yield return new WaitForSeconds(1.5f);
        playerIsDead();
        StopCoroutine(LooseEvent());
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
    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        cc.anim.SetBool("Hit", false);
        yield return null;
    }
}
