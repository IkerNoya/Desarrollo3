using System.Collections;
using System.Collections.Generic;
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

    float timer;
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        switch (hability)
        {
            case Hability.healing:
                if (Input.GetKey(habilityKeyKM) || Input.GetKey(habilityKeyJoystick))
                {
                    player.hp += healingSpeed * Time.deltaTime;
                }
                break;
            case Hability.chargeAttack:
                if (timer <= 0)
                {
                    if (Input.GetKey(habilityKeyKM) || Input.GetKey(habilityKeyJoystick))
                    {
                        //chargeAttack + animation
                    }
                    else if(Input.GetKeyUp(habilityKeyKM) || Input.GetKeyUp(habilityKeyJoystick))
                    {
                        //attack + animation
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
}
