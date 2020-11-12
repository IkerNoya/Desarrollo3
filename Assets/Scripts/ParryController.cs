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

    PlayerController enemyValues;
    bool blockDamage = false;
    bool canParry = false;

    float canMoveTimerOffset = 0.3f; // multiplication applied in coroutine to delay movement
    float cooldownTimer = 0;

    PlayerController player;
    BoxCollider2D parryCol;
    Animator anim;
    CombatController cc;
    

    public static event Action<ParryController> parryEffect;
    void Start()
    {
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
        cooldownTimer += Time.deltaTime;
        if (!canParry)
            return;
        ParryInput();
    }
    void ParryInput()
    {
        if ((Input.GetKeyDown(parryKeyKM) || Input.GetKeyDown(parryKeyJoystick)) && player.GetGrounded() && !cc.IsAttacking)
        {
            anim.SetTrigger("Parry");
            player.SetCanMove(false);
            StartCoroutine(ParryColliderTime(duration));
            cooldownTimer = 0;
            canParry = false;
        }
    }
    public bool GetBlockDamage()
    {
        return blockDamage;
    }
    IEnumerator ParryColliderTime(float time)
    {
        blockDamage = true;
        parryCol.enabled = true;
        yield return new WaitForSeconds(time);
        parryCol.enabled = false;
        yield return new WaitForSeconds(time * canMoveTimerOffset);
        player.SetCanMove(true);
        blockDamage = false;
        yield return null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitCollider"))
        {
            enemyValues.anim.SetTrigger("Damage");
            StartCoroutine(slowMotion.ActivateSlowMotion(0.5f, 0.5f));
            parryEffect(this);
        }
    }

}
