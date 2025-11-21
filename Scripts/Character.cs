using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Character : MonoBehaviour 
{
	// Stats
	public int Nivel = 1;
	public float Experiencia;
	public float ExperienciaMax;
	public float ExperienciaExtra;
	public float Vida;
	public float VidaMax;
	public float VidaRegen;
	public float Mana;
	public float ManaMax;
	public float ManaRegen;
	public float ManaRegenBuff;
	public float Armadura;
	public float ResistenciaMagica = 9f;
	public float ArmaduraFinal;
	public float AbilityPower;
	public float LifeSteal;
	public float Daño;
	
	// Modificadores temporales
	public float VelocidadDeMovimiento = 1f;
	public float VelocidadDeAtaqueExtra = 0;
	public float canceladordedamage = 1f;
	public float armaduraTemporal;
	
	// Constantes por nivel
	public float DañoPL = 0.55f;
	public float VidaPL;
	public float VidaRegenPL;
	public float ManaPL;
	public float ManaRegenPL;
	public float ArmaduraPL;
	public float ResistenciaMagicaPL;
	
	public float TiempoDeRespawn;
	public float reducciondeRespawn;
	public int Skillponits = 1;
	public int Kills;
	public int Deaths;
	public int Assists;
	public int MasteriesPoints = 12;
	
	// Gold system
	private float passiveGoldTimer;
	private const float PASSIVE_GOLD_INTERVAL = 1.8f; // Cada 1.8 segundos
	private const int PASSIVE_GOLD_AMOUNT = 2; // 2 gold por tick
	
	// Damage tracking for assists
	private Dictionary<GameObject, DamageInfo> recentDamagers;
	
	// Bools
	public bool Isbot;
	public bool IsBuilding;
	public bool IsCreep;
	public bool IsPlayer;
	public bool IsJungle;
	public bool IsMascot;
	public bool IsMelee;
	public bool IsRanged;
	public bool IsURF;
	public bool IsAlive = true;
	
	// Efectos
	public GameObject AtaqueBasico;
	public Transform LanzadorDeMisiles;
	public AudioSource AtaqueBasicoS;
	public AudioSource SonidoMuerteS;
	public AudioClip[] SonidoAtaque;
	public AudioClip[] SonidoMuerte;
	public GameObject LevelUpFX;
	
	// Referencias cacheadas
	private MinionsAI chekteam;
	private IABots chekteamBots;
	private MascotAI mascotAI;
	private ClickToMove playerController;
	private JungleAI jungleScript;
	private Animator animator;
	private UnityEngine.AI.NavMeshAgent navAgent;
	private CapsuleCollider capsuleCollider;
	private Debuffs debuffs;
	
	public Transform Base;
	public GameObject Canvas;
	
	public ChampContainer things;
	private Announcer announcer;
	private GameObject levelUpInstance;
	private KillingSpreeSystem killingSpree;
	private InventorySystem inventory;
	
	// Detección de enemigos
	private Collider[] enemigosExp;
	private LayerMask enemigoEXP;
	
	// Constantes
	private const float EXP_DETECTION_RADIUS = 10f;
	private const float BASE_HEAL_RANGE = 5f;
	private const float BASE_HEAL_RATE = 5.5f;
	private const int MAX_LEVEL_NORMAL = 18;
	private const int MAX_LEVEL_URF = 30;
	private const float EXP_INCREMENT = 80f;
	private const float ASSIST_TIMEOUT = 10f; // 10 segundos para contar asistencia

	//estadisticas base
	float baseDaño ,baseVidaMax,baseManaMax,baseArmadura,baseResistenciaMagica,baseVidaRegen,baseManaRegen;



	// Helper class para tracking de daño
	private class DamageInfo
	{
		public GameObject attacker;
		public float timestamp;
		public float totalDamage;
		
		public DamageInfo(GameObject atk, float time, float dmg)
		{
			attacker = atk;
			timestamp = time;
			totalDamage = dmg;
		}
	}
	
	private void Awake()
	{
		// Cachear componentes
		animator = GetComponent<Animator>();
		navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		debuffs = GetComponent<Debuffs>();
		killingSpree = GetComponent<KillingSpreeSystem>();
		inventory = GetComponent<InventorySystem>();
		
		// Inicializar arrays y dictionaries
		enemigosExp = new Collider[20];
		recentDamagers = new Dictionary<GameObject, DamageInfo>();
	}
	
	private void Start() 
	{
		things = GameObject.Find("ChampContainer").GetComponent<ChampContainer>();
		announcer = GameObject.Find("Announcer").GetComponent<Announcer>();
		canceladordedamage = 1f;
		
		InitializeCharacterType();
		ConfigureURFMode();
		InitializeHealthMana();
		
		// Guardar stats base al inicio
		SaveBaseStats();
	}
	
	private void SaveBaseStats()
	{
		baseDaño = Daño;
		baseVidaMax = VidaMax;
		baseManaMax = ManaMax;
		baseArmadura = Armadura;
		baseResistenciaMagica = ResistenciaMagica;
		baseVidaRegen = VidaRegen;
		baseManaRegen = ManaRegen;
	}
	
	private void InitializeCharacterType()
	{
		if (Isbot)
		{
			chekteamBots = GetComponent<IABots>();
			if (chekteamBots != null)
			{
				chekteamBots.animator.SetBool("Isalive", IsAlive);
				if (IsCreep) chekteamBots.ultimopunto = transform;
			}
		}
		else if (IsCreep)
		{
			chekteam = GetComponent<MinionsAI>();
			if (chekteam != null)
			{
				chekteam.animator.SetBool("Isalive", IsAlive);
				chekteam.ultimopunto = transform;
			}
		}
		else if (IsMascot)
		{
			mascotAI = GetComponent<MascotAI>();
			if (mascotAI != null)
			{
				mascotAI.animator.SetBool("Isalive", IsAlive);
			}
		}
		else if (IsJungle)
		{
			jungleScript = GetComponent<JungleAI>();
			if (jungleScript != null)
			{
				jungleScript.animator.SetBool("Isalive", IsAlive);
			}
		}
		else if (IsPlayer)
		{
			playerController = GetComponent<ClickToMove>();
			if (playerController != null)
			{
				playerController.animator.SetBool("Isalive", IsAlive);
			}
		}
	}
	
	private void ConfigureURFMode()
	{
		if (things.URF)
		{
			VelocidadDeAtaqueExtra = 0.5f;
			VelocidadDeMovimiento = 25f;
		}
	}
	
	private void InitializeHealthMana()
	{
		if (Application.loadedLevel == 3)
		{
			Vida = VidaMax;
			Mana = ManaMax;
			Experiencia += 700f;
		}
	}
	
	private void Update() 
	{
		UpdateAnimator();
		ConfigureTeam();
		DetectEnemies();
		HandleRespawn();
		CheckDeath();
		
		if (Isbot || IsPlayer)
		{
			UpdateCanvas();
			UpdateExperience();
			RegenerateHealthMana();
			UpdatePassiveGold();
			ApplyItemBonuses(); // Aplicar bonuses cada frame
		}
		
		if (IsCreep || IsMascot)
		{
			ClampHealth();
		}
	}
	
	private void ApplyItemBonuses()
	{
		if (inventory == null) return;
		
		// Aplicar stats con bonuses de items
		Daño = baseDaño + inventory.GetTotalBonusStat("Damage");
		VidaMax = baseVidaMax + inventory.GetTotalBonusStat("Health");
		ManaMax = baseManaMax + inventory.GetTotalBonusStat("Mana");
		Armadura = baseArmadura + inventory.GetTotalBonusStat("Armor");
		ResistenciaMagica = baseResistenciaMagica + inventory.GetTotalBonusStat("MagicResist");
		VidaRegen = baseVidaRegen + inventory.GetTotalBonusStat("HealthRegen");
		ManaRegen = baseManaRegen + inventory.GetTotalBonusStat("ManaRegen");
		AbilityPower = inventory.GetTotalBonusStat("AbilityPower");
		VelocidadDeAtaqueExtra = inventory.GetTotalBonusStat("AttackSpeed");
		VelocidadDeMovimiento = 1f + inventory.GetTotalBonusStat("MovementSpeed");
		LifeSteal = inventory.GetTotalBonusStat("LifeSteal");
		
		// Limitar la vida actual si VidaMax se redujo
		if (Vida > VidaMax)
		{
			Vida = VidaMax;
		}
		
		// Limitar el mana actual si ManaMax se redujo
		if (Mana > ManaMax)
		{
			Mana = ManaMax;
		}
	}
	
	private void UpdateAnimator()
	{
		if (animator != null && (Isbot || IsCreep || IsPlayer || IsMascot || IsJungle))
		{
			animator.SetFloat("MS", 1 + debuffs.SpeedBoostModifier);
			animator.SetFloat("AS", 1 + debuffs.AttackSpeedBoostModifier + VelocidadDeAtaqueExtra);
			animator.SetBool("Isalive", IsAlive);
		}
	}
	
	private void ConfigureTeam()
	{
		if (gameObject.tag == "TeamRed")
		{
			if (Isbot || IsPlayer)
			{
				gameObject.layer = 21;
				if (Base == null)
				{
					GameObject well = GameObject.Find("Well Red");
					if (well != null) Base = well.transform;
				}
			}
			else if (IsCreep || IsMascot)
			{
				gameObject.layer = 12;
			}
			
			enemigoEXP = (1 << LayerMask.NameToLayer("TeamBlue"))
				| (1 << LayerMask.NameToLayer("Jungle"))
					| (1 << LayerMask.NameToLayer("ChampionBlue"));
		}
		else if (gameObject.tag == "TeamBlue")
		{
			if (Isbot || IsPlayer)
			{
				gameObject.layer = 20;
				if (Base == null)
				{
					GameObject well = GameObject.Find("Well Blue");
					if (well != null) Base = well.transform;
				}
			}
			else if (IsCreep || IsMascot)
			{
				gameObject.layer = 11;
			}
			
			enemigoEXP = (1 << LayerMask.NameToLayer("TeamRed"))
				| (1 << LayerMask.NameToLayer("Jungle"))
					| (1 << LayerMask.NameToLayer("ChampionRed"));
		}
	}
	
	private void HandleRespawn()
	{
		if (TiempoDeRespawn > 0f)
		{
			TiempoDeRespawn -= Time.deltaTime;
			capsuleCollider.enabled = false;
			if (navAgent != null) navAgent.enabled = false;
		}
		else if (TiempoDeRespawn < 0f)
		{
			Revivir();
		}
	}
	
	private void UpdateCanvas()
	{
		if (Canvas == null)
		{
			Canvas = GameObject.Find("CanvasGame");
		}
	}
	
	private void UpdateExperience()
	{
		if ((Application.loadedLevel == 3 || Application.loadedLevel == 4) && Nivel < MAX_LEVEL_NORMAL)
		{
			Experiencia += (0.5f + Nivel) * Time.deltaTime;
		}
		
		if (Experiencia > ExperienciaMax)
		{
			ScalingStats();
		}
	}
	
	private void RegenerateHealthMana()
	{
		if (!IsAlive) return;
		
		bool nearBase = Base != null && Vector3.Distance(transform.position, Base.position) <= BASE_HEAL_RANGE;
		
		// Regeneración de vida
		if (Vida < VidaMax)
		{
			if ((Application.loadedLevel == 2 || Application.loadedLevel == 4 || Application.loadedLevel == 5) && nearBase)
			{
				Vida += BASE_HEAL_RATE + (Nivel / 2f) * 2f;
			}
			else if (!nearBase)
			{
				Vida += VidaRegen * Time.deltaTime;
			}
		}
		
		Vida = Mathf.Clamp(Vida, 0, VidaMax);
		
		// Regeneración de mana
		if (Mana < ManaMax)
		{
			if ((Application.loadedLevel == 2 || Application.loadedLevel == 4 || Application.loadedLevel == 5) && nearBase)
			{
				Mana += BASE_HEAL_RATE + (Nivel / 2f) * 2f + ManaRegenBuff;
			}
			else if (!nearBase)
			{
				Mana += ManaRegen * Time.deltaTime + ManaRegenBuff;
			}
		}
		
		Mana = Mathf.Clamp(Mana, 0, ManaMax);
	}
	
	private void ClampHealth()
	{
		if (Vida > VidaMax) Vida = VidaMax;
	}
	
	private void UpdatePassiveGold()
	{
		if (inventory == null) return;
		
		passiveGoldTimer += Time.deltaTime;
		
		if (passiveGoldTimer >= PASSIVE_GOLD_INTERVAL)
		{
			inventory.AddGold(PASSIVE_GOLD_AMOUNT);
			passiveGoldTimer = 0f;
		}
	}
	
	public void RegisterDamage(GameObject attacker, float damage)
	{
		if (attacker == null || attacker == gameObject) return;
		
		// Solo registrar daño de campeones
		Character attackerChar = attacker.GetComponent<Character>();
		if (attackerChar == null || (!attackerChar.IsPlayer && !attackerChar.Isbot)) return;
		
		float currentTime = Time.time;
		
		if (recentDamagers.ContainsKey(attacker))
		{
			recentDamagers[attacker].timestamp = currentTime;
			recentDamagers[attacker].totalDamage += damage;
		}
		else
		{
			recentDamagers.Add(attacker, new DamageInfo(attacker, currentTime, damage));
		}
		
		// Limpiar atacantes antiguos
		CleanupOldDamagers();
	}
	
	private void CleanupOldDamagers()
	{
		float currentTime = Time.time;
		
		// Crear lista de keys a eliminar
		System.Collections.Generic.List<GameObject> toRemove = new System.Collections.Generic.List<GameObject>();
		
		foreach (var kvp in recentDamagers)
		{
			if (currentTime - kvp.Value.timestamp > ASSIST_TIMEOUT)
			{
				toRemove.Add(kvp.Key);
			}
		}
		
		// Eliminar los viejos
		foreach (GameObject key in toRemove)
		{
			recentDamagers.Remove(key);
		}
	}
	
	public void AtaqueBasicoMisil()
	{
		GameObject objetivo = GetCurrentTarget();
		if (objetivo == null) return;
		
		// Revelar al atacante si está invisible
		Debuffs myDebuffs = GetComponent<Debuffs>();
		if (myDebuffs != null)
		{
			myDebuffs.OnAttackPerformed();
		}
		
		if (IsRanged)
		{
			SpawnRangedAttack(objetivo);
		}
		else if (IsMelee)
		{
			ExecuteMeleeAttack(objetivo);
		}
		
		PlayAttackSound();
	}
	
	private GameObject GetCurrentTarget()
	{
		if (Isbot && chekteamBots != null) return chekteamBots.Objetivo;
		if (IsCreep && chekteam != null) return chekteam.Objetivo;
		if (IsMascot && mascotAI != null) return mascotAI.Objetivo;
		if (IsJungle && jungleScript != null) return jungleScript.Objetivo;
		if (IsPlayer && playerController != null) return playerController.Objetivo;
		return null;
	}
	
	private void SpawnRangedAttack(GameObject target)
	{
		GameObject missile = Instantiate(AtaqueBasico, LanzadorDeMisiles.position, LanzadorDeMisiles.rotation);
		MissilGenerico missilScript = missile.GetComponent<MissilGenerico>();
		
		if (missilScript != null)
		{
			missilScript.TargetMisil = target;
			missilScript.MyOwner = gameObject;
			missilScript.damage = (Daño + debuffs.damageboost) * canceladordedamage;
		}
	}
	
	private void ExecuteMeleeAttack(GameObject target)
	{
		if (target == null) return;
		
		Character targetChar = target.GetComponent<Character>();
		if (targetChar == null) return;
		
		float totalArmor = targetChar.Armadura + targetChar.armaduraTemporal;
		float armorReduction = (totalArmor * 100f) / (totalArmor + 100f);
		float damage = Mathf.Max(0, ((Daño + debuffs.damageboost) - armorReduction) * canceladordedamage);
		
		// Registrar el daño para sistema de asistencias
		targetChar.RegisterDamage(gameObject, damage);
		
		// Revelar al objetivo si recibe daño
		Debuffs targetDebuffs = target.GetComponent<Debuffs>();
		if (targetDebuffs != null)
		{
			targetDebuffs.OnDamageTaken();
		}
		
		// Life steal
		if (LifeSteal > 0 && IsAlive)
		{
			Vida += (damage * LifeSteal) / 100f;
		}
		
		// Marcar jungle como atacado
		if (targetChar.IsJungle && targetChar.jungleScript != null)
		{
			targetChar.jungleScript.Isattacked = true;
		}
		
		targetChar.Vida -= damage;
		
		// Dar experiencia y oro si mata
		if (targetChar.Vida <= 0 && (Isbot || IsPlayer))
		{
			GainKillExperience(targetChar);
		}
	}
	
	public void GainKillExperience(Character target)
	{
		if (target == null) return;
		
		// Determinar quién se lleva el crédito por la kill
		GameObject killer = DetermineKiller(target);
		
		if (target.Isbot || target.IsPlayer)
		{
			int goldReward = 300;
			float expReward = (target.ExperienciaMax - target.Experiencia) / 2f;
			
			// Kill de campeón
			if (killer != null)
			{
				Character killerChar = killer.GetComponent<Character>();
				if (killerChar != null)
				{
					// Dar oro y exp al killer
					killerChar.Experiencia += expReward;
					InventorySystem killerInv = killerChar.GetComponent<InventorySystem>();
					if (killerInv != null)
					{
						killerInv.AddGold(goldReward);
					}
					
					// Actualizar estadísticas
					killerChar.Kills++;
					
					// Tracking de killing spree
					KillingSpreeSystem killerSpree = killerChar.GetComponent<KillingSpreeSystem>();
					if (killerSpree != null)
					{
						killerSpree.OnKill(target);
					}
					
					Debug.Log(killerChar.name + " killed " + target.name);
				}
			}
			
			// Dar asistencias
			GiveAssists(target, killer, goldReward / 2, expReward / 3f);
		}
		else if (target.IsCreep || target.IsMascot)
		{
			// Kill de minion
			int goldReward = Random.Range(18, 24);
			float expReward = Random.Range(30f, 60f) + target.Nivel;
			
			if (killer != null)
			{
				Character killerChar = killer.GetComponent<Character>();
				if (killerChar != null)
				{
					killerChar.Experiencia += expReward;
					InventorySystem killerInv = killerChar.GetComponent<InventorySystem>();
					if (killerInv != null)
					{
						killerInv.AddGold(goldReward);
					}
				}
			}
		}
		else if (target.IsJungle)
		{
			// Kill de jungle monster
			int goldReward = Random.Range(40, 80);
			float expReward = Random.Range(50f, 90f) + target.Nivel;
			
			if (killer != null)
			{
				Character killerChar = killer.GetComponent<Character>();
				if (killerChar != null)
				{
					killerChar.Experiencia += expReward;
					InventorySystem killerInv = killerChar.GetComponent<InventorySystem>();
					if (killerInv != null)
					{
						killerInv.AddGold(goldReward);
					}
				}
			}
		}
		
		ScalingStats();
	}
	
	private GameObject DetermineKiller(Character victim)
	{
		if (victim == null) return null;
		
		// Si yo le di el golpe final y soy campeón, soy el killer
		if (Isbot || IsPlayer)
		{
			return gameObject;
		}
		
		// Si soy un minion, buscar al campeón que más daño hizo recientemente
		if (IsCreep || IsMascot)
		{
			GameObject bestAttacker = null;
			float highestDamage = 0f;
			float currentTime = Time.time;
			
			foreach (var kvp in victim.recentDamagers)
			{
				if (kvp.Value == null || kvp.Key == null) continue;
				
				// Solo considerar daño de los últimos 3 segundos para last hit
				if (currentTime - kvp.Value.timestamp <= 3f)
				{
					if (kvp.Value.totalDamage > highestDamage)
					{
						highestDamage = kvp.Value.totalDamage;
						bestAttacker = kvp.Key;
					}
				}
			}
			
			return bestAttacker;
		}
		
		return gameObject;
	}
	
	private void GiveAssists(Character victim, GameObject killer, int assistGold, float assistExp)
	{
		if (victim == null || victim.recentDamagers == null) return;
		
		foreach (var kvp in victim.recentDamagers)
		{
			if (kvp.Key == null || kvp.Key == killer) continue;
			
			Character assister = kvp.Key.GetComponent<Character>();
			if (assister == null) continue;
			
			// Dar oro y exp de asistencia
			assister.Experiencia += assistExp;
			assister.Assists++;
			
			InventorySystem assisterInv = assister.GetComponent<InventorySystem>();
			if (assisterInv != null)
			{
				assisterInv.AddGold(assistGold);
			}
			
			Debug.Log(assister.name + " assisted in killing " + victim.name);
		}
	}
	
	private void PlayAttackSound()
	{
		if (AtaqueBasicoS != null && SonidoAtaque.Length > 0)
		{
			AtaqueBasicoS.clip = SonidoAtaque[Random.Range(0, SonidoAtaque.Length)];
			AtaqueBasicoS.Play();
		}
	}
	
	public void SonidoMuerteF()
	{
		if (SonidoMuerteS != null && SonidoMuerte.Length > 0)
		{
			SonidoMuerteS.clip = SonidoMuerte[Random.Range(0, SonidoMuerte.Length)];
			SonidoMuerteS.Play();
		}
	}
	
	private void Revivir()
	{
		IsAlive = true;
		TiempoDeRespawn = 0;
		Vida = VidaMax;
		Mana = ManaMax;
		
		if (Isbot && chekteamBots != null)
		{
			ReviveBot();
		}
		else if (IsPlayer && playerController != null)
		{
			RevivePlayer();
		}
	}
	
	private void ReviveBot()
	{
		chekteamBots.punto = 0;
		chekteamBots.Objetivo = null;
		chekteamBots.ultimopunto = chekteamBots.camino[0];
		chekteamBots.animator.SetBool("IsAttacking", false);
		chekteamBots.animator.SetBool("Isalive", true);
		chekteamBots.animator.SetFloat("State", 0);
		
		RespawnAtBase(chekteamBots.EquipoRojo);
	}
	
	private void RevivePlayer()
	{
		playerController.Objetivo = null;
		playerController.animator.SetBool("IsAttacking", false);
		playerController.animator.SetBool("Isalive", true);
		playerController.animator.SetFloat("State", 0);
		
		RespawnAtBase(gameObject.layer == 21);
	}
	
	private void RespawnAtBase(bool isRedTeam)
	{
		string wellName = isRedTeam ? "Well Red" : "Well Blue";
		GameObject well = GameObject.Find(wellName);
		
		if (well != null)
		{
			transform.position = well.transform.position;
			if (playerController != null) playerController.Position = well.transform.position;
			
			AudioSource respawn = well.GetComponent<AudioSource>();
			if (respawn != null) respawn.Play();
		}
		
		capsuleCollider.enabled = true;
		if (navAgent != null)
		{
			navAgent.enabled = true;
			navAgent.Warp(Base.position);
		}
	}
	
	public void Muerte2()
	{
		DarExpEnemy();
		CalculateRespawnTime();
		HandleScoring();
		
		// End killing spree when dying
		if (killingSpree != null)
		{
			killingSpree.EndSpree();
		}
	}
	
	private void CalculateRespawnTime()
	{
		if (Isbot || IsPlayer)
		{
			if (Application.loadedLevel == 2 || Application.loadedLevel == 3 || Application.loadedLevel == 5)
			{
				TiempoDeRespawn = things.URF ? 15f : (2.5f * Nivel) + 7.5f - reducciondeRespawn;
			}
			else if (Application.loadedLevel == 4)
			{
				TiempoDeRespawn = things.URF ? 15f : (1.5f * Nivel) + 4.5f - reducciondeRespawn;
				
				if (Canvas != null)
				{
					SpecialMapMeshanics mechanics = Canvas.GetComponent<SpecialMapMeshanics>();
					if (mechanics != null)
					{
						if (gameObject.tag == "TeamRed")
							mechanics.ScorePurple -= 2f;
						else if (gameObject.tag == "TeamBlue")
							mechanics.ScoreBlue -= 2f;
					}
				}
			}
		}
	}
	
	private void HandleScoring()
	{
		if (Canvas == null || announcer == null) return;
		
		ClassicMode classicMode = Canvas.GetComponent<ClassicMode>();
		if (classicMode == null) return;
		
		if (Isbot && chekteamBots != null)
		{
			UpdateBotScore(classicMode, chekteamBots.EquipoRojo);
		}
		else if (IsPlayer && playerController != null)
		{
			announcer.PlayKilled();
			UpdatePlayerScore(classicMode, playerController.EquipoRojo);
		}
	}
	
	private void UpdateBotScore(ClassicMode mode, bool isRedTeam)
	{
		if (things.team == 0) // Azul
		{
			if (isRedTeam)
			{
				announcer.EKill();
				mode.BlueScore++;
			}
			else
			{
				announcer.AKill();
				mode.RedScore++;
			}
		}
		else if (things.team == 1) // Rojo
		{
			if (isRedTeam)
			{
				announcer.AKill();
				mode.BlueScore++;
			}
			else
			{
				announcer.EKill();
				mode.RedScore++;
			}
		}
	}
	
	private void UpdatePlayerScore(ClassicMode mode, bool isRedTeam)
	{
		if (things.team == 0)
		{
			if (isRedTeam) mode.BlueScore++;
			else mode.RedScore++;
		}
		else if (things.team == 1)
		{
			if (isRedTeam) mode.BlueScore++;
			else mode.RedScore++;
		}
	}
	
	private void CheckDeath()
	{
		if (Vida <= 0)
		{
			Vida = 0;
			IsAlive = false;
			capsuleCollider.enabled = false;
			if (navAgent != null) navAgent.enabled = false;
			
			if (IsBuilding)
			{
				HandleBuildingDeath();
			}
			else if (IsJungle && jungleScript != null)
			{
				jungleScript.IsAlive = false;
				jungleScript.Isattacked = false;
			}
		}
	}
	
	private void HandleBuildingDeath()
	{
		gameObject.layer = 0;
		gameObject.tag = "Untagged";
		TorretaRojaAI turret = GetComponent<TorretaRojaAI>();
		if (turret != null) turret.IsAlive = false;
	}
	
	public void ScalingStats()
	{
		if (!IsPlayer && !Isbot) return;
		
		int maxLevel = things.URF ? MAX_LEVEL_URF : MAX_LEVEL_NORMAL;
		
		if (Nivel < maxLevel && Experiencia >= ExperienciaMax)
		{
			SpawnLevelUpEffect();
			ApplyLevelUpStats();
			Experiencia -= ExperienciaMax;
			ExperienciaMax += EXP_INCREMENT;
			Nivel++;
			Skillponits++;
			if (IsPlayer) MasteriesPoints++;
		}
	}
	
	private void SpawnLevelUpEffect()
	{
		if (levelUpInstance == null)
		{
			levelUpInstance = Instantiate(LevelUpFX, transform.position, transform.rotation);
		}
		else
		{
			levelUpInstance.SetActive(true);
			levelUpInstance.transform.position = transform.position;
		}
	}
	
	private void ApplyLevelUpStats()
	{
		// Actualizar stats BASE, no las totales
		baseDaño += DañoPL;
		baseVidaMax += VidaPL;
		baseManaMax += ManaPL;
		baseVidaRegen += VidaRegenPL;
		baseManaRegen += ManaRegenPL;
		baseArmadura += ArmaduraPL;
		
		// Las stats totales se actualizarán en el próximo frame con ApplyItemBonuses()
	}
	
	public void DeathCreep()
	{
		PooledMinion pooled = GetComponent<PooledMinion>();
		if (pooled != null) pooled.Die();
	}
	
	public void DeathMascot()
	{
		gameObject.SetActive(false);
	}
	
	public void DeathJungle()
	{
		if (jungleScript != null && jungleScript.RespawnPoint != null)
		{
			JungleSpawn spawn = jungleScript.RespawnPoint.GetComponent<JungleSpawn>();
			if (spawn != null && spawn.MJS.Amount > 0)
			{
				spawn.MJS.Amount--;
			}
		}
		Destroy(gameObject);
	}
	
	private void DetectEnemies() 
	{
		int count = Physics.OverlapSphereNonAlloc(transform.position, EXP_DETECTION_RADIUS, enemigosExp, enemigoEXP);
	}
	
	private void DarExpEnemy()
	{
		if (Application.loadedLevel != 2 && Application.loadedLevel != 3 && 
		    Application.loadedLevel != 4 && Application.loadedLevel != 5) return;
		
		int count = Physics.OverlapSphereNonAlloc(transform.position, EXP_DETECTION_RADIUS, enemigosExp, enemigoEXP);
		
		for (int i = 0; i < count; i++)
		{
			Character enemyChar = enemigosExp[i].GetComponent<Character>();
			if (enemyChar != null)
			{
				enemyChar.Experiencia += CalculateExpReward();
			}
		}
	}
	
	private float CalculateExpReward()
	{
		if (Application.loadedLevel == 4)
		{
			if (Isbot || IsPlayer) return 25f * Nivel + ExperienciaExtra;
			if (IsMascot) return 15f + Nivel + ExperienciaExtra;
		}
		else
		{
			if (Isbot || IsPlayer) return 10f * Nivel + ExperienciaExtra;
			if (IsCreep || IsMascot) return 10f + Nivel + ExperienciaExtra;
			if (IsJungle) return 20f + Nivel + ExperienciaExtra;
		}
		return 0f;
	}
}