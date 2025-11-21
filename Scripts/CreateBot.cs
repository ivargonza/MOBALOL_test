using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateBot : MonoBehaviour 
{
	public GameObject[] Bot;
	public Button[] Champ;
	public Toggle TeamBlue;
	public Toggle TeamRed;
	public int index { get; set; }
	public GameObject ChampSelected;


	void FixedUpdate () 
	{
		if (TeamRed.isOn) 
		{
			TeamBlue.isOn = false;
		}
		if (TeamBlue.isOn) 
		{
			TeamRed.isOn = false;
		}
		
		
	}
	public void Open()
	{
		gameObject.SetActive (true);
	}
	public void Close()
	{
		gameObject.SetActive (false);
	}

	public void Spawn()
	{
		if (TeamRed.isOn) 
		{
			Instantiate (ChampSelected, GameObject.Find ("Well Red").transform.position, GameObject.Find ("Well Red").transform.rotation);

		}
		else if (TeamBlue.isOn) 
		{
			Instantiate (ChampSelected, GameObject.Find ("Well Blue").transform.position, GameObject.Find ("Well Blue").transform.rotation);

		}
		gameObject.SetActive (false);

	}

	public void Button()
	{
		ChampSelected = Bot [index];
		if (TeamRed.isOn) 
		{
			ChampSelected.GetComponent<IABots>().EquipoRojo = true;
			ChampSelected.GetComponent<IABots>().Enemigo = 11;
			ChampSelected.tag = "TeamRed";
			ChampSelected.layer = 12;
		}
		if (TeamBlue.isOn) 
		{
			ChampSelected.GetComponent<IABots>().EquipoRojo = false;
			ChampSelected.GetComponent<IABots>().Enemigo = 12;
			ChampSelected.tag = "TeamBlue";
			ChampSelected.layer = 11;
		}

        ChampSelected.GetComponent<IABots>().Top = false;
        ChampSelected.GetComponent<IABots>().Mid = true;
        ChampSelected.GetComponent<IABots>().Bot = false;
       
		
	}
}
