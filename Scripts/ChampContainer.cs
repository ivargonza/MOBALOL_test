using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampContainer : MonoBehaviour 
{
	[Header("Player Setup")]
	public MobaCam Tar;
	public GameObject Champ;
	public GameObject DSpell;
	public GameObject FSpell;
	public int team; // 0 = Blue, 1 = Red
	
	[Header("Bot Setup")]
	public GameObject[] BotsBlue;
	public GameObject[] BotsRed;
	
	[Header("UI")]
	public Sprite LoadScreenSelected;
	public Sprite[] LoadScreenBotBlue;
	public Sprite[] LoadScreenBotRed;
	public Sprite[] IconsBotBlue;
	public Sprite[] IconsBotRed;
	
	[Header("Game Settings")]
	public string Nombre;
	public string NombreMapa;
	public bool isLoged;
	public bool URF;
	
	[Header("Bot Spawn Settings")]
	public float botSpawnInterval = 2f;
	public float botSelectionDelay = 0.5f;
	
	// Runtime variables
	private float timeBlue;
	private float timeRed;
	private int botAmmountBlue;
	private int botAmmountRed;
	private int lineaBlue = 0;
	private int lineaRed = 0;
	
	private GameObject botBlue;
	private GameObject botRed;
	private TitanBar tB;
	private GameObject playerChampion;
	
	// Cached references
	private Transform wellBlue;
	private Transform wellRed;
	private bool isInitialized = false;
	
	// Scene indices donde el juego está activo
	private readonly int[] activeScenes = { 2, 3, 4 };
	
	void Awake() 
	{
		DontDestroyOnLoad(gameObject);
	}
	
	private void OnEnable()
	{
		// Resetear toda la lógica interna cuando el objeto se activa
		//ResetAllVariables();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		
		// También resetear variables al apagarse
		//ResetAllVariables();
	}
	

	
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Resetear estado cuando carga nueva escena
		isInitialized = false;
	}
	
	public void SpawnChamp()
	{
		CacheReferences();
		
		if (wellBlue == null || wellRed == null)
		{
			Debug.LogError("ChampContainer: Wells not found! Make sure 'Well Blue' and 'Well Red' exist in scene.");
			return;
		}
		
		Transform spawnPoint = team == 0 ? wellBlue : wellRed;
		
		// Spawn player champion
		playerChampion = Instantiate(Champ, spawnPoint.position, spawnPoint.rotation);
		
		// Setup camera
		if (Tar == null)
		{
			GameObject camObj = GameObject.Find("FollowCam");
			if (camObj != null)
				Tar = camObj.GetComponent<MobaCam>();
		}
		
		if (Tar != null)
			Tar.target = playerChampion.transform;
		
		// Setup team
		ClickToMove ctm = playerChampion.GetComponent<ClickToMove>();
		if (ctm != null)
			ctm.EquipoRojo = (team == 1);
		
		// Initialize bot spawning
		InitializeBotSpawning();
	}
	
	private void InitializeBotSpawning()
	{
		if (BotsBlue != null && BotsBlue.Length > 0)
		{
			botBlue = BotsBlue[0];
			timeBlue = botSpawnInterval;
		}
		
		if (BotsRed != null && BotsRed.Length > 0)
		{
			botRed = BotsRed[0];
			timeRed = botSpawnInterval;
		}
	}
	
	private void CacheReferences()
	{
		if (isInitialized) return;
		
		// Cache well transforms
		GameObject wellBlueObj = GameObject.Find("Well Blue");
		GameObject wellRedObj = GameObject.Find("Well Red");
		
		if (wellBlueObj != null)
			wellBlue = wellBlueObj.transform;
		
		if (wellRedObj != null)
			wellRed = wellRedObj.transform;
		
		// Cache TitanBar
		GameObject titanBarObj = GameObject.Find("Titan bar");
		if (titanBarObj != null)
			tB = titanBarObj.GetComponent<TitanBar>();
		
		isInitialized = true;
	}
	
	void Update() 
	{
		// Check if we're in an active scene
		int currentScene = SceneManager.GetActiveScene().buildIndex;
		bool isActiveScene = System.Array.Exists(activeScenes, scene => scene == currentScene);
		
		if (!isActiveScene) return;
		
		// Initialize if needed
		if (!isInitialized)
			CacheReferences();
		
		// Update bot spawn timers
		UpdateBotSpawning(ref timeBlue, ref botAmmountBlue, BotsBlue != null ? BotsBlue.Length : 0, true);
		UpdateBotSpawning(ref timeRed, ref botAmmountRed, BotsRed != null ? BotsRed.Length : 0, false);
	}
	
	private void UpdateBotSpawning(ref float timer, ref int botAmount, int maxBots, bool isBlueTeam)
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			return;
		}
		
		// Time to spawn
		if (timer <= 0f && botAmount < maxBots)
		{
			if (isBlueTeam)
				SpawnBotBlue();
			else
				SpawnBotRed();
			
			timer = 0f; // Reset to 0, will be set by DetermineBot
		}
		
		// Select next bot
		if (botAmount < maxBots && timer == 0f)
		{
			if (isBlueTeam)
				DetermineBotBlue();
			else
				DetermineBotRed();
		}
	}
	
	private void DetermineBotBlue()
	{
		if (botAmmountBlue < BotsBlue.Length)
		{
			botBlue = BotsBlue[botAmmountBlue];
			timeBlue = botSelectionDelay;
		}
	}
	
	private void DetermineBotRed()
	{
		if (botAmmountRed < BotsRed.Length)
		{
			botRed = BotsRed[botAmmountRed];
			timeRed = botSelectionDelay;
		}
	}
	
	private void SpawnBotBlue()
	{
		if (botBlue == null || wellBlue == null) return;
		
		lineaBlue++;
		
		// Spawn en índice exacto
		int index = botAmmountBlue;
		
		botAmmountBlue++;  // recién ahora sumo 1
		
		IABots botAI = botBlue.GetComponent<IABots>();
		if (botAI != null)
		{
			botAI.EquipoRojo = false;
			SetBotLane(botAI, lineaBlue);
		}
		
		GameObject spawnedBot = Instantiate(botBlue, wellBlue.position, wellBlue.rotation);
		
		if (team == 0 && tB != null && index < tB.Bots.Length)
		{
			tB.Bots[index] = spawnedBot;
			
			if (index < IconsBotBlue.Length && index < tB.BotsIcons.Length)
				tB.BotsIcons[index].sprite = IconsBotBlue[index];
		}
		
		timeBlue = 0f;
	}
	
	private void SpawnBotRed()
	{
		if (botRed == null || wellRed == null) return;
		
		lineaRed++;
		
		int index = botAmmountRed;
		
		botAmmountRed++;
		
		IABots botAI = botRed.GetComponent<IABots>();
		if (botAI != null)
		{
			botAI.EquipoRojo = true;
			SetBotLane(botAI, lineaRed);
		}
		
		GameObject spawnedBot = Instantiate(botRed, wellRed.position, wellRed.rotation);
		
		if (team == 1 && tB != null && index < tB.Bots.Length)
		{
			tB.Bots[index] = spawnedBot;
			
			if (index < IconsBotRed.Length && index < tB.BotsIcons.Length)
				tB.BotsIcons[index].sprite = IconsBotRed[index];
		}
		
		timeRed = 0f;
	}
	
	// Helper method to reduce code duplication
	private void SetBotLane(IABots botAI, int laneNumber)
	{
		// Reset all lanes
		botAI.Mid = false;
		botAI.Top = false;
		botAI.Bot = false;
		
		// Set specific lane based on rotation (1-5 bots distribution)
		switch (laneNumber % 5)
		{
		case 1: // Bot lane
		case 3:
			botAI.Bot = true;
			break;
		case 2: // Top lane
		case 4:
			botAI.Top = true;
			break;
		case 0: // Mid lane (when laneNumber = 5, 10, etc.)
			botAI.Mid = true;
			break;
		}
	}
	
	// Public utility methods

	
	public GameObject GetPlayerChampion()
	{
		return playerChampion;
	}
	
	public bool IsPlayerTeamBlue()
	{
		return team == 0;
	}
}