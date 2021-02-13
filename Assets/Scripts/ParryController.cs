using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ParryController : MonoBehaviour
{
    [SerializeField] KeyCode parryKeyKM;
    [SerializeField] KeyCode parryKeyJoystick;
    [Space]
    [SerializeField] float duration;
    [Space]
    [SerializeField] GameObject enemy;
    [SerializeField] SlowMotion slowMotion;
    [SerializeField] float parryCooldown;
    [SerializeField] ParticleSystem shockwave;

    HabilityController habilityController;

    public AK.Wwise.Event parrySound;

    PlayerController enemyValues;
    bool blockDamage = false;
    bool canParry = true;

    float canMoveTimerOffset = 0.3f; // multiplication applied in coroutine to delay movement
    float cooldownTimer = 0;

    PlayerController player;
    BoxCollider2D parryCol;
    Animator anim;
    CombatController cc;
    

    public static event Action<ParryController> parryEffect;
    void Start()
    {
        habilityController = GetComponent<HabilityController>();
        parryCol = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
        parryCol.enabled = false;
        enemyValues = enemy.GetComponent<PlayerController>();
        cc = GetComponent<CombatController>();
    }
    void Update()
    {
        if(cooldownTimer >= parryCooldown)
        {
            canParry = true;
        }
        if (!canParry)
        {
            cooldownTimer += Time.deltaTime;
            return;
        }
        ParryInput();
    }
    void ParryInput()
    {
        if ((Input.GetKey(parryKeyKM) || Input.GetKey(parryKeyJoystick)) && player.GetGrounded() && !cc.IsAttacking && canParry)
        {
            parryCol.enabled = true;
            anim.SetBool("Block", true);
            player.SetCanMove(false);
            blockDamage = true;
            cooldownTimer = 0;
            Debug.Log("True");
        }
        if(Input.GetKeyUp(parryKeyKM) || Input.GetKeyUp(parryKeyJoystick))
        {
            parryCol.enabled = false;
            anim.SetBool("Block", false);
            player.SetCanMove(true);
            blockDamage = false;
            Debug.Log("False");
        }
    }
    public bool GetBlockDamage()
    {
        return blockDamage;
    }
    IEnumerator ParryColliderTime(float time)
    {
        blockDamage = true;
        yield return new WaitForSeconds(time);
        parryCol.enabled = false;
        yield return new WaitForSeconds(time * canMoveTimerOffset);
        anim.SetBool("Block", false);
        blockDamage = false;
        yield return null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitCollider") && habilityController.hability == HabilityController.Hability.parry)
        {
            shockwave.Play();
            parrySound.Post(gameObject);
            anim.SetTrigger("Deflect");
            canParry = false;
            enemyValues.anim.SetTrigger("Damage");
            StartCoroutine(slowMotion.ActivateSlowMotion(0.5f, 0.5f));
            StartCoroutine(ParryColliderTime(duration));
            parryEffect(this);
        }
        else if(collision.gameObject.CompareTag("HitCollider") && habilityController.hability != HabilityController.Hability.parry)
        {
            parrySound.Post(gameObject);
        }
    }

}
