using UnityEngine;
using System.Collections;

public class CreateBotRandomAllMid : MonoBehaviour 
{
	public GameObject[] Bots;
	public GameObject bot;
	public float time;
	public int BotAmmount;
	public bool EquipoAzul;
	
	void Start () 
	{
		DetermineBotInicial ();
	}

	void Update () 
	{
		if (time > 0) 
		{
			time -= Time.deltaTime;
		}
		if (time < 0) 
		{
			SpawnBot();
			
		}
		if (BotAmmount > 0 && time == 0) 
		{
			DetermineBot();
			
		}
		if (BotAmmount < 1) 
		{
			gameObject.GetComponent<CreateBotRandomAllMid>().enabled = false;
		}
		if (bot != null) 
		{
			bot.GetComponent<MinionsAI>().Mid = true;
			bot.GetComponent<MinionsAI>().Top = false;
			bot.GetComponent<MinionsAI>().Bot = false;
		}
	
	}

	void DetermineBot()
	{
		bot = Bots [Random.Range(0,Bots.Length)];
		time = 0.5f;
	}
	
	void DetermineBotInicial()
	{
		if (Bots.Length > 0) 
		{
			bot = Bots [Random.Range(0,Bots.Length)];
			time = 100;
		} 
	}

	void SpawnBot()
	{
		time = 0;
		BotAmmount -= 1;
		if (!EquipoAzul) 
		{
			bot.GetComponent<MinionsAI>().EquipoRojo = true;
			Instantiate (bot, GameObject.Find ("Well Red").transform.position, GameObject.Find ("Well Red").transform.rotation);
		}
		else 
		{
			bot.GetComponent<MinionsAI>().EquipoRojo = false;
			Instantiate (bot, GameObject.Find ("Well Blue").transform.position, GameObject.Find ("Well Blue").transform.rotation);
		}
	}
}
