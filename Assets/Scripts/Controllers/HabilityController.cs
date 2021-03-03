using System.Collections;
using UnityEngine;

public class HabilityController : MonoBehaviour
{
    public enum Hability
    {
        healing, chargeAttack, parry
    }
    PlayerController player;
    public Hability hability;

    [SerializeField] KeyCode habilityKeyKM;
    [SerializeField] KeyCode habilityKeyJoystick;
    [Space]
    [SerializeField] float healingSpeed;
    [SerializeField] float chargeSpeed;
    [Space]
    [SerializeField] float chargeCooldown;
    [SerializeField] float parryCooldown;
    [Space]
    [SerializeField] float chargeAttackDamage;
    [SerializeField] GameObject habilityCol;
    [Space]
    CombatController combatController;
    float timer;
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        combatController = GetComponent<CombatController>();
        habilityCol.SetActive(false);
    }

    void Update()
    {
        switch (hability)
        {
            case Hability.healing:
                if ((Input.GetKey(habilityKeyKM) || Input.GetKey(habilityKeyJoystick)) && player.hp < 100)
                {
                    player.SetCanMove(false);
                    player.Heal(healingSpeed);
                    player.anim.SetBool("HoldHealing", true);
                    if (player.hp > 100)
                        player.hp = 100;
                }
                if (Input.GetKeyUp(habilityKeyKM) || Input.GetKeyUp(habilityKeyJoystick))
                {
                    player.anim.SetBool("HoldHealing", false);
                    player.anim.SetTrigger("StopHealing");
                }
                break;
            case Hability.chargeAttack:
                if (timer <= 0)
                {
                    if (Input.GetKey(habilityKeyKM) || Input.GetKey(habilityKeyJoystick))
                    {
                        player.SetCanMove(false);
                        player.anim.SetBool("HoldCharge", true);
                    }
                    else if(Input.GetKeyUp(habilityKeyKM) || Input.GetKeyUp(habilityKeyJoystick))
                    {
                        player.anim.SetBool("HoldCharge", false);
                        player.anim.SetTrigger("ReleaseCharge");
                        timer = chargeCooldown;
                    }
                }
                break;
            case Hability.parry:
                if (timer <= 0)
                {
                    if (Input.GetKeyDown(habilityKeyKM) || Input.GetKeyDown(habilityKeyJoystick))
                    {
                        //parry + animation
                        timer = parryCooldown;
                    }
                }
                break;
        }
        timer -= Time.deltaTime;
    }
    public Hability GetCurrentHablity()
    {
        return hability;
    }
    public float GetChargeDamage()
    {
        return chargeAttackDamage;
    }
    public void ActivateHabilityCollider()
    {
        habilityCol.SetActive(true);
        StartCoroutine(DeactivateHitIn(0.05f));
    }
    IEnumerator DeactivateHitIn(float t)
    {
        yield return new WaitForSeconds(t);
        habilityCol.SetActive(false);
    }
}
