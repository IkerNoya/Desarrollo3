using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilityController : MonoBehaviour
{
    public enum Hability
    {
        healing, chargeAttack, parry
    }
    public Hability hability;
    [SerializeField] KeyCode habilityKeyKM;
    [SerializeField] KeyCode habilityKeyJoystick;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKey(habilityKeyKM) || Input.GetKey(habilityKeyJoystick))
        {
            Debug.Log("Activate Hability");
        }
    }
}
