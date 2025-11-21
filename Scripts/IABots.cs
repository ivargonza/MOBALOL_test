using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class IABots : MonoBehaviour
{
	// === CAMPOS ORIGINALES (mantener compatibilidad) ===
	public Transform[] camino;
	public Collider[] Targets;
	public Collider[] Aliados;
	public Collider[] Supplies;
	public Collider[] CapturePoint;
	public LayerMask Enemigo;
	public LayerMask Aliado;
	public LayerMask Supplie;
	public LayerMask CaptureP;
	
	public bool EquipoRojo;
	public bool ItsSafe;
	public bool Mid;
	public bool Bot;
	public bool Top;
	public bool SpecialMap;
	public bool AttackedByturret;
	
	public float Rango;
	public float RangoDeDeteccion = 6f;
	public GameObject CapPo;
	public GameObject Objetivo;
	public NavMeshAgent nav;
	public Animator animator;
	
	public Transform ultimopunto;
	public int punto;
	public int N;
	int ATT;
	Vector3 RandomPos;
	public float Feartime;
	public float timerATT;
	
	float ranptimer = 1f;
	public bool obsetted;
	
	public float VidaPerc;
	public float TimeLoop = 0.5f;
	public float RanPerLife;
	float timeCL;
	
	// === NUEVOS CAMPOS PARA IA MEJORADA ===
	[Header("=== CONFIGURACION MEJORADA ===")]
	public BotBuildConfig buildConfig;
	
	[Header("Deteccion Mejorada")]
	public float detectionRadius = 12f;
	public LayerMask turretLayerMask;
	
	[Header("Sistema de Items")]
	public bool autoCompraItems = true;
	public float itemCheckInterval = 5f;
	
	[Header("Debug")]
	public bool showDebugLogs = false;
	public BotState estadoActual = BotState.Laning;
	
	// Componentes cacheados
	private Character _character;
	private Debuffs _debuffs;
	private Recall _recall;
	private InventorySystem _inventory;
	
	// Sistema de estados
	public enum BotState
	{
		Laning,
		Fighting,
		Retreating,
		Pushing
	}
	
	// Tracking de items
	private int currentItemIndex = 0;
	private bool boughtStarterItems = false;
	private float itemTimer = 0f;
	
	// Anti-stuck mejorado
	private Vector3 lastPosition;
	private float stuckTimer = 0f;
	private int stuckCount = 0;
	private float lastMoveTime = 0f;
	private const float STUCK_CHECK_INTERVAL = 1.5f;
	private const float STUCK_THRESHOLD = 0.3f;
	private const int MAX_STUCK_COUNT = 3;
	
	// Target priority cache
	private List<TargetInfo> prioritizedTargets = new List<TargetInfo>();
	
	private class TargetInfo
	{
		public GameObject target;
		public float priority;
		public float distance;
		
		public TargetInfo(GameObject t, float p, float d)
		{
			target = t;
			priority = p;
			distance = d;
		}
	}
	
	void Awake()
	{
		nav = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		_character = GetComponent<Character>();
		_debuffs = GetComponent<Debuffs>();
		_recall = GetComponent<Recall>();
		_inventory = GetComponent<InventorySystem>();
		
		prioritizedTargets = new List<TargetInfo>();
	}
	
	void Start()
	{
		StartCoroutine(SpecialMapInitialize());
		RanPerLife = Random.Range(10f, 50f);
		lastPosition = transform.position;
		lastMoveTime = Time.time;
		
		if (buildConfig != null)
		{
			RanPerLife = buildConfig.healthRetreatThreshold;
		}
	}
	
	void OnEnable()
	{
		if (nav == null) nav = GetComponent<NavMeshAgent>();
		if (animator == null) animator = GetComponent<Animator>();
		if (_character == null) _character = GetComponent<Character>();
		if (_debuffs == null) _debuffs = GetComponent<Debuffs>();
		if (_recall == null) _recall = GetComponent<Recall>();
		if (_inventory == null) _inventory = GetComponent<InventorySystem>();
		
		RandomizePos();
		ItsSafe = true;
		SetPath();
		timeCL = Random.Range(180f, 300f);
		InvokeRepeating("RandLaneF", Random.Range(120f, 180f), timeCL);
	}
	
	void Update()
	{
		if (_character == null || !_character.IsAlive) return;
		
		UpdateTeamLayers();
		
		// Timers
		if (ranptimer > 0f) ranptimer -= Time.deltaTime;
		if (ranptimer < 0f)
		{
			ranptimer = Random.Range(1f, 5f);
			RandomizePos();
			AttackedByturret = false;
		}
		
		if (nav == null || !nav.enabled) return;
		
		// Deteccion
		FindTargets();
		FindAllies();
		FindSupplies();
		FindCapturePoint();
		
		// Calculos
		CalcularVida();
		SafeCheck();
		
		// Anti-stuck mejorado
		CheckIfStuck();
		
		// Fear timer
		if (Feartime > 0f) Feartime -= Time.deltaTime;
		if (Feartime <= 0f)
		{
			Feartime = 0f;
			ItsSafe = true;
			AttackedByturret = false;
		}
		
		// === SISTEMA DE ESTADOS MEJORADO ===
		EvaluateState();
		ExecuteState();
		
		// Sistema de items
		if (autoCompraItems)
		{
			UpdateItemPurchase();
		}
	}
	
	void EvaluateState()
	{
		float healthThreshold = (buildConfig != null) ? buildConfig.healthRetreatThreshold : RanPerLife;
		float engageThreshold = (buildConfig != null) ? buildConfig.healthEngageThreshold : 50f;
		
		bool mapaSinRegenBase = (Application.loadedLevel == 2);
		
		int enemyMinions = CountEnemyMinions();
		int enemyChamps = CountEnemyChampions();
		int allyMinions = CountAllyMinions();
		
		// Prioridad 1: Retreat si vida baja
		if (VidaPerc < healthThreshold && VidaPerc > 0)
		{
			ChangeState(BotState.Retreating);
			return;
		}
		
		// Prioridad 1.5: Retreat si muchos minions enemigos sin aliados
		if (enemyMinions > allyMinions + 3 && enemyChamps == 0 && VidaPerc < 60f)
		{
			ChangeState(BotState.Retreating);
			return;
		}
		
		// Comprar items si es posible
		bool puedeComprar = false;
		if (mapaSinRegenBase)
		{
			puedeComprar = (Supplies != null && Supplies.Length > 0);
		}
		else
		{
			puedeComprar = IsNearBase();
		}
		
		if (puedeComprar && _inventory != null && _inventory.GetCurrentGold() > 300)
		{
			TryBuyItemsNow();
		}
		
		// Prioridad 3: Combate si hay enemigos
		if (Targets != null && Targets.Length > 0 && VidaPerc >= engageThreshold)
		{
			if (ShouldEngage())
			{
				ChangeState(BotState.Fighting);
				return;
			}
		}
		
		// Prioridad 4: Volver a laning
		if (estadoActual == BotState.Fighting && (Targets == null || Targets.Length == 0))
		{
			ChangeState(BotState.Laning);
		}
		
		if (estadoActual == BotState.Retreating)
		{
			if (mapaSinRegenBase)
			{
				if (VidaPerc > 70f)
				{
					ChangeState(BotState.Laning);
				}
			}
			else
			{
				if (VidaPerc > 80f && IsNearBase())
				{
					ChangeState(BotState.Laning);
				}
			}
		}
	}
	
	void TryBuyItemsNow()
	{
		if (_inventory == null || buildConfig == null) return;
		if (ItemSystem.Instance == null) return;
		
		if (!boughtStarterItems)
		{
			TryBuyStarterItems();
		}
		
		TryBuyNextItem();
	}
	
	bool ShouldEngage()
	{
		if (Targets == null || Targets.Length == 0) return false;
		
		int enemyChamps = CountEnemyChampions();
		int enemyMinions = CountEnemyMinions();
		int allyChamps = CountAllyChampions();
		int allyMinions = CountAllyMinions();
		
		// Verificar torreta enemiga cercana
		if (buildConfig != null && buildConfig.avoidTurrets)
		{
			Collider[] turrets = Physics.OverlapSphere(transform.position, 10f, turretLayerMask);
			if (turrets != null && turrets.Length > 0)
			{
				if (allyMinions < 3)
				{
					return false;
				}
			}
		}
		
		// SIEMPRE pelear si solo hay minions enemigos
		if (enemyChamps == 0 && enemyMinions > 0)
		{
			return true;
		}
		
		// No pelear si muy outnumbered por campeones
		if (enemyChamps > allyChamps + 1) return false;
		
		// Ventaja numerica de campeones
		if (allyChamps >= enemyChamps) return true;
		
		// Caso neutral: depende de agresividad
		if (buildConfig != null)
		{
			float roll = Random.value * 100f;
			return roll < buildConfig.aggressiveness;
		}
		
		return VidaPerc > RanPerLife && ItsSafe;
	}
	
	void ChangeState(BotState newState)
	{
		if (estadoActual == newState) return;
		
		if (showDebugLogs)
		{
			Debug.Log(string.Format("{0}: {1} -> {2}", name, estadoActual, newState));
		}
		
		if (estadoActual == BotState.Fighting)
		{
			Objetivo = null;
			animator.SetBool("IsAttacking", false);
		}
		
		estadoActual = newState;
		
		if (newState == BotState.Laning)
		{
			RandomizePos();
			stuckCount = 0;
		}
	}
	
	void ExecuteState()
	{
		switch (estadoActual)
		{
		case BotState.Laning:
			ExecuteLaning();
			break;
		case BotState.Fighting:
			ExecuteFighting();
			break;
		case BotState.Retreating:
			ExecuteRetreating();
			break;
		case BotState.Pushing:
			ExecuteLaning();
			break;
		}
	}
	
	void ExecuteLaning()
	{
		if (_debuffs != null && _debuffs.Fear) return;
		if (camino == null || camino.Length == 0) return;
		
		// Asegurar que nav no esté detenido
		if (nav.isStopped)
		{
			nav.isStopped = false;
		}
		
		// Validar punto
		if (punto < 0) punto = 0;
		if (punto >= camino.Length) punto = camino.Length - 1;
		
		Transform targetPoint = camino[punto];
		if (targetPoint == null)
		{
			punto = Mathf.Min(punto + 1, camino.Length - 1);
			return;
		}
		
		Vector3 destino = targetPoint.position + RandomPos;
		float dist = Vector3.Distance(transform.position, destino);
		
		if (dist > 2f)
		{
			nav.isStopped = false;
			nav.SetDestination(destino);
			ultimopunto = targetPoint;
			animator.SetFloat("State", 1f);
			animator.SetBool("IsAttacking", false);
		}
		else
		{
			// Llegamos al punto, avanzar
			if (SpecialMap)
			{
				punto = Random.Range(0, camino.Length);
			}
			else
			{
				punto = Mathf.Min(punto + 1, camino.Length - 1);
			}
			
			N = Mathf.Max(0, punto - 1);
			RandomizePos();
			
			// NO detenerse, seguir al siguiente punto
			if (punto < camino.Length && camino[punto] != null)
			{
				nav.SetDestination(camino[punto].position + RandomPos);
			}
		}
	}
	
	void ExecuteFighting()
	{
		Objetivo = SelectBestTarget();
		
		if (Objetivo == null)
		{
			ChangeState(BotState.Laning);
			return;
		}
		
		Character targetChar = Objetivo.GetComponent<Character>();
		Debuffs targetDeb = Objetivo.GetComponent<Debuffs>();
		
		if (targetChar == null || !targetChar.IsAlive || 
		    (targetDeb != null && (targetDeb.Invulnerable || targetDeb.Invisible)))
		{
			Objetivo = null;
			ChangeState(BotState.Laning);
			return;
		}
		
		float dist = Vector3.Distance(transform.position, Objetivo.transform.position);
		
		if (dist <= Rango && targetChar.Vida > 0)
		{
			animator.SetBool("IsAttacking", true);
			nav.isStopped = true;
			
			Vector3 lookPos = new Vector3(
				Objetivo.transform.position.x,
				transform.position.y,
				Objetivo.transform.position.z
				);
			transform.LookAt(lookPos);
		}
		else if (dist > Rango && dist < detectionRadius)
		{
			nav.isStopped = false;
			animator.SetBool("IsAttacking", false);
			animator.SetFloat("State", 1f);
			nav.SetDestination(Objetivo.transform.position);
		}
		else
		{
			Objetivo = null;
			animator.SetBool("IsAttacking", false);
			ChangeState(BotState.Laning);
		}
	}
	
	GameObject SelectBestTarget()
	{
		if (Targets == null || Targets.Length == 0) return null;
		
		prioritizedTargets.Clear();
		Vector3 myPos = transform.position;
		
		foreach (Collider col in Targets)
		{
			if (col == null) continue;
			
			Character targetChar = col.GetComponent<Character>();
			Debuffs targetDeb = col.GetComponent<Debuffs>();
			
			if (targetChar == null || !targetChar.IsAlive) continue;
			if (targetDeb != null && (targetDeb.Invulnerable || targetDeb.Invisible)) continue;
			
			float dist = Vector3.Distance(myPos, col.transform.position);
			float priority = CalculateTargetPriority(col.gameObject, targetChar, dist);
			
			prioritizedTargets.Add(new TargetInfo(col.gameObject, priority, dist));
		}
		
		if (prioritizedTargets.Count == 0) return null;
		
		prioritizedTargets.Sort((a, b) => b.priority.CompareTo(a.priority));
		
		return prioritizedTargets[0].target;
	}
	
	float CalculateTargetPriority(GameObject target, Character targetChar, float distance)
	{
		float priority = 0f;
		
		priority += (detectionRadius - distance) * 5f;
		
		if (targetChar.IsPlayer || targetChar.Isbot)
		{
			priority += 100f;
			
			if (buildConfig != null && buildConfig.prioritizeLowHealthEnemies)
			{
				float hpPercent = (targetChar.Vida / targetChar.VidaMax) * 100f;
				if (hpPercent < 25f)
				{
					priority += 150f;
				}
				else if (hpPercent < 50f)
				{
					priority += 50f;
				}
			}
		}
		else if (targetChar.IsCreep)
		{
			if (_character != null && targetChar.Vida <= _character.Daño * 1.5f)
			{
				priority += 80f;
			}
			else
			{
				priority += 20f;
			}
		}
		else if (targetChar.IsJungle)
		{
			priority += 30f;
		}
		else if (targetChar.IsBuilding)
		{
			priority += 10f;
		}
		
		return priority;
	}
	
	void ExecuteRetreating()
	{
		bool mapaSinRegenBase = (Application.loadedLevel == 3);
		
		animator.SetBool("IsAttacking", false);
		nav.isStopped = false;
		
		if (mapaSinRegenBase)
		{
			if (Supplies != null && Supplies.Length > 0)
			{
				Collider nearestSupply = GetNearestCollider(Supplies);
				if (nearestSupply != null)
				{
					float distSupply = Vector3.Distance(transform.position, nearestSupply.transform.position);
					
					nav.SetDestination(nearestSupply.transform.position);
					animator.SetFloat("State", 1f);
					
					if (distSupply < 2f)
					{
						animator.SetFloat("State", 0f);
					}
					return;
				}
			}
			
			RetrocederPorCamino();
			return;
		}
		
		if (_character != null && _character.Base != null)
		{
			float distBase = Vector3.Distance(transform.position, _character.Base.position);
			
			if (distBase > 3f)
			{
				nav.SetDestination(_character.Base.position);
				animator.SetFloat("State", 1f);
			}
			else
			{
				animator.SetFloat("State", 0f);
			}
		}
		else
		{
			RetrocederPorCamino();
		}
	}
	
	void RetrocederPorCamino()
	{
		if (camino == null || camino.Length == 0) return;
		
		int retreatIndex = Mathf.Max(0, N - 1);
		
		if (retreatIndex >= 0 && retreatIndex < camino.Length)
		{
			Transform retreatPoint = camino[retreatIndex];
			if (retreatPoint != null && _character != null && _character.Vida > 0)
			{
				float distRetreat = Vector3.Distance(transform.position, retreatPoint.position);
				
				nav.isStopped = false;
				nav.SetDestination(retreatPoint.position);
				animator.SetFloat("State", 1f);
				
				if (distRetreat < 3f)
				{
					N = Mathf.Max(0, N - 1);
					punto = Mathf.Max(0, punto - 1);
					RandomizePos();
				}
			}
		}
		else
		{
			Transform firstPoint = camino[0];
			if (firstPoint != null)
			{
				nav.isStopped = false;
				nav.SetDestination(firstPoint.position);
				animator.SetFloat("State", 1f);
			}
		}
	}
	
	// === ANTI-STUCK MEJORADO ===
	void CheckIfStuck()
	{
		stuckTimer += Time.deltaTime;
		if (stuckTimer < STUCK_CHECK_INTERVAL) return;
		stuckTimer = 0f;
		
		float distMoved = Vector3.Distance(transform.position, lastPosition);
		
		// Si estamos atacando, no contar como stuck
		if (estadoActual == BotState.Fighting && Objetivo != null)
		{
			lastPosition = transform.position;
			stuckCount = 0;
			return;
		}
		
		if (distMoved < STUCK_THRESHOLD)
		{
			stuckCount++;
			
			if (showDebugLogs)
			{
				Debug.Log(string.Format("{0} posible stuck ({1}/{2})", name, stuckCount, MAX_STUCK_COUNT));
			}
			
			if (stuckCount >= MAX_STUCK_COUNT)
			{
				ResolveStuck();
				stuckCount = 0;
			}
		}
		else
		{
			stuckCount = 0;
			lastMoveTime = Time.time;
		}
		
		lastPosition = transform.position;
	}
	
	void ResolveStuck()
	{
		if (showDebugLogs)
		{
			Debug.Log(string.Format("{0} STUCK - Resolviendo...", name));
		}
		
		// Resetear navegacion
		if (nav != null && nav.enabled)
		{
			nav.isStopped = true;
			nav.ResetPath();
			nav.isStopped = false;
		}
		
		// Limpiar objetivo
		Objetivo = null;
		animator.SetBool("IsAttacking", false);
		
		// Randomizar posicion offset
		RandomizePos();
		
		// Forzar movimiento
		if (camino != null && camino.Length > 0)
		{
			// Intentar ir al siguiente punto o retroceder
			if (punto < camino.Length - 1)
			{
				punto++;
			}
			else if (punto > 0)
			{
				punto--;
			}
			
			Transform targetPoint = camino[Mathf.Clamp(punto, 0, camino.Length - 1)];
			if (targetPoint != null)
			{
				Vector3 newDest = targetPoint.position + RandomPos;
				nav.SetDestination(newDest);
			}
		}
		
		// Cambiar a Laning
		estadoActual = BotState.Laning;
		animator.SetFloat("State", 1f);
		
		// Intentar warp si sigue muy stuck
		if (Time.time - lastMoveTime > 10f)
		{
			if (showDebugLogs)
			{
				Debug.Log(string.Format("{0} WARP - Sin movimiento por 10s", name));
			}
			
			if (camino != null && camino.Length > 0 && camino[0] != null)
			{
				nav.Warp(camino[0].position);
				punto = 0;
				N = 0;
			}
			
			lastMoveTime = Time.time;
		}
	}
	
	// === SISTEMA DE COMPRA DE ITEMS ===
	void UpdateItemPurchase()
	{
		if (_inventory == null || buildConfig == null) return;
		if (ItemSystem.Instance == null) return;
		
		itemTimer += Time.deltaTime;
		if (itemTimer < itemCheckInterval) return;
		itemTimer = 0f;
		
		bool mapaSinRegenBase = (Application.loadedLevel == 2);
		bool puedeComprar = false;
		
		if (mapaSinRegenBase)
		{
			puedeComprar = (Supplies != null && Supplies.Length > 0);
		}
		else
		{
			puedeComprar = IsNearBase();
		}
		
		if (!puedeComprar) return;
		
		if (!boughtStarterItems)
		{
			TryBuyStarterItems();
		}
		
		TryBuyNextItem();
	}
	
	void TryBuyStarterItems()
	{
		if (buildConfig.starterItemIDs == null || buildConfig.starterItemIDs.Count == 0)
		{
			boughtStarterItems = true;
			return;
		}
		
		bool boughtAll = true;
		
		foreach (int itemID in buildConfig.starterItemIDs)
		{
			if (_inventory.HasItem(itemID)) continue;
			
			Item item = ItemSystem.Instance.GetItemById(itemID);
			if (item == null) continue;
			
			if (_inventory.CanAfford(item.cost))
			{
				bool success = _inventory.BuyItem(itemID);
				if (success && showDebugLogs)
				{
					Debug.Log(string.Format("{0} compro starter: {1}", name, item.itemName));
				}
			}
			else
			{
				boughtAll = false;
			}
		}
		
		if (boughtAll)
		{
			boughtStarterItems = true;
		}
	}
	
	void TryBuyNextItem()
	{
		if (buildConfig.itemPurchaseOrder == null) return;
		if (currentItemIndex >= buildConfig.itemPurchaseOrder.Count) return;
		
		int itemID = buildConfig.itemPurchaseOrder[currentItemIndex];
		Item item = ItemSystem.Instance.GetItemById(itemID);
		
		if (item == null)
		{
			currentItemIndex++;
			return;
		}
		
		if (_inventory.HasItem(itemID))
		{
			currentItemIndex++;
			return;
		}
		
		if (_inventory.CanAfford(item.cost))
		{
			bool success = _inventory.BuyItem(itemID);
			if (success)
			{
				if (showDebugLogs)
				{
					Debug.Log(string.Format("{0} compro: {1} (Nivel {2})", 
					                        name, item.itemName, _character.Nivel));
				}
				currentItemIndex++;
			}
		}
	}
	
	// === MÉTODOS DE AYUDA ===
	bool IsNearBase()
	{
		if (_character == null || _character.Base == null) return false;
		return Vector3.Distance(transform.position, _character.Base.position) < 10f;
	}
	
	int CountEnemyChampions()
	{
		int count = 0;
		if (Targets == null) return 0;
		
		foreach (Collider col in Targets)
		{
			if (col == null) continue;
			Character ch = col.GetComponent<Character>();
			if (ch != null && (ch.IsPlayer || ch.Isbot))
			{
				count++;
			}
		}
		return count;
	}
	
	int CountEnemyMinions()
	{
		int count = 0;
		if (Targets == null) return 0;
		
		foreach (Collider col in Targets)
		{
			if (col == null) continue;
			Character ch = col.GetComponent<Character>();
			if (ch != null && ch.IsCreep)
			{
				count++;
			}
		}
		return count;
	}
	
	int CountAllyChampions()
	{
		int count = 1;
		if (Aliados == null) return count;
		
		foreach (Collider col in Aliados)
		{
			if (col == null) continue;
			Character ch = col.GetComponent<Character>();
			if (ch != null && (ch.IsPlayer || ch.Isbot))
			{
				count++;
			}
		}
		return count;
	}
	
	int CountAllyMinions()
	{
		int count = 0;
		if (Aliados == null) return count;
		
		foreach (Collider col in Aliados)
		{
			if (col == null) continue;
			Character ch = col.GetComponent<Character>();
			if (ch != null && ch.IsCreep)
			{
				count++;
			}
		}
		return count;
	}
	
	Collider GetNearestCollider(Collider[] colliders)
	{
		if (colliders == null || colliders.Length == 0) return null;
		
		Collider nearest = null;
		float minDist = Mathf.Infinity;
		Vector3 myPos = transform.position;
		
		foreach (Collider col in colliders)
		{
			if (col == null) continue;
			float dist = Vector3.Distance(myPos, col.transform.position);
			if (dist < minDist)
			{
				minDist = dist;
				nearest = col;
			}
		}
		
		return nearest;
	}
	
	// === MÉTODOS ORIGINALES (compatibilidad) ===
	void RandLaneF()
	{
		int randLane = Random.Range(0, 3);
		timeCL = Random.Range(120f, 300f);
		Mid = (randLane == 0);
		Bot = (randLane == 1);
		Top = (randLane == 2);
		
		StartCoroutine(ChangeLane());
	}
	
	IEnumerator ChangeLane()
	{
		yield return new WaitForSeconds(0.5f);
		SetPath();
	}
	
	void SetPath()
	{
		string lanePrefix = null;
		if (Mid) lanePrefix = "Mid";
		else if (Bot) lanePrefix = "Bot";
		else if (Top) lanePrefix = "Top";
		
		if (lanePrefix == null) return;
		
		Transform[] builtPath = BuildPath(lanePrefix, !EquipoRojo);
		
		bool valid = true;
		for (int i = 0; i < builtPath.Length; i++)
		{
			if (builtPath[i] == null)
			{
				valid = false;
				break;
			}
		}
		
		if (valid)
		{
			camino = builtPath;
			punto = Mathf.Clamp(punto, 0, camino.Length - 1);
			ultimopunto = camino[punto];
		}
	}
	
	Transform[] BuildPath(string lanePrefix, bool forward)
	{
		Transform[] path = new Transform[7];
		
		if (forward)
		{
			path[0] = SafeFindTransform("Spawner Minion" + lanePrefix);
			for (int i = 1; i <= 6; i++)
			{
				char letter = (char)('F' - (i - 1));
				path[i] = SafeFindTransform("Waypoint Minion" + lanePrefix + i + "_" + letter);
			}
		}
		else
		{
			for (int i = 6; i >= 1; i--)
			{
				char letter = (char)('F' - (i - 1));
				path[6 - i] = SafeFindTransform("Waypoint Minion" + lanePrefix + i + "_" + letter);
			}
			path[6] = SafeFindTransform("Spawner Minion" + lanePrefix);
		}
		
		return path;
	}
	
	Transform SafeFindTransform(string objName)
	{
		GameObject go = GameObject.Find(objName);
		return (go != null) ? go.transform : null;
	}
	
	void UpdateTeamLayers()
	{
		if (EquipoRojo)
		{
			gameObject.tag = "TeamRed";
			Enemigo = (1 << LayerMask.NameToLayer("TeamBlue"))
				| (1 << LayerMask.NameToLayer("Jungle"))
					| (1 << LayerMask.NameToLayer("ChampionBlue"))
					| (1 << LayerMask.NameToLayer("BuildingBlue"));
			
			Aliado = (1 << LayerMask.NameToLayer("TeamRed"))
				| (1 << LayerMask.NameToLayer("ChampionRed"))
					| (1 << LayerMask.NameToLayer("BuildingRed"));
			
			turretLayerMask = (1 << LayerMask.NameToLayer("BuildingBlue"));
		}
		else
		{
			gameObject.tag = "TeamBlue";
			Enemigo = (1 << LayerMask.NameToLayer("TeamRed"))
				| (1 << LayerMask.NameToLayer("Jungle"))
					| (1 << LayerMask.NameToLayer("ChampionRed"))
					| (1 << LayerMask.NameToLayer("BuildingRed"));
			
			Aliado = (1 << LayerMask.NameToLayer("TeamBlue"))
				| (1 << LayerMask.NameToLayer("ChampionBlue"))
					| (1 << LayerMask.NameToLayer("BuildingBlue"));
			
			turretLayerMask = (1 << LayerMask.NameToLayer("BuildingRed"));
		}
		
		Supplie = (1 << LayerMask.NameToLayer("HealthPack"));
		CaptureP = (1 << LayerMask.NameToLayer("Capture Point"));
	}
	
	void FindTargets()
	{
		Targets = Physics.OverlapSphere(transform.position, detectionRadius, Enemigo);
	}
	
	void FindAllies()
	{
		Aliados = Physics.OverlapSphere(transform.position, detectionRadius, Aliado);
	}
	
	void FindSupplies()
	{
		Supplies = Physics.OverlapSphere(transform.position, detectionRadius, Supplie);
	}
	
	void FindCapturePoint()
	{
		CapturePoint = Physics.OverlapSphere(transform.position, detectionRadius, CaptureP);
		
		if (CapturePoint != null && CapturePoint.Length > 0)
		{
			Collider nearest = GetNearestCollider(CapturePoint);
			CapPo = (nearest != null) ? nearest.gameObject : null;
		}
		else
		{
			CapPo = null;
		}
	}
	
	void SafeCheck()
	{
		if (Targets != null && Aliados != null && Targets.Length > Aliados.Length + 1)
		{
			ItsSafe = false;
			Feartime = 1.7f;
		}
		
		if (AttackedByturret && VidaPerc < RanPerLife)
		{
			ItsSafe = false;
			Feartime = 1f;
		}
		
		if (VidaPerc < RanPerLife)
		{
			ItsSafe = false;
			Feartime = 1.5f;
		}
	}
	
	void CalcularVida()
	{
		if (_character == null) _character = GetComponent<Character>();
		
		if (_character != null && _character.Vida > 0)
		{
			VidaPerc = (_character.Vida * 100f) / _character.VidaMax;
		}
		else
		{
			VidaPerc = 0f;
		}
		
		if (TimeLoop > 0f) TimeLoop -= Time.deltaTime;
		if (TimeLoop < 0f)
		{
			TimeLoop = 1.5f;
			RanPerLife = (buildConfig != null) ? buildConfig.healthRetreatThreshold : Random.Range(10f, 50f);
		}
	}
	
	void RandomizePos()
	{
		RandomPos = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
	}
	
	IEnumerator SpecialMapInitialize()
	{
		yield return new WaitForSeconds(120f);
		if (Application.loadedLevelName == "Map4")
		{
			SpecialMap = true;
			if (camino != null && camino.Length > 0)
			{
				punto = Random.Range(0, camino.Length);
			}
		}
	}
	
	void randomattack()
	{
		ATT = 1 - ATT;
		animator.SetFloat("attack_state", (ATT == 1) ? 1f : 0f);
	}
	
	public void RandomAttack()
	{
		randomattack();
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRadius);
		
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, Rango);
		
		if (Objetivo != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, Objetivo.transform.position);
		}
		
		if (camino != null)
		{
			Gizmos.color = Color.green;
			for (int i = 0; i < camino.Length; i++)
			{
				if (camino[i] != null)
				{
					Gizmos.DrawWireSphere(camino[i].position, 0.5f);
					if (i < camino.Length - 1 && camino[i + 1] != null)
					{
						Gizmos.DrawLine(camino[i].position, camino[i + 1].position);
					}
				}
			}
		}
	}
}