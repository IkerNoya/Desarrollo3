using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
	[SerializeField] int hitIndex;
	[SerializeField] float hitCooldown;
	[Space]
	[SerializeField] string currentComboName;
	[Serializable]
	public class Combo
	{
		public string ComboName;
		public List<ComboHit> comboData;
	}
	public List<Combo> combos;

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
	[Space]
	public Animator animator;
	public AnimatorOverrideController overrider;
	public GameObject hitCol;

	[Space]
	[SerializeField] List<AnimationClip> NovaAttackClips;
	[SerializeField] List<AnimationClip> CyberBunnyAttackClips;
	[SerializeField] AnimationClip AirAttack2_Nova;
	[SerializeField] AnimationClip AirAttack2_Cyber;
	[Space]

	private Combo currentCombo;
	float damage;
	bool criticalDamage;
	private Dictionary<string, Combo> combosByID;
	bool canAirAttack = true;

	bool isAttacking = false;
	public bool IsAttacking => isAttacking;

	int comboLimit = 3;
	bool isComboCanceled = true;

	PlayerController player;
	DataManager data;

	public event Action OnComboCanceled;

	float timer = 0;
	void Start()
	{
		combosByID = new Dictionary<string, Combo>();
		player = GetComponentInParent<PlayerController>();
		data = DataManager.Instance;

		foreach (Combo combo in combos)
		{
			combosByID.Add(combo.ComboName, combo);
		}
		LoadAnimator();
	}
	void Update()
	{
		if (player.GetPause())
			return;
		if (currentCombo == null)
		{
			timer += Time.deltaTime;
			Combo ground = combosByID["Ground_Combo"];
			Combo air = combosByID["Air_Combo"];
			ComboHit comboHitGround = ground.comboData[0];
			ComboHit comboHitAir = air.comboData[0];
			if ((Input.GetKeyDown(comboHitGround.comboKeyKM) || Input.GetKeyDown(comboHitGround.comboKeyJoystick)) && timer >= comboHitGround.hitDuration && player.GetGrounded())
			{
				timer = 0;
				player.SetCanMove(false);
				damage = comboHitGround.damage;
				criticalDamage = comboHitGround.critical;
				StartCombo(ground);
				if(comboHitGround.hitSound!=null)
					comboHitGround.hitSound.Post(gameObject);
			}
			if ((Input.GetKeyDown(comboHitAir.comboKeyKM) || Input.GetKeyDown(comboHitAir.comboKeyJoystick)) && timer >= comboHitAir.hitDuration && !player.GetGrounded() && canAirAttack)
			{
				timer = 0;
				player.SetCanMove(false);
				damage = comboHitGround.damage;
				player.SetGravity(0);
				criticalDamage = comboHitGround.critical;
				StartCombo(air);
				if (comboHitAir.hitSound != null)
					comboHitAir.hitSound.Post(gameObject);
			}
		}
		else
		{
			if (hitIndex < currentCombo.comboData.Count)
			{
                timer += Time.deltaTime;
				if ((Input.GetKeyDown(currentCombo.comboData[hitIndex].comboKeyKM) || Input.GetKeyDown(currentCombo.comboData[hitIndex].comboKeyJoystick)) && timer >= currentCombo.comboData[hitIndex].hitDuration)
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
		if (hitCooldown <= 0 && !isComboCanceled)
			CancelCombo();
		if (player.GetGrounded())
			canAirAttack = true;
	}

	void LoadAnimator()
	{
		switch (player.playerSelect)
		{
			case PlayerController.PlayerSelect.player1:
				switch (data.player1Choice.playerSelection)
				{
					case DataManager.PlayerSelection.Nova:
						animator.runtimeAnimatorController = Resources.Load("Animations/AnimatorController/Nova") as RuntimeAnimatorController;
						overrider = Resources.Load("Animations/AnimatorController/Nova") as AnimatorOverrideController;
						for (int i = 0; i < NovaAttackClips.Count; i++)
						{
							combos[0].comboData[i].clip = NovaAttackClips[i];
						}
						combos[1].comboData[1].clip = AirAttack2_Nova;
						break;
					case DataManager.PlayerSelection.CyberBunny:
						animator.runtimeAnimatorController = Resources.Load("Animations/AnimatorController/CyberBunny") as RuntimeAnimatorController;
						overrider = Resources.Load("Animations/AnimatorController/CyberBunny") as AnimatorOverrideController;
						for (int i = 0; i < CyberBunnyAttackClips.Count; i++)
						{
							combos[0].comboData[i].clip = CyberBunnyAttackClips[i];
						}
						combos[1].comboData[1].clip = AirAttack2_Cyber;
						break;
				}
				break;
			case PlayerController.PlayerSelect.player2:
				switch (data.player2Choice.playerSelection)
				{
					case DataManager.PlayerSelection.Nova:
						animator.runtimeAnimatorController = Resources.Load("Animations/AnimatorController/Nova") as RuntimeAnimatorController;
						overrider = Resources.Load("Animations/AnimatorController/Nova") as AnimatorOverrideController;
						for (int i = 0; i < NovaAttackClips.Count; i++)
						{
							combos[0].comboData[i].clip = NovaAttackClips[i];
						}
						combos[1].comboData[1].clip = AirAttack2_Nova;
						break;
					case DataManager.PlayerSelection.CyberBunny:
						animator.runtimeAnimatorController = Resources.Load("Animations/AnimatorController/CyberBunny") as RuntimeAnimatorController;
						overrider = Resources.Load("Animations/AnimatorController/CyberBunny") as AnimatorOverrideController;
						for (int i = 0; i < CyberBunnyAttackClips.Count; i++)
						{
							combos[0].comboData[i].clip = CyberBunnyAttackClips[i];
						}
						combos[1].comboData[1].clip = AirAttack2_Cyber;
						break;
				}
				break;
		}
	}

	void StartCombo(Combo combo)
	{
		currentCombo = combo;
		isComboCanceled = false;
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
		isComboCanceled = true;
		criticalDamage = false;
		currentComboName = "";
		hitIndex = 0;
		currentCombo = null;
		player.SetCanMove(true);
		if (!player.GetGrounded())
			StartCoroutine(WaitForGravityReset(0.25f));
		canAirAttack = false;
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
	public void ActivateCanMove()
	{
		player.SetCanMove(true);
	}

	IEnumerator DeactivateHitIn(float t)
	{
		yield return new WaitForSeconds(t);
		hitCol.SetActive(false);
	}
	IEnumerator WaitForGravityReset(float time)
	{
		yield return new WaitForSeconds(time);
		player.SetDefaultGravity();
		yield return null;
	}
}
