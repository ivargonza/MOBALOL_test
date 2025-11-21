using UnityEngine;
using System.Collections;

public class Debuffs : MonoBehaviour 
{
	[Header("Crowd Control")]
	public bool Stun;
	public bool Slow;
	public bool AirBorne;
	public bool Silence;
	public bool Blind;
	public bool Fear;
	public bool Invulnerable;
	public bool SpellInmune;
	public bool Invisible;
	
	[Header("Buffs")]
	public bool SpeedBoost;
	public bool AttackSpeedBoost;
	
	[Header("Durations & Percentages")]
	public float SlowPercent;
	public float SlowTime;
	public float FearTime;
	public float SpeedBoostPercent;
	public float SpeedBoostTime;
	public float AttackSpeedBoostPercent;
	public float AttackSpeedBoostTime;
	public float damageboost;
	public float damageboostTime;
	public float StunTime;
	public float AirBorneTime;
	public float SilenceTime;
	public float BlindTime;
	public float InvulnerableTime;
	public float SpellInmuneTime;
	public float buffsIncDur;
	
	[Header("Modifiers")]
	public float SpeedBoostModifier;
	public float AttackSpeedBoostModifier;
	
	[Header("Invisibility Settings")]
	public bool revealOnAttack = true;
	public bool revealOnDamage = true;
	public GameObject invisibilityEffect;
	
	[Header("Effects")]
	public GameObject StunFX;
	public GameObject SlowFX;
	public GameObject SilenceFX;
	public GameObject BlindFX;
	
	// Cached components
	private UnityEngine.AI.NavMeshAgent nav;
	private Character character;
	private Animator animator;
	private CapsuleCollider capsuleCollider;
	private Renderer[] renderers;
	
	private float startNavSpeed;
	private float rango;
	private float invisibilityTimer;
	private bool wasVisible = true;
	
	private void Awake()
	{
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		renderers = GetComponentsInChildren<Renderer>();
	}
	
	private void Start() 
	{
		if (character.IsPlayer || character.Isbot || character.IsCreep || 
		    character.IsMascot || character.IsJungle) 
		{
			nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (nav != null)
			{
				startNavSpeed = nav.speed;
			}
			
			CacheRangoFromAI();
			
			if (SilenceFX != null && !Silence) 
			{
				SilenceFX.SetActive(false);
			}
		}
	}
	
	private void CacheRangoFromAI()
	{
		if (character.IsPlayer)
		{
			ClickToMove ctm = GetComponent<ClickToMove>();
			if (ctm != null) rango = ctm.Rango;
		}
		else if (character.Isbot)
		{
			IABots bot = GetComponent<IABots>();
			if (bot != null) rango = bot.Rango;
		}
		else if (character.IsCreep)
		{
			MinionsAI minion = GetComponent<MinionsAI>();
			if (minion != null) rango = minion.Rango;
		}
		else if (character.IsMascot)
		{
			MascotAI mascot = GetComponent<MascotAI>();
			if (mascot != null) rango = mascot.Rango;
		}
		else if (character.IsJungle)
		{
			JungleAI jungle = GetComponent<JungleAI>();
			if (jungle != null) rango = jungle.Rango;
		}
	}
	
	private void Update() 
	{
		if (!(character.IsPlayer || character.Isbot || character.IsCreep || 
		      character.IsMascot || character.IsJungle)) 
			return;
		
		UpdateStun();
		UpdateBlind();
		UpdateDamageBoost();
		UpdateSlow();
		UpdateSilence();
		UpdateSpeedBoost();
		UpdateAttackSpeedBoost();
		UpdateInvulnerability();
		UpdateSpellImmune();
		UpdateFear();
		UpdateInvisibility();
		UpdateDeathCleaning();
	}
	
	// ========== STUN ==========
	private void UpdateStun()
	{
		if (StunTime > 0f)
		{
			StunTime -= Time.deltaTime;
			Stun = true;
			
			if (StunFX != null) StunFX.SetActive(true);
			
			if (nav != null) nav.enabled = false;
			if (animator != null)
			{
				animator.SetBool("IsAttacking", false);
				animator.SetFloat("State", 0f);
			}
			

		}
		else if (Stun)
		{
			Stun = false;
			StunTime = 0f;
			
			if (StunFX != null) StunFX.SetActive(false);
			
			if (character.IsAlive && nav != null)
			{
				nav.enabled = true;
			}
			

		}
	}
	
	// ========== BLIND ==========
	private void UpdateBlind()
	{
		if (BlindTime > 0f)
		{
			BlindTime -= Time.deltaTime;
			Blind = true;
			character.canceladordedamage = 0f;
		}
		else if (Blind)
		{
			Blind = false;
			BlindTime = 0f;
			character.canceladordedamage = 1f;
		}
	}
	
	// ========== DAMAGE BOOST ==========
	private void UpdateDamageBoost()
	{
		if (damageboostTime > 0f)
		{
			damageboostTime -= Time.deltaTime;
		}
		else if (damageboost != 0f)
		{
			damageboostTime = 0f;
			damageboost = 0f;
		}
	}
	
