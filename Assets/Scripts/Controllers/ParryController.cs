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
    [SerializeField] AnimationCurve slowMotionIntensity;

    HabilityController habilityController;

    public AK.Wwise.Event parrySound;

    PlayerController enemyValues;
    bool blockDamage = false;

    PlayerController player;
    BoxCollider2D parryCol;
    Animator anim;
    CombatController cc;
    bool canBeParried = false;
    

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
        ParryInput();
    }
    void ParryInput()
    {
        if ((Input.GetKey(parryKeyKM) || Input.GetKey(parryKeyJoystick)) && player.GetGrounded() && !cc.IsAttacking)
        {
            parryCol.enabled = true;
            anim.SetBool("Block", true);
            player.SetCanMove(false);
            blockDamage = true;
            if (habilityController.GetCanParry() && habilityController.GetCurrentHablity()==HabilityController.Hability.parry)
            {
                StartCoroutine(ParryTiming(0.5f));
                habilityController.SetParryCooldown();
            }
        }
        if(Input.GetKeyUp(parryKeyKM) || Input.GetKeyUp(parryKeyJoystick))
        {
            parryCol.enabled = false;
            player.SetCanMove(true);
            anim.SetBool("Block", false);
            blockDamage = false;
        }
    }
    public bool GetBlockDamage()
    {
        return blockDamage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HitCollider") && habilityController.hability == HabilityController.Hability.parry && canBeParried)
        {
            shockwave.Play();
            parrySound.Post(gameObject);
            anim.SetTrigger("Deflect");
            enemyValues.anim.SetTrigger("Damage");
            StartCoroutine(slowMotion.ActivateSlowMotion(0.5f, slowMotionIntensity.Evaluate(1)));
            parryEffect(this);
        }
        else if(collision.gameObject.CompareTag("HitCollider") && habilityController.hability != HabilityController.Hability.parry)
        {
            parrySound.Post(gameObject);
        }
    }
    IEnumerator ParryTiming(float time)
    {
        canBeParried = true;
        yield return new WaitForSeconds(time);
        canBeParried = false;
        yield return null;
    }
}
