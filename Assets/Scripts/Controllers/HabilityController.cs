using System.Collections;
using System.Net.Http.Headers;
using UnityEngine;

public class HabilityController : MonoBehaviour
{
    public enum Hability
    {
        healing, chargeAttack, parry
    }
    PlayerController player;
    public Hability hability;
    public AK.Wwise.Event healingSound;
    public AK.Wwise.Event ChargeAttackSound;

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
    [SerializeField] float parryDamage;
    [SerializeField] GameObject habilityCol;
    [Space]
    [SerializeField] ParticleSystem healingParticles;
    [Space]
    CombatController combatController;
    float timer;
    bool canParry = false;
    DataManager data;
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        combatController = GetComponent<CombatController>();
        habilityCol.SetActive(false);
        data = DataManager.instance;
        LoadHabilities();
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
                    if(healingSound!=null)
                        healingSound.Post(gameObject);
                    if (player.hp > 100)
                        player.hp = 100;
                    healingParticles.Play();
                }
                if (Input.GetKeyUp(habilityKeyKM) || Input.GetKeyUp(habilityKeyJoystick))
                {
                    player.anim.SetBool("HoldHealing", false);
                    player.anim.SetTrigger("StopHealing");
                    if(healingSound!=null)
                        healingSound.Stop(gameObject);
                    healingParticles.Stop();
                }
                break;
            case Hability.chargeAttack:
                if (timer <= 0)
                {
                    if (Input.GetKey(habilityKeyKM) || Input.GetKey(habilityKeyJoystick))
                    {
                        player.SetCanMove(false);
                        player.anim.SetBool("HoldCharge", true);
                        if (ChargeAttackSound != null)
                            ChargeAttackSound.Post(gameObject);
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
                    canParry = true;
                }
                break;
        }
        timer -= Time.deltaTime;
    }
    void LoadHabilities()
    {
        switch (player.playerSelect)
        {
            case PlayerController.PlayerSelect.player1:
                switch (data.GetPlayer1Hability())
                {
                    case DataManager.Hability.heal:
                        hability = Hability.healing;
                        break;
                    case DataManager.Hability.burst:
                        hability = Hability.chargeAttack;
                        break;
                    case DataManager.Hability.parry:
                        hability = Hability.parry;
                        break;
                }
                break;
            case PlayerController.PlayerSelect.player2:
                switch (data.GetPlayer2Hability())
                {
                    case DataManager.Hability.heal:
                        hability = Hability.healing;
                        break;
                    case DataManager.Hability.burst:
                        hability = Hability.chargeAttack;
                        break;
                    case DataManager.Hability.parry:
                        hability = Hability.parry;
                        break;
                }
                break;
        }
    }
    public Hability GetCurrentHablity()
    {
        return hability;
    }
    public void SetCanParry(bool value)
    {
        canParry = value;
    }
    public bool GetCanParry()
    {
        return canParry;
    }
    public float GetChargeDamage()
    {
        return chargeAttackDamage;
    }
    public float GetParryDamage()
    {
        return parryDamage;
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
    public void SetParryCooldown()
    {
        canParry = false;
        timer = parryCooldown;
    }
}
