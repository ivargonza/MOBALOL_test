using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateBotRandom : MonoBehaviour 
{
	public GameObject[] Bots;
	public GameObject bot;
	public float time;
	public int BotAmmount;
	public int linea;
	public bool EquipoAzul;
	public bool BotInicial;

	void Awake() 
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
			gameObject.GetComponent<CreateBotRandom>().enabled = false;
		}
		if (bot != null) 
		{
			if(linea == 1)
			{
				bot.GetComponent<MinionsAI>().Mid = true;
				bot.GetComponent<MinionsAI>().Top = false;
				bot.GetComponent<MinionsAI>().Bot = false;
			}
			if(linea == 2)
			{
				bot.GetComponent<MinionsAI>().Mid = false;
				bot.GetComponent<MinionsAI>().Top = true;
				bot.GetComponent<MinionsAI>().Bot = false;
			}
			if(linea == 3)
			{
				bot.GetComponent<MinionsAI>().Mid = false;
				bot.GetComponent<MinionsAI>().Top = false;
				bot.GetComponent<MinionsAI>().Bot = true;
			}
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
			linea = 1;
			time = 51;
		} 
	}

	void SpawnBot()
	{
		time = 0;
		BotAmmount -= 1;
		if (linea > 0) 
		{
			linea +=1;
		}
		if (linea > 3) 
		{
			linea =2;
		}

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
