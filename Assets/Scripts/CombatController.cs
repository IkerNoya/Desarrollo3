using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class CombatController : MonoBehaviour
{
	[SerializeField]
	private int hitIndex;
	[SerializeField]
	private float hitCooldown;
	[SerializeField]
	private string currentComboName;
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
		public KeyCode comboKey;
		public KeyCode keyBeingPressed;
		public AnimationClip clip;
		public bool critical;
	}

	public Animator animator;
	public AnimatorOverrideController overrider;
	public GameObject hitCol;

	bool isAttacking = false;
	public bool IsAttacking => isAttacking;

	private int comboLimit = 3;

	PlayerController player;

	public event Action OnComboCanceled;

	float timer = 0;

	private void Start()
	{
		combosByID = new Dictionary<string, Combo>();

		Time.timeScale = 0.5f;

		player = GetComponentInParent<PlayerController>();

		foreach (Combo combo in combos)
		{
			combosByID.Add(combo.ComboName, combo);
		}
	}

	void Update()
	{
		if (currentCombo == null)
		{
			timer += Time.deltaTime;
			foreach (Combo combo in combos)
			{
				ComboHit comboHit = combo.comboData[0];
				if (Input.GetKeyDown(comboHit.comboKey) && timer >= comboHit.hitDuration)
				{
					timer = 0;
					player.SetCanMove(false);
					StartCombo(combo);
					break;
				}
			}
		}
		else
		{
			if (hitIndex < currentCombo.comboData.Count)
			{
                timer += Time.deltaTime;
                if (Input.GetKeyDown(currentCombo.comboData[hitIndex].comboKey) && timer >= currentCombo.comboData[hitIndex].hitDuration)
				{
					timer = 0;
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
