using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GenerateBotsChampSelection : MonoBehaviour 
{
	[Header("Bot Data")]
	public Sprite[] LoadScreens;
	public Sprite[] Icons;
	public GameObject[] Bots;
	
	[Header("References")]
	public ChampContainer BotsCont;
	public ChampionSelection BotsIcons;
	
	[Header("Game Mode")]
	public bool OneForAll;
	public int OneForAllIndex;
	public bool Init = false;
	
	// Current bot selection
	private GameObject botBlue;
	private GameObject botRed;
	private Sprite LoadBlue;
	private Sprite LoadRed;
	private Sprite IconBlue;
	private Sprite IconRed;
	
	// Timers
	private float timeBlue;
	private float timeRed;
	
	// Bot counts
	public int BotAmmountBlue;
	public int BotAmmountRed;
	
	// Tracking de bots ya seleccionados para evitar repeticiones
	private List<int> usedBotsBlue;
	private List<int> usedBotsRed;
	
	// Contadores internos para índices de array
	private int currentIndexBlue;
	private int currentIndexRed;
	
	// One For All opponent index
	private int oneForAllOtherIndex;
	
	// Constants
	private const float SELECTION_DELAY = 0.2f;
	
	private void Start()
	{
		oneForAllOtherIndex = Random.Range(0, (Bots != null && Bots.Length > 0) ? Bots.Length : 1);
		usedBotsBlue = new List<int>();
		usedBotsRed = new List<int>();
	}
	
	public void Starting() 
	{
		usedBotsBlue.Clear();
		usedBotsRed.Clear();
		currentIndexBlue = 0;
		currentIndexRed = 0;
		
		// Si el ChampContainer tiene arrays definidos, aseguro las cantidades
		if (BotsCont != null)
		{
			if (BotsCont.BotsBlue != null) BotAmmountBlue = BotsCont.BotsBlue.Length;
			if (BotsCont.BotsRed != null) BotAmmountRed = BotsCont.BotsRed.Length;
		}
		
		DetermineBotInicialBlue();
		DetermineBotInicialRed();
	}
	
	private void Update() 
	{
		if (!Init) return;
		
		UpdateTeamSpawning(ref timeBlue, ref BotAmmountBlue, true);
		UpdateTeamSpawning(ref timeRed, ref BotAmmountRed, false);
	}
	
	private void UpdateTeamSpawning(ref float timer, ref int botAmount, bool isBlueTeam)
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			return;
		}
		
		// Si el timer llegó a 0, spawneamos el bot preparado
		if (timer <= 0f)
		{
			if (isBlueTeam)
				SpawnBotBlue();
			else
				SpawnBotRed();
		}
		
		// Después de spawnear, siempre preparar el siguiente (sin condicional botAmount > 0)
		if (timer == 0f)
		{
			if (isBlueTeam)
				DetermineBotBlue();
			else
				DetermineBotRed();
		}
	}
	
	private void DetermineBotBlue()
	{
		int index = GetBotIndex(true);
		SetBotData(ref botBlue, ref LoadBlue, ref IconBlue, ref timeBlue, index);
	}
	
	private void DetermineBotRed()
	{
		int index = GetBotIndex(false);
		SetBotData(ref botRed, ref LoadRed, ref IconRed, ref timeRed, index);
	}
	
	private void DetermineBotInicialBlue()
	{
		if (Bots == null || Bots.Length == 0) return;
		
		int index = GetBotIndex(true);
		SetBotData(ref botBlue, ref LoadBlue, ref IconBlue, ref timeBlue, index);
	}
	
	private void DetermineBotInicialRed()
	{
		if (Bots == null || Bots.Length == 0) return;
		
		int index = GetBotIndex(false);
		SetBotData(ref botRed, ref LoadRed, ref IconRed, ref timeRed, index);
	}
	
	private int GetBotIndex(bool isBlueTeam)
	{
		if (Bots == null || Bots.Length == 0) return 0;
		
		if (OneForAll)
		{
			if (BotsIcons == null) return Random.Range(0, Bots.Length);
			
			bool isPlayerTeam = (isBlueTeam && BotsIcons.team == 0) || 
				(!isBlueTeam && BotsIcons.team == 1);
			
			return isPlayerTeam ? OneForAllIndex : oneForAllOtherIndex;
		}
		
		List<int> usedBots = isBlueTeam ? usedBotsBlue : usedBotsRed;
		
		List<int> availableIndices = new List<int>();
		for (int i = 0; i < Bots.Length; i++)
		{
			if (!usedBots.Contains(i))
				availableIndices.Add(i);
		}
		
		if (availableIndices.Count == 0)
		{
			usedBots.Clear();
			for (int i = 0; i < Bots.Length; i++)
				availableIndices.Add(i);
		}
		
		int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
		usedBots.Add(randomIndex);
		
		return randomIndex;
	}
	
	private void SetBotData(ref GameObject bot, ref Sprite loadScreen, ref Sprite icon, 
	                        ref float timer, int index)
	{
		if (Bots == null || index < 0 || index >= Bots.Length) return;
		
		bot = Bots[index];
		
		if (LoadScreens != null && index < LoadScreens.Length)
			loadScreen = LoadScreens[index];
		
		if (Icons != null && index < Icons.Length)
			icon = Icons[index];
		
		timer = SELECTION_DELAY;
	}
	
	private void SpawnBotBlue()
	{
		timeBlue = 0f;
		
		if (BotsCont == null) return;  // NO validar BotAmmount <= 0 aquí
		
		int arrayIndex = currentIndexBlue;
		
		if (BotsCont.BotsBlue == null || arrayIndex >= BotsCont.BotsBlue.Length) 
		{
			// inválido: decrementar si hay inconsistencia
			if (BotAmmountBlue > 0) BotAmmountBlue--;
			return;
		}
		
		BotsCont.BotsBlue[arrayIndex] = botBlue;
		
		if (BotsCont.LoadScreenBotBlue != null && arrayIndex < BotsCont.LoadScreenBotBlue.Length)
			BotsCont.LoadScreenBotBlue[arrayIndex] = LoadBlue;
		
		if (BotsCont.IconsBotBlue != null && arrayIndex < BotsCont.IconsBotBlue.Length)
			BotsCont.IconsBotBlue[arrayIndex] = IconBlue;
		
		if (BotsIcons != null && BotsIcons.team == 0)
		{
			if (BotsIcons.BotChamp != null && arrayIndex < BotsIcons.BotChamp.Length)
			{
				if (BotsIcons.BotChamp[arrayIndex] != null)
					BotsIcons.BotChamp[arrayIndex].sprite = IconBlue;
			}
		}
		
		currentIndexBlue++;
		BotAmmountBlue--;
	}
	
	private void SpawnBotRed()
	{
		timeRed = 0f;
		
		if (BotsCont == null) return;  // NO validar BotAmmount <= 0 aquí
		
		int arrayIndex = currentIndexRed;
		
		if (BotsCont.BotsRed == null || arrayIndex >= BotsCont.BotsRed.Length)
		{
			if (BotAmmountRed > 0) BotAmmountRed--;
			return;
		}
		
		BotsCont.BotsRed[arrayIndex] = botRed;
		
		if (BotsCont.LoadScreenBotRed != null && arrayIndex < BotsCont.LoadScreenBotRed.Length)
			BotsCont.LoadScreenBotRed[arrayIndex] = LoadRed;
		
		if (BotsCont.IconsBotRed != null && arrayIndex < BotsCont.IconsBotRed.Length)
			BotsCont.IconsBotRed[arrayIndex] = IconRed;
		
		if (BotsIcons != null && BotsIcons.team == 1)
		{
			if (BotsIcons.BotChamp != null && arrayIndex < BotsIcons.BotChamp.Length)
			{
				if (BotsIcons.BotChamp[arrayIndex] != null)
					BotsIcons.BotChamp[arrayIndex].sprite = IconRed;
			}
		}
		
		currentIndexRed++;
		BotAmmountRed--;
	}
	
	// Public utility methods
	public void ResetBotGeneration()
	{
		Init = false;
		timeBlue = 0f;
		timeRed = 0f;
		BotAmmountBlue = 0;
		BotAmmountRed = 0;
		currentIndexBlue = 0;
		currentIndexRed = 0;
		
		if (usedBotsBlue != null) usedBotsBlue.Clear();
		if (usedBotsRed != null) usedBotsRed.Clear();
		
		oneForAllOtherIndex = Random.Range(0, (Bots != null && Bots.Length > 0) ? Bots.Length : 1);
	}
	
	public void SetOneForAllOpponent(int index)
	{
		if (index >= 0 && index < Bots.Length)
		{
			oneForAllOtherIndex = index;
		}
	}
	

}