	// ========== SLOW ==========
	private void UpdateSlow()
	{
		if (SlowTime > 0f)
		{
			SlowTime -= Time.deltaTime;
			Slow = true;
			
			if (nav != null)
			{
				float speed = startNavSpeed - ((startNavSpeed * SlowPercent) / 100f);
				SpeedBoostModifier = -(SlowPercent / 100f);
				nav.speed = speed;
			}
			
			if (SlowFX != null) SlowFX.SetActive(true);
		}
		else if (Slow)
		{
			Slow = false;
			SlowPercent = 0f;
			SpeedBoostModifier = 0f;
			SlowTime = 0f;
			
			if (SlowFX != null) SlowFX.SetActive(false);
			if (nav != null) nav.speed = startNavSpeed;
		}
	}
	
	// ========== SILENCE ==========
	private void UpdateSilence()
	{
		if (SilenceTime > 0f)
		{
			SilenceTime -= Time.deltaTime;
			Silence = true;
			if (SilenceFX != null) SilenceFX.SetActive(true);
		}
		else if (Silence)
		{
			Silence = false;
			SilenceTime = 0f;
			if (SilenceFX != null) SilenceFX.SetActive(false);
		}
	}
	
	// ========== SPEED BOOST ==========
	private void UpdateSpeedBoost()
	{
		if (SpeedBoostTime > 0f)
		{
			SpeedBoostTime -= Time.deltaTime;
			SpeedBoost = true;
			
			if (nav != null)
			{
				float speed = startNavSpeed + ((startNavSpeed * SpeedBoostPercent) / 100f);
				SpeedBoostModifier = SpeedBoostPercent / 100f;
				nav.speed = speed;
			}
		}
		else if (SpeedBoost)
		{
			SpeedBoost = false;
			SpeedBoostPercent = 0f;
			SpeedBoostTime = 0f;
			SpeedBoostModifier = 0f;
			
			if (nav != null) nav.speed = startNavSpeed;
		}
	}
	
	// ========== ATTACK SPEED BOOST ==========
	private void UpdateAttackSpeedBoost()
	{
		if (AttackSpeedBoostTime > 0f)
		{
			AttackSpeedBoostTime -= Time.deltaTime;
			AttackSpeedBoost = true;
			AttackSpeedBoostModifier = AttackSpeedBoostPercent / 100f;
		}
		else if (AttackSpeedBoost)
		{
			AttackSpeedBoost = false;
			AttackSpeedBoostPercent = 0f;
			AttackSpeedBoostTime = 0f;
			AttackSpeedBoostModifier = 0f;
		}
	}
	
	// ========== INVULNERABILITY ==========
	private void UpdateInvulnerability()
	{
		if (InvulnerableTime > 0f)
		{
			InvulnerableTime -= Time.deltaTime;
			Invulnerable = true;
			
			if (capsuleCollider != null)
			{
				capsuleCollider.enabled = false;
			}
		}
		else if (Invulnerable)
		{
			Invulnerable = false;
			InvulnerableTime = 0f;
			
			if (character.IsAlive && capsuleCollider != null)
			{
				capsuleCollider.enabled = true;
			}
		}
	}
	
	// ========== SPELL IMMUNE ==========
	private void UpdateSpellImmune()
	{
		if (SpellInmuneTime > 0f)
		{
			SpellInmuneTime -= Time.deltaTime;
			SpellInmune = true;
		}
		else if (SpellInmune)
		{
			SpellInmune = false;
			SpellInmuneTime = 0f;
		}
	}
	
	// ========== FEAR ==========
	private void UpdateFear()
	{
		if (FearTime > 0f)
		{
			FearTime -= Time.deltaTime;
			Fear = true;

			ClearObjective();
		}
		else if (Fear)
		{
			Fear = false;
			FearTime = 0f;

		}
	}
	
	// ========== INVISIBILITY ==========
	private void UpdateInvisibility()
	{
		if (invisibilityTimer > 0f)
		{
			invisibilityTimer -= Time.deltaTime;
			
			if (invisibilityTimer <= 0f)
			{
				RemoveInvisibility();
			}
		}
		
		UpdateVisibility();
	}
	
	public void ApplyInvisibility(float duration)
	{
		Invisible = true;
		invisibilityTimer = duration;
		
		if (invisibilityEffect != null)
		{
			invisibilityEffect.SetActive(true);
		}
	}
	
	public void RemoveInvisibility()
	{
		Invisible = false;
		invisibilityTimer = 0f;
		
		if (invisibilityEffect != null)
		{
			invisibilityEffect.SetActive(false);
		}
	}
	
	private void UpdateVisibility()
	{
		if (Invisible != wasVisible)
		{
			SetRenderersVisibility(!Invisible);
			wasVisible = Invisible;
		}
	}
	
