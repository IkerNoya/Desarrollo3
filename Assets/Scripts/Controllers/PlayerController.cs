using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region SERIALIZED_VARIABLES
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
    [SerializeField] SpriteRenderer sprite;
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
    [Space]
    [SerializeField] float footstepSoundTimer;
    [Space]
    [SerializeField] GameObject spawner;
    [Space]
    [SerializeField] AnimationClip startRun;
    [SerializeField] AnimationClip endRun;
    [Space]
    AnimatorOverrideController overrider;
    #endregion

    #region PUBLIC_VARIABLES
    public AK.Wwise.Event footstepSound;
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
    public AK.Wwise.Event dashSound;
    public AK.Wwise.Event jumpSound;
    public CombatController comboController;
    [Space]
    public PlayerSelect playerSelect;
    State state;
    [Space]
    public Animator anim;
    #endregion

    #region PRIVATE_VARIABLES
    bool isGroundedCenter = false;
    bool isGroundedLeft = false;
    bool isGroundedRight = false;
    bool wasGrounded = false;
    bool jumped = false;
    bool isDead = false;
    bool canMove = true;
    bool isInWall = false;
    bool jumpInWall = false;
    bool leftOrRighWall = false; // true = left, false = right;
    bool wj = false;
    bool isCriticalHit;
    bool canDash = true;
    bool isDashing = false;
    bool isPaused = false;
    float walljumpAnimTime = 0.1403281f;

    ParryController parryController;
    BoxCollider2D collider;

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
    float soundTimer = 0;
    float initialGravity;

    Vector2 movement;
    Vector3 leftColliderSide;
    Vector3 rightColliderSide;

    Camera cam;
    DataManager data;
    GameManager manager;
    #endregion

    #region Actions
    public static Action<PlayerController> takeDamage;
    public static Action<PlayerController> EmptyHP;
    public static Action<PlayerController> Pause;
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
        collider = GetComponent<BoxCollider2D>();
        data = DataManager.Instance;
        LoadSelectionData();
        overrider = comboController.overrider;
        initialGravity = rigidBody.gravityScale;
        manager = GameManager.Get();
    }

    void Update()
    {
        if (isDead)
            return;
        if (isPaused)
            return;
        rightColliderSide = new Vector3(transform.position.x + collider.bounds.size.x / 2, transform.position.y, transform.position.z);
        leftColliderSide = new Vector3(transform.position.x - collider.bounds.size.x / 2, transform.position.y, transform.position.z);
        if (canMove)
        {
            direction = Input.GetAxis(playerAxis) + Input.GetAxis(joystickAxis);
            movement = new Vector2(direction, 0) * speed;
            anim.SetFloat("VelocityX", Mathf.Abs(direction));
            anim.SetFloat("Speed", Mathf.Abs(direction));
            if (Mathf.Abs(rigidBody.velocity.x) < 1f)
                overrider["Start_Run"] = startRun;
            else if (Mathf.Abs(rigidBody.velocity.x) >= 1 && Mathf.Abs(direction) <= 0.6f)
                overrider["Start_Run"] = endRun;
        }
        else
        {
            direction = 0;
            movement = Vector3.zero;
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
        }

        if (movement.x > runAxisLimit || movement.x < -runAxisLimit)
        {
            if (soundTimer >= footstepSoundTimer && isGroundedCenter)
            {
                footstepSound.Post(gameObject);
                soundTimer = 0;
            }
        }
        Inputs();
        if (direction != 0)
        {
            LastDirection = direction;
        }
        isGroundedCenter = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
        isGroundedRight = Physics2D.Raycast(rightColliderSide, Vector2.down, distanceToGround, groundLayer);
        isGroundedLeft = Physics2D.Raycast(leftColliderSide, Vector2.down, distanceToGround, groundLayer);
        if ((Physics2D.Raycast(transform.position, Vector2.right, distanceToWall, layerWallR) || Physics2D.Raycast(transform.position, Vector2.left, distanceToWall, layerWallL)) && (!isGroundedCenter && !isGroundedLeft && !isGroundedRight))
        {
            jumpAmmount = 2;
            isInWall = true;
        }

        else isInWall = false;
        if ((isGroundedCenter || isGroundedLeft || isGroundedRight) && !wasGrounded)
        {
            ResetWallJump();
            anim.ResetTrigger("Jump");
            wasGrounded = true;
            jumpAmmount = noOfJumps;
        }
        if (!canDash)
        {
            dashCooldown += Time.deltaTime;
            if (dashCooldown > 2)
            {
                dashCooldown = 0;
                canDash = true;
            }
        }
        if (isDashing)
        {
            dashDuration += Time.deltaTime;
            if (dashDuration > 0.15f)
            {
                dashDuration = 0;
                if (isGroundedCenter || isGroundedLeft || isGroundedRight) state = State.Grounded;
                else state = State.Falling;
            }
        }
        if ((isGroundedCenter || isGroundedLeft || isGroundedRight) && !jumped && !isDashing) state = State.Grounded;
        else if (jumped && !isDashing) state = State.Jumping;
        else if ((!isGroundedCenter && !isGroundedLeft && !isGroundedRight) && isInWall) state = State.InWall;
        else if (rigidBody.velocity.y < wallStickiness && !isInWall && (!isGroundedCenter && !isGroundedLeft && !isGroundedRight) && !isDashing) state = State.Falling;
        wasGrounded = isGroundedCenter;
        wasGrounded = isGroundedLeft;
        wasGrounded = isGroundedRight;
        if (isGroundedCenter || isGroundedLeft || isGroundedRight)
            anim.SetBool("Grounded", true);
        else if (!isGroundedCenter && !isGroundedLeft && !isGroundedRight)
            anim.SetBool("Grounded", false);
        if (anim.GetBool("Grounded") && Mathf.Abs(direction) <= 0.3f)
            anim.SetBool("Stop", true);
        else if (Mathf.Abs(direction) > 0.1f)
            anim.SetBool("Stop", false);
        else
            anim.SetBool("Stop", false);
        anim.SetFloat("VelocityY", rigidBody.velocity.y);

        if (wj) anim.SetTrigger("WJ");
        soundTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;
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
                    StartCoroutine(WallSlideTransition(walljumpAnimTime));
                    if (jumpInWall && jumpAmmount > 0)
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

    void LoadSelectionData()
    {
        if (playerSelect == PlayerSelect.player1)
        {
            switch (data.player1Choice.tint)
            {
                case DataManager.Tint.white:
                    sprite.color = Color.white;
                    break;
                case DataManager.Tint.red:
                    sprite.color = Color.red;
                    break;
                case DataManager.Tint.blue:
                    sprite.color = Color.blue;
                    break;
            }
            switch (data.player1Choice.playerSelection)
            {
                case DataManager.PlayerSelection.Nova:
                    startRun = Resources.Load("Animations/Clips/Start_Run_Nova") as AnimationClip;
                    endRun = Resources.Load("Animations/Clips/End_Run_Nova") as AnimationClip;
                    break;
                case DataManager.PlayerSelection.CyberBunny:
                    startRun = Resources.Load("Animations/Clips/Start_Run_Cyber") as AnimationClip;
                    endRun = Resources.Load("Animations/Clips/End_Run_Cyber") as AnimationClip;
                    break;
            }
        }
        else
        {
            switch (data.player2Choice.tint)
            {
                case DataManager.Tint.white:
                    sprite.color = Color.white;
                    break;
                case DataManager.Tint.red:
                    sprite.color = Color.red;
                    break;
                case DataManager.Tint.blue:
                    sprite.color = Color.blue;
                    break;
            }
            switch (data.player2Choice.playerSelection)
            {
                case DataManager.PlayerSelection.Nova:
                    startRun = Resources.Load("Animations/Clips/Start_Run_Nova") as AnimationClip;
                    endRun = Resources.Load("Animations/Clips/End_Run_Nova") as AnimationClip;
                    break;
                case DataManager.PlayerSelection.CyberBunny:
                    startRun = Resources.Load("Animations/Clips/Start_Run_Cyber") as AnimationClip;
                    endRun = Resources.Load("Animations/Clips/End_Run_Cyber") as AnimationClip;
                    break;
            }
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    void Inputs()
    {
        if ((Input.GetKeyDown(jumpButtonKM) || Input.GetKeyDown(jumpButtonJoystick)) && jumpAmmount > 0 && canMove && !comboController.IsAttacking)
        {
            state = State.Jumping;
            isGroundedCenter = false;
            isGroundedLeft = false;
            isGroundedRight = false;
            jumped = true;
            jumpAmmount--;
            jumpSound.Post(gameObject);
        }
        if ((Input.GetKeyDown(jumpButtonKM) || Input.GetKeyDown(jumpButtonJoystick)) && isInWall)
        {
            jumpInWall = true;
            jumpSound.Post(gameObject);
        }
        if ((Input.GetKeyDown(dashButtonJoystick) || Input.GetKeyDown(dashButtonKM)) && !isInWall && canDash)
        {
            canDash = false;
            isDashing = true;
            anim.SetTrigger("Dash");
            state = State.Dash;
            dashSound.Post(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            isPaused = true;
            Pause?.Invoke(this);
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
        anim.SetBool("Dead", true);
        rigidBody.simulated = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    void Respawn()
    {
        rigidBody.simulated = true;
        rigidBody.velocity = Vector2.zero;
        canMove = true;
        hp = 100;
        isDead = false;
        anim.Rebind();
        state = State.Falling;
        isDashing = false;
        transform.position = spawner.transform.position;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
    public bool GetCanMove()
    {
        return canMove;
    }
    public void SetGravity(float value)
    {
        rigidBody.gravityScale = value;
        rigidBody.velocity = Vector3.zero;
    }
    public void SetDefaultGravity()
    {
        rigidBody.gravityScale = initialGravity;
    }
    public bool GetGrounded()
    {
        if (isGroundedCenter && isGroundedLeft && isGroundedRight)
            return true;
        else if (isGroundedCenter && isGroundedLeft && !isGroundedRight)
            return true;
        else if (isGroundedCenter && !isGroundedLeft && isGroundedRight)
            return true;
        else if (!isGroundedCenter && !isGroundedLeft && !isGroundedRight)
            return false;
        else
            return false;
    }
    public bool GetPause()
    {
        return isPaused;
    }
    public void SetPause(bool value)
    {
        isPaused = value;
    }
    public void Heal(float value)
    {
        hp += value * Time.deltaTime;
    }
    #endregion

    #region COLLISIONS
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OutofBounds"))
        {
            hp = 0;
            switch (playerSelect)
            {
                case PlayerSelect.player1:
                    GameManager.instance.Player1DamageDone(100);
                    GameManager.instance.Player2DamageDone(-100);
                    break;
                case PlayerSelect.player2:
                    GameManager.instance.Player2DamageDone(100);
                    GameManager.instance.Player1DamageDone(-100);
                    break;
            }
            Dead();
            EmptyHP?.Invoke(this);
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
            canMove = false;
            Vector3 direction;
            hp -= collision.gameObject.GetComponentInParent<CombatController>().GetDamage();
            StartCoroutine(GravityChange(0.5f));
            anim.SetTrigger("Damage");
            if (!GetGrounded())
            {
                if (isCriticalHit)
                {
                    direction = collision.gameObject.transform.position - Vector3.down;
                    direction.Normalize();
                    rigidBody.gravityScale = initialGravity;
                    rigidBody.AddForce(new Vector2(rigidBody.velocity.x, direction.x * criticalKnockBackForce));
                    StartCoroutine(CameraShake.instance.Shake(Camera.main.gameObject, shakeDuration, shakeMagnitude));
                    anim.ResetTrigger("Damage");
                }
            }
            else
            {
                anim.SetBool("Critical", isCriticalHit);
                direction = collision.gameObject.transform.position - transform.position;
                direction.Normalize();
                if (isCriticalHit)
                {
                    rigidBody.AddForce(new Vector2(-direction.x * criticalKnockBackForce, rigidBody.velocity.y));
                    StartCoroutine(CameraShake.instance.Shake(Camera.main.gameObject, shakeDuration, shakeMagnitude));
                }
                else
                {
                    rigidBody.AddForce(new Vector2(-direction.x * knockBackForce, rigidBody.velocity.y));
                }
            }

            takeDamage?.Invoke(this);
            if (hp <= 0)
            {
                switch (playerSelect)
                {
                    case PlayerSelect.player1:
                        if (isCriticalHit)
                        {
                            manager.Player1DamageDone(collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 2);
                            manager.Player2DamageDone(-collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 2);
                        }
                        else
                        {
                            manager.Player1DamageDone(collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 4);
                            manager.Player2DamageDone(-collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 4);
                        }
                        break;
                    case PlayerSelect.player2:
                        if (isCriticalHit)
                        {
                            manager.Player2DamageDone(collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 2);
                            manager.Player1DamageDone(-collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 2);
                        }
                        else
                        {
                            manager.Player2DamageDone(collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 4);
                            manager.Player1DamageDone(-collision.gameObject.GetComponentInParent<CombatController>().GetDamage() * 4);
                        }
                        break;
                }
                Dead();
                EmptyHP?.Invoke(this);
                StartCoroutine(RespawnPlayer());
            }
            else
            {
                switch (playerSelect)
                {
                    case PlayerSelect.player1:
                        manager.Player1DamageDone(collision.gameObject.GetComponentInParent<CombatController>().GetDamage());
                        manager.Player2DamageDone(-collision.gameObject.GetComponentInParent<CombatController>().GetDamage());
                        break;
                    case PlayerSelect.player2:
                        manager.Player2DamageDone(collision.gameObject.GetComponentInParent<CombatController>().GetDamage());
                        manager.Player1DamageDone(-collision.gameObject.GetComponentInParent<CombatController>().GetDamage());
                        break;
                }
            }
        }
        else if (collision.gameObject.CompareTag("HabilityCollider") && collision.gameObject.layer != gameObject.layer && !parryController.GetBlockDamage() && !isDashing && collision.gameObject != gameObject)
        {
            canMove = false;
            Vector3 direction;
            hp -= collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage();
            StartCoroutine(GravityChange(0.5f));
            anim.SetTrigger("Damage");
            direction = collision.gameObject.transform.position - transform.position;
            direction.Normalize();
            rigidBody.AddForce(new Vector2(-direction.x * knockBackForce, rigidBody.velocity.y));
            StartCoroutine(CameraShake.instance.Shake(Camera.main.gameObject, shakeDuration, shakeMagnitude));
            takeDamage?.Invoke(this);
            if (hp <= 0)
            {
                switch (playerSelect)
                {
                    case PlayerSelect.player1:
                        manager.Player1DamageDone(collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage() * 2);
                        manager.Player2DamageDone(-collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage() * 2);
                        break;
                    case PlayerSelect.player2:
                        manager.Player2DamageDone(collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage() * 2);
                        manager.Player1DamageDone(-collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage() * 2);
                        break;
                }
                Dead();
                EmptyHP?.Invoke(this);
                StartCoroutine(RespawnPlayer());
            }
            else
            {
                switch (playerSelect)
                {
                    case PlayerSelect.player1:
                        manager.Player1DamageDone(collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage());
                        manager.Player2DamageDone(-collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage());
                        break;
                    case PlayerSelect.player2:
                        manager.Player2DamageDone(collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage());
                        manager.Player1DamageDone(-collision.gameObject.GetComponentInParent<HabilityController>().GetChargeDamage());
                        break;
                }
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGroundedCenter = false;
            isGroundedLeft = false;
            isGroundedRight = false;
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
    IEnumerator GravityChange(float time)
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.gravityScale = 0;
        yield return new WaitForSeconds(time);
        rigidBody.gravityScale = initialGravity;
        yield return null;
    }
    #endregion COROUTINES

}
