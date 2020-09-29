using System;
using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] CameraShake HealthBarShake;
    [SerializeField] Image healthBar;
    [SerializeField] float hpShakeDuration;
    [SerializeField] float hpShakeMagnitude;
    float healthbarSize = 1f;

    private void Awake()
    {
        PlayerController.EmptyHP += HPEmpty;
        PlayerController.takeDamage += TakeDamage;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = HitPercentage(player.hp, healthbarSize);
    }
    float HitPercentage(int damage, float barSize)
    {
        float maxPercentage = 100;
        float result = barSize * damage / maxPercentage;
        return result;
    }
    void TakeDamage(PlayerController pc)
    {
        StartCoroutine(HealthBarShake.Shake(hpShakeDuration, hpShakeMagnitude));
    }
    void HPEmpty(PlayerController pc)
    {
         healthBar.fillAmount = 0f;
         StartCoroutine(HealthBarShake.Shake(hpShakeDuration, hpShakeMagnitude));
    }
    private void OnDisable()
    {
        PlayerController.EmptyHP -= HPEmpty;
        PlayerController.takeDamage -= TakeDamage;
    }
}