	private void SetRenderersVisibility(bool visible)
	{
		foreach (Renderer renderer in renderers)
		{
			if (renderer == null) continue;
			
			if (character.IsPlayer)
			{
				// El jugador se ve a sí mismo semi-transparente
				SetRendererAlpha(renderer, visible ? 1f : 0.3f);
			}
			else
			{
				// Enemigos completamente invisibles
				renderer.enabled = visible;
			}
		}
	}
	
	private void SetRendererAlpha(Renderer renderer, float alpha)
	{
		foreach (Material mat in renderer.materials)
		{
			if (mat.HasProperty("_Color"))
			{
				Color color = mat.color;
				color.a = alpha;
				mat.color = color;
				
				if (alpha < 1f)
				{
					mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					mat.SetInt("_ZWrite", 0);
					mat.DisableKeyword("_ALPHATEST_ON");
					mat.EnableKeyword("_ALPHABLEND_ON");
					mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					mat.renderQueue = 3000;
				}
			}
		}
	}
	
	public void OnAttackPerformed()
	{
		if (Invisible && revealOnAttack)
		{
			RemoveInvisibility();
		}
	}
	
	public void OnDamageTaken()
	{
		if (Invisible && revealOnDamage)
		{
			RemoveInvisibility();
		}
	}
	
	// ========== PUBLIC METHODS FOR APPLYING EFFECTS ==========
	
	public void ApplyInvulnerability(float duration)
	{
		InvulnerableTime = duration;
	}
	
	public void ApplyFear(float duration)
	{
		FearTime = duration;
	}
	
	public void ApplyStun(float duration)
	{
		StunTime = duration;
	}
	
	public void ApplySlow(float duration, float percent)
	{
		SlowTime = duration;
		SlowPercent = percent;
	}
	
	public void ApplySilence(float duration)
	{
		SilenceTime = duration;
	}
	
	public void ApplyBlind(float duration)
	{
		BlindTime = duration;
	}
	
	public void ApplySpeedBoost(float duration, float percent)
	{
		SpeedBoostTime = duration;
		SpeedBoostPercent = percent;
	}
	
	public void ApplyAttackSpeedBoost(float duration, float percent)
	{
		AttackSpeedBoostTime = duration;
		AttackSpeedBoostPercent = percent;
	}
	
	public void ApplyDamageBoost(float duration, float amount)
	{
		damageboostTime = duration;
		damageboost = amount;
	}
	
	// ========== DEATH CLEANING ==========
	private void UpdateDeathCleaning()
	{
		if (character.Vida <= 0f) 
		{
			if (animator != null)
			{
				animator.SetBool("IsAttacking", false);
				animator.SetFloat("State", 2f);
			}
			
			ResetPositionOnDeath();
			DeactivateAllEffects();
			ClearAllDebuffs();
		}
	}
	
	private void ResetPositionOnDeath()
	{
		if (character.IsPlayer)
		{
			ClickToMove ctm = GetComponent<ClickToMove>();
			if (ctm != null) ctm.Position = transform.position;
		}
		else if (character.Isbot)
		{
			IABots bot = GetComponent<IABots>();
			if (bot != null) bot.ultimopunto = transform;
		}
		else if (character.IsCreep)
		{
			MinionsAI minion = GetComponent<MinionsAI>();
			if (minion != null) minion.ultimopunto = transform;
		}
	}
	
	private void DeactivateAllEffects()
	{
		if (SlowFX != null) SlowFX.SetActive(false);
		if (SilenceFX != null) SilenceFX.SetActive(false);
		if (StunFX != null) StunFX.SetActive(false);
	}
	
	private void ClearAllDebuffs()
	{
		Slow = false;
		Stun = false;
		Silence = false;
		SpeedBoost = false;
		Blind = false;
		Fear = false;
		Invisible = false;
		
		SpeedBoostPercent = 0f;
		SpeedBoostModifier = 0f;
		SlowPercent = 0f;
		
		StunTime = 0f;
		SlowTime = 0f;
		SilenceTime = 0f;
		BlindTime = 0f;
		FearTime = 0f;
		SpeedBoostTime = 0f;
		AttackSpeedBoostTime = 0f;
		InvulnerableTime = 0f;
		SpellInmuneTime = 0f;
		invisibilityTimer = 0f;
		
		character.canceladordedamage = 1f;
		
		if (nav != null) nav.speed = startNavSpeed;
	}
	
	// ========== HELPER METHODS ==========

	private void ClearObjective()
	{
		if (character.IsPlayer)
		{
			ClickToMove ctm = GetComponent<ClickToMove>();
			if (ctm != null) ctm.Objetivo = null;
		}
		else if (character.Isbot)
		{
			IABots bot = GetComponent<IABots>();
			if (bot != null) bot.Objetivo = null;
		}
		else if (character.IsCreep)
		{
			MinionsAI minion = GetComponent<MinionsAI>();
			if (minion != null) minion.Objetivo = null;
		}
		else if (character.IsJungle)
		{
			JungleAI jungle = GetComponent<JungleAI>();
			if (jungle != null) jungle.Objetivo = null;
		}
	}
}