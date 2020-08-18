using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    public Animator anim;
    public int MaxHitCount;
    protected float timer;
    protected float hitCounter = 0;
    bool canHit = true;
    float HitCooldown = 1;
    float lastHitTime = 0;
    protected void StartCombo()
    {
        if(Time.time - lastHitTime > HitCooldown)
        {
            hitCounter = 0;
        }
        
    }
    protected void ContinueCombo()
    {
        canHit = false;
        if (hitCounter == 1)
        {

        }
    }
}
