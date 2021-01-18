using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
	[SerializeField] int hitIndex;
	[SerializeField] float hitCooldown;
	[SerializeField] string currentComboName;
	[SerializeField] ParticleSystem shockwave;
	float damage;
	bool criticalDamage;
	[Serializable]
	public class Combo
	{
		public string ComboName;
		public List<ComboHit> comboData;
	}
	public List<Combo> combos;
	private Combo currentCombo;

	private Dictionary<string, Combo> combosByID;

	[Serializable]
	public class ComboHit
	{
		public float cooldown = 0.5f;
		public float hitDuration = 0.1f;
		public float damage = 10;
		public KeyCode comboKeyKM;
		public KeyCode comboKeyJoystick;
		public KeyCode keyBeingPressed;
		public AnimationClip clip;
		public bool critical;
		public AK.Wwise.Event hitSound;
	}

	public Animator animator;
	public AnimatorOverrideController overrider;
	public GameObject hitCol;

	bool isAttacking = false;
	public bool IsAttacking => isAttacking;

	int comboLimit = 3;

	PlayerController player;

	public event Action OnComboCanceled;

	float timer = 0;

	void Start()
	{
		combosByID = new Dictionary<string, Combo>();

		player = GetComponentInParent<PlayerController>();

		foreach (Combo combo in combos)
		{
			combosByID.Add(combo.ComboName, combo);
		}
	}

	void Update()
	{
		if (player.GetPause())
			return;
		if (currentCombo == null)
		{
			timer += Time.deltaTime;
			foreach (Combo combo in combos)
			{
				ComboHit comboHit = combo.comboData[0];
				if ((Input.GetKeyDown(comboHit.comboKeyKM) || Input.GetKeyDown(comboHit.comboKeyJoystick)) && timer >= comboHit.hitDuration && player.GetGrounded())
				{
					timer = 0;
					player.SetCanMove(false);
					damage = comboHit.damage;
					criticalDamage = comboHit.critical;
					StartCombo(combo);
					if(comboHit.hitSound!=null)
						comboHit.hitSound.Post(gameObject);
					break;
				}
			}
		}
		else
		{
			if (hitIndex < currentCombo.comboData.Count)
			{
                timer += Time.deltaTime;
				if ((Input.GetKeyDown(currentCombo.comboData[hitIndex].comboKeyKM) || Input.GetKeyDown(currentCombo.comboData[hitIndex].comboKeyJoystick)) && timer >= currentCombo.comboData[hitIndex].hitDuration && player.GetGrounded())
				{
					timer = 0;
					damage = currentCombo.comboData[hitIndex].damage;
					criticalDamage = currentCombo.comboData[hitIndex].critical;
					if(currentCombo.comboData[hitIndex].hitSound!=null);
						currentCombo.comboData[hitIndex].hitSound.Post(gameObject);
					AttackAction();
				}
			}
		}

		hitCooldown -= Time.deltaTime;
		if (hitCooldown <= 0)
			CancelCombo();
	}

	void StartCombo(Combo combo)
	{
		currentCombo = combo;
		currentComboName = currentCombo.ComboName;
		AttackAction();
	}

	public void AttackAction()
	{
		comboLimit = currentCombo.comboData.Count;
		hitCooldown = currentCombo.comboData[hitIndex].cooldown;
		overrider["Hit"] = currentCombo.comboData[hitIndex].clip;
		hitIndex++;
		animator.SetTrigger("Hit");
		SetHitAnim();
	}

	public void CancelCombo()
	{
		criticalDamage = false;
		currentComboName = "";
		hitIndex = 0;
		currentCombo = null;
		player.SetCanMove(true);
		OnComboCanceled?.Invoke();
	}

	void SetHitAnim()
	{
		float anim = hitIndex / (float)comboLimit;
		//animator.SetFloat("ComboIndexFloat", anim);
	}

	public float GetDamage()
	{
		return damage;
	}
	public bool GetCriticalDamageValue()
	{
		return criticalDamage;
	}
	public void ActivateHit()
	{
		hitCol.SetActive(true);
		StartCoroutine(DeactivateHitIn(0.1f));
	}

	IEnumerator DeactivateHitIn(float t)
	{
		yield return new WaitForSeconds(t);
		hitCol.SetActive(false);
	}
}
