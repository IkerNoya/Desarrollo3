using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] string[] animNames;
    [SerializeField] int NoOfJumps;
    [SerializeField] int damage;
    [SerializeField] string playerAxis;
    [SerializeField] KeyCode attackButton;
    [SerializeField] KeyCode jumpButton;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject player;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] ComboController comboController;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] CameraShake HealthBarShake;
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] float shakeDuration; 
    [SerializeField] float shakeMagnitude;
    [SerializeField] float hpShakeDuration;
    [SerializeField] float hpShakeMagnitude;
    [SerializeField] Vector3 InitialPos;
    [SerializeField] BoxCollider2D playerCollider;
    [SerializeField] float distanceToGround = 0.01f;
    [SerializeField] LayerMask groundLayer;
    public enum PlayerSelect
    {
        player1, player2
    }
    public PlayerSelect playerSelect;

    public Animator anim;

    bool isGrounded = false;
    bool  wasGrounded = false;
    bool jumped = false;
    bool isDead = false;
    bool canMove = true;
    bool canWallJump = true;

    int hp = 100;
    int jumpAmmount;

    float direction;
    float LastDirection;
    float startRunAnim = 0.0001f;

    Vector3 movement;

    public delegate void EndGame();
    public static event EndGame endGame;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        jumpAmmount = NoOfJumps;
        comboController.hitCol.SetActive(false);
        transform.position = new Vector3(InitialPos.x, InitialPos.y,InitialPos.z);
        InitialPos = cam.WorldToScreenPoint(transform.localPosition);
    }

    void Update()
    {
        Debug.Log(playerCollider.bounds.min.y);
        if (isDead)
            return;
        if (canMove)
            direction = Input.GetAxis(playerAxis);
        if (Input.GetKeyDown(attackButton) && isGrounded && comboController.canAttack)
        {
            comboController.isAttacking = true;
            comboController.StartCombo();
        }
        if (!comboController.isAttacking)
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
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
        if (isGrounded && !wasGrounded)
        {
            wasGrounded = true;
            Debug.DrawRay(transform.position, Vector2.down, Color.red);
            comboController.canAttack = true;
            jumpAmmount = NoOfJumps;
            canWallJump = true;
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
        }
        wasGrounded = isGrounded;
        healthBar.fillAmount = HitPercentage(hp, 1f);
    }
   
    void FixedUpdate()
    {
        if (jumped)
        {
            jumped = false;
            rigidBody.velocity = new Vector2(0, jumpForce) * Time.fixedDeltaTime;
        }
    }
    void MovementAnimations()
    {
        if ((direction > startRunAnim || direction < -startRunAnim) && !comboController.isAttacking && isGrounded && canMove)
        {
            ActivateAnim("IsRunning");
        }
        if(direction < startRunAnim && direction > -startRunAnim && isGrounded)
        {
            ActivateAnim("Idle");
        }
        if (Input.GetKeyDown(jumpButton) && jumpAmmount > 0 && canMove && !comboController.isAttacking)
        {
            isGrounded = false;
            jumped = true;
            jumpAmmount--;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("OutofBounds"))
        {
            hp = 0;
            healthBar.fillAmount = 0f;
            StartCoroutine(HealthBarShake.Shake(hpShakeDuration, hpShakeMagnitude));
            Dead();
            StartCoroutine(RespawnPlayer());
        }
        if (collision.collider.CompareTag("Walls") && !isGrounded && canWallJump)
        {
            jumpAmmount = 2;
            canWallJump = false;
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
        if (collision.gameObject.CompareTag("Crown"))
        {
            Destroy(collision.gameObject);
            StartCoroutine(EndEvent());
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            comboController.canAttack = false;
        }
    }
    IEnumerator EndEvent()
    {
        StartCoroutine(slowMotion.ActivateSlowMotion(1.5f, 0.5f));
        yield return new WaitForSeconds(1.5f);
        endGame();
        yield break;
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
}
