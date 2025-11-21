using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour 
{
	public Slider Quality;
	public Slider Volume;
    public Slider VolumeMusic;
    public Toggle Details;
	public Toggle Shadows;
	public Light ShadowsLight;
	public GameObject Menu;
	public bool openDia;
    public AudioSource Music;

	public GameObject[] MapObjects;

	void Start()
	{
        QualitySettings.SetQualityLevel(Mathf.RoundToInt(PlayerPrefs.GetFloat("Quality")));
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        Music.volume = PlayerPrefs.GetFloat("VolumeMusic");
    }

	void OnEnable()
	{
		QualitySettings.SetQualityLevel (Mathf.RoundToInt(PlayerPrefs.GetFloat("Quality")));
		AudioListener.volume = PlayerPrefs.GetFloat("Volume");
		Volume.value = PlayerPrefs.GetFloat("Volume");
        VolumeMusic.value = PlayerPrefs.GetFloat("VolumeMusic");
        Quality.value = PlayerPrefs.GetFloat("Quality");

		int DetailsValue = PlayerPrefs.GetInt ("Details");
		int ShadowsConf = PlayerPrefs.GetInt ("Shadows");

		Details.isOn = (DetailsValue == 1 ? true : false);
		Shadows.isOn = (ShadowsConf == 1 ? true : false);

		if(MapObjects.Length > 0)
		{
			for(int i = 0; i < MapObjects.Length; i++)
			{
				if(Details.isOn)
				{
					MapObjects[i].SetActive(true);
				}
				else
				{
					MapObjects[i].SetActive(false);
				}
			}
		}
		if(ShadowsLight != null)
		{
			if(Shadows.isOn)
			{
				ShadowsLight.shadows = LightShadows.Hard;
			}
			else
			{
				ShadowsLight.shadows = LightShadows.None;
			}
		}

	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(openDia)
			{
				CloseDialog();
				openDia = false;
			}
			else
			{
				OpenDialog();
				openDia = true;
			}
		}
	}
	
	public void OpenDialog()
	{
		Menu.SetActive (true);
		openDia = true;

	}
	public void CloseDialog()
	{
		Menu.SetActive (false);
		openDia = false;

	}
	public void AplySettingsSlider()
	{
		QualitySettings.SetQualityLevel (Mathf.RoundToInt(Quality.value));
		AudioListener.volume = Volume.value;
        Music.volume = VolumeMusic.value;
		if(MapObjects.Length > 0)
		{
			for(int i = 0; i < MapObjects.Length; i++)
			{
				if(Details.isOn)
				{
					MapObjects[i].SetActive(true);
				}
				else
				{
					MapObjects[i].SetActive(false);
				}
			}
		}
		if(ShadowsLight != null)
		{
			if(Shadows.isOn)
			{
				ShadowsLight.shadows = LightShadows.Hard;
			}
			else
			{
				ShadowsLight.shadows = LightShadows.None;
			}
		}
	}
	public void SaveSettings()
	{
		PlayerPrefs.SetFloat ("Quality", Quality.value);
		PlayerPrefs.SetFloat ("Volume", Volume.value);
        PlayerPrefs.SetFloat("VolumeMusic", VolumeMusic.value);
        PlayerPrefs.SetInt ("Details", (Details.isOn ? 1 : 0));
		PlayerPrefs.SetInt ("Shadows", (Shadows.isOn ? 1 : 0));
		Menu.SetActive (false);
		openDia = false;
		QualitySettings.SetQualityLevel (Mathf.RoundToInt(PlayerPrefs.GetFloat("Quality")));
		AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        Music.volume = PlayerPrefs.GetFloat("VolumeMusic");
        if (MapObjects.Length > 0)
		{
			for(int i = 0; i < MapObjects.Length; i++)
			{
				if(Details.isOn)
				{
					MapObjects[i].SetActive(true);
				}
				else
				{
					MapObjects[i].SetActive(false);
				}
			}
		}
		if(ShadowsLight != null)
		{
			if(Shadows.isOn)
			{
				ShadowsLight.shadows = LightShadows.Hard;
			}
			else
			{
				ShadowsLight.shadows = LightShadows.None;
			}
		}
	}
	public void Disconnect() 
	{
		Destroy (GameObject.Find ("ChampContainer"));
		Application.LoadLevel (0);

	}
}
