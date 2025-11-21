using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour 
{
	public Animator loginAnim;
	public ChampionSelection misc;
	public Text NombreDeUsuario;
	public Text PerfilNombre;
	public Button LoguinBut;

	public GameObject LoginScreen;
	public GameObject Home;
	public GameObject GameType;
	public GameObject PVP;
	public GameObject VSAI;
	public GameObject Practice;
	public GameObject Summon;
	public GameObject Prov;
	public GameObject Garden;
	public GameObject ProvPractice;
	public GameObject ChampSelection;
	public GameObject Profile;

	public AudioSource GateOpen;
	public AudioSource MenuSounds;
	public AudioClip LoginS;
	public AudioClip PlayS;
	public AudioClip ReturnS;
	// Use this for initialization
	void Start () 
	{
		if (PlayerPrefs.GetInt ("Loged") == 1)
		{
			Home.SetActive (true);
			LoginScreen.SetActive (false);
			PerfilNombre.text = PlayerPrefs.GetString("User","Summoner");
			GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().Nombre = PlayerPrefs.GetString("User","Summoner");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (NombreDeUsuario.text == "") 
		{
			LoguinBut.interactable = false;
		}
		else 
		{
			LoguinBut.interactable = true;
		}
		if (Input.GetKey (KeyCode.Return) && PlayerPrefs.GetInt ("Loged") == 0) 
		{
			Login();
		}
	
	}

	public void Login()
	{
		loginAnim.enabled = true;
		misc.Nombreinvocador = NombreDeUsuario;
		Home.SetActive (true);
		PerfilNombre.text = NombreDeUsuario.text;
		MenuSounds.clip = LoginS;
		MenuSounds.Play ();
		GateOpen.Play ();
		PlayerPrefs.SetInt ("Loged", 1);
		PlayerPrefs.SetString ("User", NombreDeUsuario.text);
		GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().Nombre = NombreDeUsuario.text;
	}
	public void Play()
	{
		Home.SetActive (false);
		GameType.SetActive (true);
		MenuSounds.clip = PlayS;
		MenuSounds.Play ();
		PVP.SetActive (true);
		VSAI.SetActive (true);
		Practice.SetActive (true);
		Summon.SetActive (false);
		Prov.SetActive (false);
		Garden.SetActive (false);
	}
	public void ReturnHome()
	{
		Home.SetActive (true);
		GameType.SetActive (false);
		MenuSounds.clip = ReturnS;
		MenuSounds.Play ();
		PVP.SetActive (true);
		VSAI.SetActive (true);
		Practice.SetActive (true);
		Summon.SetActive (false);
		Prov.SetActive (false);
		ProvPractice.SetActive (false);
		Garden.SetActive (false);
	}
	public void VsAI()
	{
		PVP.SetActive (false);
		VSAI.SetActive (false);
		Practice.SetActive (false);
		Summon.SetActive (true);
		Prov.SetActive (true);
		Garden.SetActive (true);
	}
	public void PraciceButt()
	{
		PVP.SetActive (false);
		VSAI.SetActive (false);
		Practice.SetActive (false);
		ProvPractice.SetActive (true);
	}
	public void Rift()
	{
        GameType.SetActive(false);
        ChampSelection.SetActive (true);
		Profile.SetActive (false);
		GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().NombreMapa = "Map1";
	}
	public void SoloMid()
	{
        GameType.SetActive(false);
        ChampSelection.SetActive (true);
		Profile.SetActive (false);
		GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().NombreMapa = "Map2";
	}
	public void SoloMidPract()
	{
        GameType.SetActive(false);
        ChampSelection.SetActive (true);
		Profile.SetActive (false);
		GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().NombreMapa = "Map2_Practice";
	}
	public void GardenMap()
	{
        GameType.SetActive(false);
        ChampSelection.SetActive (true);
		Profile.SetActive (false);
		GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().NombreMapa = "Map3";
	}
	public void CloseButton()
	{
		PlayerPrefs.SetInt ("Loged", 0);
		Application.Quit ();
	}
}
