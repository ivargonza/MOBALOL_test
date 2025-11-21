using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class loading : MonoBehaviour 
{
	[Header("Progress Bars")]
	public Image progressbarBlue;
	public Image progressbarRed;
	
	[Header("Player Screens")]
	public Image LoadscreenBluePlayer;
	public Image LoadscreenRedPlayer;
	
	[Header("Bot Containers")]
	public GameObject botBlue;
	public GameObject botRed;
	
	[Header("Bot Screens")]
	public Image[] LoadscreenBlue;
	public Image[] LoadscreenRed;
	
	[Header("Text Elements")]
	public Text porcentajeBlue;
	public Text porcentajeRed;
	public Text NombreBlue;
	public Text NombreRed;
	public Text[] NombreBlueBot;
	public Text[] NombreRedBot;
	
	private ChampContainer things;
	private AsyncOperation asyncOperation;
	
	// Constants
	private const float PROGRESS_OFFSET = 10f;
	private const string PERCENTAGE_FORMAT = "F1";
	private const float INIT_DELAY = 0.1f;
	
	private bool isInitialized = false;
	
	private void Start() 
	{
		CacheReferences();
		
		if (things != null)
		{
			string mapName = things.NombreMapa;
			if (!string.IsNullOrEmpty(mapName))
			{
				StartCoroutine(LoadLevel(mapName));
				StartCoroutine(ConfigureScreens());
			}
			else
			{
				Debug.LogError("Loading: Map name is null or empty!");
			}
		}
		else
		{
			Debug.LogError("Loading: ChampContainer not found!");
		}
	}
	
	private void CacheReferences()
	{
		GameObject container = GameObject.Find("ChampContainer");
		if (container != null)
		{
			things = container.GetComponent<ChampContainer>();
		}
	}
	
	private void FixedUpdate()
	{
		if (asyncOperation == null || things == null) return;
		
		float progressPercentage = PROGRESS_OFFSET + (asyncOperation.progress * 100f);
		
		if (things.team == 0) // Blue team
		{
			ConfigureBlueTeam(progressPercentage);
		}
		else if (things.team == 1) // Red team
		{
			ConfigureRedTeam(progressPercentage);
		}
	}
	
	private void ConfigureBlueTeam(float progress)
	{
		// Player info
		if (NombreBlue != null)
			NombreBlue.text = things.Nombre;
		
		if (LoadscreenBluePlayer != null && things.LoadScreenSelected != null)
			LoadscreenBluePlayer.sprite = things.LoadScreenSelected;
		
		// Progress bar
		if (progressbarBlue != null)
			progressbarBlue.fillAmount = asyncOperation.progress;
		
		if (porcentajeBlue != null)
			porcentajeBlue.text = progress.ToString(PERCENTAGE_FORMAT) + " %";
		
		// Show/hide bot containers
		SetBotContainersActive(false, true);
	}
	
	private void ConfigureRedTeam(float progress)
	{
		// Player info
		if (NombreRed != null)
			NombreRed.text = things.Nombre;
		
		if (LoadscreenRedPlayer != null && things.LoadScreenSelected != null)
			LoadscreenRedPlayer.sprite = things.LoadScreenSelected;
		
		// Progress bar
		if (progressbarRed != null)
			progressbarRed.fillAmount = asyncOperation.progress;
		
		if (porcentajeRed != null)
			porcentajeRed.text = progress.ToString(PERCENTAGE_FORMAT) + " %";
		
		// Show/hide bot containers
		SetBotContainersActive(true, false);
	}
	
	private void SetBotContainersActive(bool blueActive, bool redActive)
	{
		if (botBlue != null) botBlue.SetActive(blueActive);
		if (botRed != null) botRed.SetActive(redActive);
	}
	
	private IEnumerator ConfigureScreens()
	{
		yield return new WaitForSeconds(INIT_DELAY);
		
		if (things == null) yield break;
		
		string mapName = things.NombreMapa;
		
		// Only configure bot screens for map scenes
		if (mapName == "Map1" || mapName == "Map2" || mapName == "Map3")
		{
			ConfigureBotScreens();
		}
		
		isInitialized = true;
	}
	
	private void ConfigureBotScreens()
	{
		ConfigureTeamBots(
			things.LoadScreenBotBlue,
			things.BotsBlue,
			LoadscreenBlue,
			NombreBlueBot
			);
		
		ConfigureTeamBots(
			things.LoadScreenBotRed,
			things.BotsRed,
			LoadscreenRed,
			NombreRedBot
			);
	}
	
	private void ConfigureTeamBots(Sprite[] loadScreens, GameObject[] bots, 
	                               Image[] screenImages, Text[] nameTexts)
	{
		if (loadScreens == null || screenImages == null) return;
		
		int count = Mathf.Min(loadScreens.Length, screenImages.Length);
		
		for (int i = 0; i < count; i++)
		{
			// Set load screen sprite
			if (screenImages[i] != null && loadScreens[i] != null)
			{
				screenImages[i].sprite = loadScreens[i];
			}
			
			// Set bot name
			if (nameTexts != null && i < nameTexts.Length && 
			    bots != null && i < bots.Length)
			{
				if (nameTexts[i] != null && bots[i] != null)
				{
					nameTexts[i].text = bots[i].name;
				}
			}
		}
	}
	
	private IEnumerator LoadLevel(string levelName) 
	{
		if (string.IsNullOrEmpty(levelName))
		{
			Debug.LogError("Loading: Level name is null or empty!");
			yield break;
		}
		
		// Check if scene exists in build settings
		if (!IsSceneInBuildSettings(levelName))
		{
			Debug.LogError("Loading: Scene '{levelName}' not found in Build Settings!");
			yield break;
		}
		
		asyncOperation = SceneManager.LoadSceneAsync(levelName);
		
		if (asyncOperation == null)
		{
			Debug.LogError("Loading: Failed to load scene '{levelName}'!");
			yield break;
		}
		
		// Optional: Prevent scene from activating automatically
		// asyncOperation.allowSceneActivation = false;
		
		yield return asyncOperation;
	}
	
	private bool IsSceneInBuildSettings(string sceneName)
	{
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			
			if (name == sceneName)
				return true;
		}
		
		return false;
	}
	
	// Public utility methods
	public float GetLoadProgress()
	{
		return asyncOperation != null ? asyncOperation.progress : 0f;
	}
	
	public bool IsLoadingComplete()
	{
		return asyncOperation != null && asyncOperation.isDone;
	}
	
	public bool IsInitialized()
	{
		return isInitialized;
	}
}