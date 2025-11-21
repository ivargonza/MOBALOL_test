using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChampionSelection : MonoBehaviour 
{
	[Header("Champions")]
	public GameObject[] Champion;
	public Button[] Champ;
	
	[Header("UI Elements")]
	public Image[] BotChamp;
	public Sprite Simbol;
	public Text CountDownText;
	public Text NameText;
	public Image ChampSel;
	public Button Play;
	public Text Nombreinvocador;
	
	[Header("Audio")]
	public AudioClip[] Select;
	public AudioSource Selecting;
	public AudioSource Lock;
	public AudioSource Music;
	public AudioSource EndSelect;
	public AudioSource Count10Seconds;
	
	[Header("Panels")]
	public GameObject Map1;
	public GameObject Map2;
	public GameObject Map;
	public GameObject MainMenu;
	public GameObject ChampList;
	public GameObject SkinSelect;
	
	[Header("References")]
	public MenuPrincipal profile;
	public ChampContainer Things;
	public GenerateBotsChampSelection botAmm;
	public SkinsSelect ChampIndex;
	
	[Header("Game Modes")]
	public Toggle OneForAll;
	public Toggle URF;
	
	// Runtime variables
	public int index { get; set; }
	public Sprite LoadScreenSelected;
	public string NombreDeChampion;
	public bool OneForAllcheck;
	public int OneForAllIndex;
	public bool isLocked;
	public int team;
	
	private GameObject ChampSelected;
	private GenerateBotsChampSelection GBCS;
	private float CountDown;
	
	// Constants
	private const float INITIAL_COUNTDOWN = 90f;
	private const float LOCK_COUNTDOWN = 15f;
	private const float WARNING_TIME = 10f;
	private const float END_TIME = 3f;
	private const int LOADING_SCENE_INDEX = 1;
	
	private void Start() 
	{
		CacheReferences();
	}
	
	private void CacheReferences()
	{
		if (Things == null)
		{
			GameObject container = GameObject.Find("ChampContainer");
			if (container != null)
				Things = container.GetComponent<ChampContainer>();
		}
		
		GBCS = GetComponent<GenerateBotsChampSelection>();
		if (botAmm == null)
			botAmm = GBCS;
	}
	
	private void OnEnable()
	{
		InitializeSelection();
	}
	
	private void InitializeSelection()
	{
		team = Random.Range(0, 2);
		CountDown = INITIAL_COUNTDOWN;
		isLocked = false;
		
		// Audio setup
		if (Count10Seconds != null) Count10Seconds.enabled = false;
		if (EndSelect != null) EndSelect.enabled = false;
		if (Music != null) Music.enabled = true;
		
		// UI setup
		if (ChampList != null) ChampList.SetActive(true);
		if (SkinSelect != null) SkinSelect.SetActive(false);
		
		// Start setup coroutine that waits end of frame so resets from OnEnable() finish first
		StartCoroutine(SetupBotInfo());
	}
	
	private void Update() 
	{
		UpdateGameModes();
		UpdateCountdown();
		UpdateMapDisplay();
		UpdatePlayButton();
	}
	
	private void UpdateGameModes()
	{
		if (OneForAll != null && GBCS != null)
		{
			OneForAllcheck = OneForAll.isOn;
			GBCS.OneForAll = OneForAllcheck;
		}
		
		if (URF != null && Things != null)
		{
			Things.URF = URF.isOn;
		}
	}
	
	private void UpdateCountdown()
	{
		if (CountDown > 0f)
		{
			CountDown -= Time.deltaTime;
			
			// Update text
			if (CountDownText != null)
				CountDownText.text = CountDown.ToString("0") + " s";
			
			// Handle countdown events
			HandleCountdownEvents();
		}
		else if (CountDown <= 0f)
		{
			// Time's up
			if (Things != null && Things.Champ == null)
			{
				Cancel();
			}
			else
			{
				Seleccion();
			}
		}
	}
	
	private void HandleCountdownEvents()
	{
		// End selection warning (3 seconds)
		if (CountDown <= END_TIME && CountDown > 0f)
		{
			if (Things != null && Things.Champ != null)
			{
				if (EndSelect != null && !EndSelect.enabled)
				{
					EndSelect.enabled = true;
					if (Music != null) Music.enabled = false;
				}
			}
		}
		
		// 10 second warning
		if (CountDown <= WARNING_TIME && CountDown > END_TIME)
		{
			if (Things != null && Things.Champ == null)
			{
				if (Count10Seconds != null && !Count10Seconds.enabled)
				{
					Count10Seconds.enabled = true;
				}
			}
		}
	}
	
	private void UpdateMapDisplay()
	{
		if (Things == null) return;
		
		// Disable all maps first
		if (Map1 != null) Map1.SetActive(false);
		if (Map2 != null) Map2.SetActive(false);
		if (Map != null) Map.SetActive(false);
		
		// Enable the correct map
		switch (Things.NombreMapa)
		{
		case "Map1":
			if (Map1 != null) Map1.SetActive(true);
			break;
		case "Map2":
			if (Map2 != null) Map2.SetActive(true);
			break;
		case "Map3":
			if (Map != null) Map.SetActive(true);
			break;
		}
	}
	
	private void UpdatePlayButton()
	{
		if (Play == null) return;
		
		Play.interactable = (ChampSelected != null && !isLocked);
	}
	
	public void Cancel()
	{
		// Reset state
		if (Things != null)
		{
			Things.Champ = null;
			Things.team = 0;
			Things.LoadScreenSelected = null;
		}
		
		// Reset UI
		if (ChampSel != null) ChampSel.sprite = Simbol;
		if (NameText != null) NameText.text = "";
		if (ChampList != null) ChampList.SetActive(true);
		if (SkinSelect != null) SkinSelect.SetActive(false);
		
		isLocked = false;
		
		// Switch to main menu
		gameObject.SetActive(false);
		if (MainMenu != null) MainMenu.SetActive(true);
		if (profile != null && profile.Profile != null) 
			profile.Profile.SetActive(true);
	}
	
	public void Seleccion()
	{
		SceneManager.LoadScene(LOADING_SCENE_INDEX);
	}
	
	public void LockIn()
	{
		if (ChampSelected == null || Things == null) return;
		
		// Update state
		if (Play != null) Play.interactable = false;
		isLocked = true;
		CountDown = LOCK_COUNTDOWN;
		
		// Play audio
		if (Lock != null) Lock.Play();
		
		// Update UI
		if (ChampList != null) ChampList.SetActive(false);
		if (SkinSelect != null) SkinSelect.SetActive(true);
		
		// Configure champion
		UIchar uiChar = ChampSelected.GetComponent<UIchar>();
		if (uiChar != null && uiChar.Nombre != null)
		{
			uiChar.Nombre.text = Things.Nombre;
		}
		
		// Set team
		ClickToMove ctm = ChampSelected.GetComponent<ClickToMove>();
		if (ctm != null)
		{
			ctm.EquipoRojo = (team == 1);
		}
		
		// Update ChampContainer
		Things.Champ = ChampSelected;
		Things.team = team;
		Things.LoadScreenSelected = LoadScreenSelected;
		
		// Start bot generation
		if (GBCS != null)
		{
			GBCS.Init = true;
			// Starting() will be called from SetupBotInfo after arrays and counts are set.
		}
	}
	
	public void ButtonSelect()
	{
		if (Champion == null || Champ == null) return;
		if (index < 0 || index >= Champion.Length) return;
		
		// Enable all buttons except selected
		for (int i = 0; i < Champion.Length; i++) 
		{
			if (Champ[i] != null && i != index)
			{
				Champ[i].interactable = true;
			}
		}
		
		// Update UI
		if (NameText != null && Champ[index] != null)
		{
			NameText.text = Champ[index].gameObject.name;
		}
		
		if (ChampSel != null && Champ[index] != null)
		{
			ChampSel.sprite = Champ[index].image.sprite;
		}
		
		// Disable selected button
		if (Champ[index] != null)
		{
			Champ[index].interactable = false;
		}
		
		// Set champion
		ChampSelected = Champion[index];
		
		// Play sound
		if (Selecting != null && Select != null && index < Select.Length)
		{
			Selecting.clip = Select[index];
			Selecting.Play();
		}
		
		// Update load screen
		if (ChampIndex != null && ChampIndex.PantallaDeCarga0 != null && 
		    index < ChampIndex.PantallaDeCarga0.Length)
		{
			LoadScreenSelected = ChampIndex.PantallaDeCarga0[index];
			ChampIndex.ChampIndex = index;
			ChampIndex.ChangeLoadScreen();
		}
		
		// Set One For All index
		if (GBCS != null)
		{
			GBCS.OneForAllIndex = index;
		}
	}
	
	private System.Collections.IEnumerator SetupBotInfo()
	{
		// Esperar a que TODOS los OnEnable() / ResetAllVars terminen.
		// Uso WaitForEndOfFrame dos veces por robustez.
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		
		if (Things == null) yield break;
		
		// Initialize arrays based on team
		int playerTeamSize = 4;
		int enemyTeamSize = 5;
		
		if (team == 0) // Blue team
		{
			Things.BotsBlue = new GameObject[playerTeamSize];
			Things.BotsRed = new GameObject[enemyTeamSize];
			Things.LoadScreenBotBlue = new Sprite[playerTeamSize];
			Things.LoadScreenBotRed = new Sprite[enemyTeamSize];
			Things.IconsBotBlue = new Sprite[playerTeamSize];
			Things.IconsBotRed = new Sprite[enemyTeamSize];
		}
		else // Red team
		{
			Things.BotsBlue = new GameObject[enemyTeamSize];
			Things.BotsRed = new GameObject[playerTeamSize];
			Things.LoadScreenBotBlue = new Sprite[enemyTeamSize];
			Things.LoadScreenBotRed = new Sprite[playerTeamSize];
			Things.IconsBotBlue = new Sprite[enemyTeamSize];
			Things.IconsBotRed = new Sprite[playerTeamSize];
		}
		
		// Asegurarse un frame más antes de asignar cantidades (evita race conditions)
		yield return new WaitForEndOfFrame();
		
		// Setup bot amounts
		if (botAmm != null)
		{
			if (team == 0)
			{
				botAmm.BotAmmountBlue = playerTeamSize;
				botAmm.BotAmmountRed = enemyTeamSize;
			}
			else
			{
				botAmm.BotAmmountBlue = enemyTeamSize;
				botAmm.BotAmmountRed = playerTeamSize;
			}
			
			// Ahora que los arrays existen y las cantidades están seteadas,
			// iniciamos al generador para que DetermineBotInicial y demás funcionen.
			botAmm.Starting();
		}
	}
}