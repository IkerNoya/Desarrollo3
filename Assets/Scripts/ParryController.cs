using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryController : MonoBehaviour
{
    [SerializeField] KeyCode parryKeyKM;
    [SerializeField] KeyCode parryKeyJoystick;
    [Space]
    [SerializeField] float duration;
    [Space]
    [SerializeField] GameObject enemy;
    PlayerController enemyValues;
    bool blockDamage = false;

    float canMoveTimerOffset = 0.3f; // multiplication applied in coroutine to delay movement
    PlayerController player;
    BoxCollider2D parryCol;
    Animator anim;

    public static event Action<ParryController> parryEffect;
    void Start()
    {
        parryCol = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
        parryCol.enabled = false;
        enemyValues = enemy.GetComponent<PlayerController>();
    }
    void Update()
    {
        if((Input.GetKeyDown(parryKeyKM) || Input.GetKeyDown(parryKeyJoystick)) && player.GetGrounded())
        {
            anim.SetTrigger("Parry");
            player.SetCanMove(false);
            StartCoroutine(ParryColliderTime(duration));
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
            //zoom
        }
    }

}
