using System;
using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] PlayerController player;
    [SerializeField] CameraShake HealthBarShake;
    [SerializeField] Image healthBar;
    [SerializeField] float hpShakeDuration;
    [SerializeField] float hpShakeMagnitude;
    float healthbarSize = 1f;
    #endregion

    #region BASE_FUNCTIONS
    private void Awake()
    {
        PlayerController.EmptyHP += HPEmpty;
        PlayerController.takeDamage += TakeDamage;
    }
    void Update()
    {
        healthBar.fillAmount = HitPercentage((int)player.hp, healthbarSize);
    }
    #endregion

    #region FUNCTIONS
    float HitPercentage(int damage, float barSize)
    {
        float maxPercentage = 100;
        float result = barSize * damage / maxPercentage;
        return result;
    }
    void TakeDamage(PlayerController pc)
    {
        StartCoroutine(HealthBarShake.Shake(gameObject, hpShakeDuration, hpShakeMagnitude));
    }
    void HPEmpty(PlayerController pc)
    {
         healthBar.fillAmount = 0f;
         StartCoroutine(HealthBarShake.Shake(gameObject, hpShakeDuration, hpShakeMagnitude));
    }
    #endregion

    #region BASE_FUCTIONS
    private void OnDisable()
    {
        PlayerController.EmptyHP -= HPEmpty;
        PlayerController.takeDamage -= TakeDamage;
    }
    #endregion
}
