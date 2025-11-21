using UnityEngine;
using System.Collections;

public class MinionsSpawner : MonoBehaviour 
{
	[Header("Team Configuration")]
	[SerializeField] private TeamType team;
	
	[SerializeField] private Character inhibitor;
	[SerializeField] private Transform spawner;
	
	private string minionMeleeTag;
	private string minionRangeTag;
	private string minionSiegeTag;
	private string superMinionTag;
	
	[Header("Lane Configuration")]
	[SerializeField] private LaneType laneType;
	
	[Header("Initial Stats Reference")]
	[SerializeField] private MinionStats meleeStats;
	[SerializeField] private MinionStats rangeStats;
	[SerializeField] private MinionStats siegeStats;
	[SerializeField] private MinionStats superStats;
	
	private float timer;
	private float upgradeTimer;
	private bool spawnSiege;
	private bool spawnSuper;
	
	private const float INITIAL_TIMER = 90f;
	private const float SPAWN_INTERVAL = 30f;
	private const float UPGRADE_INTERVAL = 90f;
	private const float SUPER_INTERVAL = 60f;
	private const float MINION_SPAWN_DELAY = 1f;
	
	private const float MELEE_HEALTH_UPGRADE = 10f;
	private const float RANGE_HEALTH_UPGRADE = 10f;
	private const float SIEGE_HEALTH_UPGRADE = 0f;
	private const float SUPER_HEALTH_UPGRADE = 0f;
	private const float MELEE_DAMAGE_UPGRADE = 1f;
	private const float RANGE_DAMAGE_UPGRADE = 2f;
	private const float SIEGE_DAMAGE_UPGRADE = 0f;
	private const float SUPER_DAMAGE_UPGRADE = 0f;
	
	private enum LaneType { Mid, Bot, Top }
	private enum TeamType { Blue, Red }
	
	[System.Serializable]
	public class MinionStats
	{
		public float vida;
		public float vidaMax;
		public float daño;
		public int nivel;
	}
	
	private void Start()
	{
		timer = INITIAL_TIMER;
		InitializePoolTags();
	}
	
	private void InitializePoolTags()
	{
		string teamSuffix = team == TeamType.Blue ? "_Blue" : "_Red";
		minionMeleeTag = "MinionMelee" + teamSuffix;
		minionRangeTag = "MinionRange" + teamSuffix;
		minionSiegeTag = "MinionSiege" + teamSuffix;
		superMinionTag = "SuperMinion" + teamSuffix;
	}
	
	private void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				timer = 0f;
				StartCoroutine(SpawnWave());
			}
		}
	}
	
	private bool IsInhibitorDestroyed()
	{
		return inhibitor != null && inhibitor.Vida <= 0f;
	}
	
	private void SpawnMinion(string poolTag, MinionStats stats)
	{
		GameObject minion = ObjectPool.Instance.SpawnFromPool(poolTag, spawner.position, spawner.rotation);
		minion.transform.SetParent(spawner);
		minion.transform.localPosition = Vector3.zero;
		minion.transform.localRotation = Quaternion.identity;
		minion.transform.SetParent(null); 

		if (minion != null)
		{
			MinionsAI ai = minion.GetComponent<MinionsAI>();
			if (ai != null)
			{
				ai.Mid = laneType == LaneType.Mid;
				ai.Bot = laneType == LaneType.Bot;
				ai.Top = laneType == LaneType.Top;
				ai.InitializePath();
			}
			
			Character character = minion.GetComponent<Character>();
			if (character != null)
			{
				character.Vida = stats.vida;
				character.VidaMax = stats.vidaMax;
				character.Daño = stats.daño;
				character.Nivel = stats.nivel;
				character.IsAlive = true;
			}
		}
	}
	
	private void UpgradeMinions()
	{
		meleeStats.vida += MELEE_HEALTH_UPGRADE;
		meleeStats.vidaMax += MELEE_HEALTH_UPGRADE;
		meleeStats.daño += MELEE_DAMAGE_UPGRADE;
		meleeStats.nivel++;
		
		rangeStats.vida += RANGE_HEALTH_UPGRADE;
		rangeStats.vidaMax += RANGE_HEALTH_UPGRADE;
		rangeStats.daño += RANGE_DAMAGE_UPGRADE;
		rangeStats.nivel++;
		
		siegeStats.vida += SIEGE_HEALTH_UPGRADE;
		siegeStats.vidaMax += SIEGE_HEALTH_UPGRADE;
		siegeStats.daño += SIEGE_DAMAGE_UPGRADE;
		siegeStats.nivel++;
		
		if (IsInhibitorDestroyed())
		{
			superStats.vida += SUPER_HEALTH_UPGRADE;
			superStats.vidaMax += SUPER_HEALTH_UPGRADE;
			superStats.daño += SUPER_DAMAGE_UPGRADE;
			superStats.nivel++;
		}
	}
	
	private IEnumerator SpawnWave()
	{
		upgradeTimer += SPAWN_INTERVAL;
		
		if (upgradeTimer >= UPGRADE_INTERVAL)
		{
			UpgradeMinions();
			upgradeTimer = 0f;
			spawnSiege = true;
		}
		
		if (upgradeTimer >= SUPER_INTERVAL)
		{
			spawnSuper = true;
		}
		
		bool inhibitorDown = IsInhibitorDestroyed();
		WaitForSeconds delay = new WaitForSeconds(MINION_SPAWN_DELAY);
		
		// Primera oleada: Super o Melee
		if (inhibitorDown)
		{
			SpawnMinion(superMinionTag, superStats);
		}
		else
		{
			SpawnMinion(minionMeleeTag, meleeStats);
		}
		yield return delay;
		
		// Segunda oleada: Super o Melee (si corresponde)
		if (spawnSuper && inhibitorDown)
		{
			SpawnMinion(superMinionTag, superStats);
		}
		else
		{
			SpawnMinion(minionMeleeTag, meleeStats);
		}
		yield return delay;
		
		// Tercer melee
		SpawnMinion(minionMeleeTag, meleeStats);
		yield return delay;
		
		// Siege o Range
		if (spawnSiege)
		{
			SpawnMinion(minionSiegeTag, siegeStats);
		}
		else
		{
			SpawnMinion(minionRangeTag, rangeStats);
		}
		yield return delay;
		
		// Dos ranges
		SpawnMinion(minionRangeTag, rangeStats);
		yield return delay;
		SpawnMinion(minionRangeTag, rangeStats);
		
		timer = SPAWN_INTERVAL;
		spawnSuper = false;
		spawnSiege = false;
	}
}
