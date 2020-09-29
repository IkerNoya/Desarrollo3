using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public float hitCooldown;
    [SerializeField] PlayerController playerAnim;
    public GameObject hitCol;
    public GameObject parryCol;
    [HideInInspector] public int hitCounter;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool canParry = true;


    float lastHit;
    private void Start()
    {

        parryCol.SetActive(false);
    }
    public void StartCombo()
    {
        if ((Time.time - lastHit) > hitCooldown)
        {
            isAttacking = false;
            playerAnim.SetCanMove(true);
            hitCounter = 0;
        }

        lastHit = Time.time;
        hitCounter++;
        if (hitCounter == 1)
        {
            playerAnim.anim.SetBool("IsAttacking_1", true);
        }
        hitCounter = Mathf.Clamp(hitCounter, 0, 3);
    }
    public void Attack1()
    {
        hitCol.SetActive(false);
        if (hitCounter >= 2)
        {
            playerAnim.anim.SetBool("IsAttacking_2", true);
            playerAnim.anim.SetBool("IsAttacking_1", false);
        }
        else
        {
            playerAnim.anim.SetBool("IsAttacking_1", false);
            hitCounter = 0;
            isAttacking = false;
            playerAnim.SetCanMove(true);
        }
    }
    public void Attack2()
    {
        hitCol.SetActive(false);
        if (hitCounter >= 3)
        {
            playerAnim.anim.SetBool("IsAttacking_3", true);
        }
        else
        {
            playerAnim.anim.SetBool("IsAttacking_2", false);
            hitCounter = 0;
            isAttacking = false;
            playerAnim.SetCanMove(true);
        }
    }
    public void Attack3()
    {
        hitCol.SetActive(false);
        playerAnim.anim.SetBool("IsAttacking_1", false);
        playerAnim.anim.SetBool("IsAttacking_2", false);
        playerAnim.anim.SetBool("IsAttacking_3", false);
        isAttacking = false;
        playerAnim.SetCanMove(true);
        hitCounter = 0;
    }
    public void ActivateHit()
    {
        hitCol.SetActive(true);
    }
    public void DeactivateParry()
    {
        parryCol.SetActive(false);
        canParry = true;
        playerAnim.anim.SetBool("Parry", false);
        StartCoroutine(WaitToAttack());
    }
    public void ActivateParry()
    {
        parryCol.SetActive(true);
        canParry = false; 
    }
    IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }
}
