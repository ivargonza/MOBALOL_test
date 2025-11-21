using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitanBar : MonoBehaviour 
{
	public Image[] BotsIcons;
	public Image[] BotsHealthBars;
	public Image[] RedteamBotsHealthBars;

	public GameObject[] Bots;
	public GameObject botBlue;

	public ChampContainer cont;
	// Use this for initialization
	void Start () 
	{
		cont = GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ();
		StartCoroutine (fillSpaces ());
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (Application.loadedLevel == 2 || Application.loadedLevel == 3|| Application.loadedLevel == 6)
		{
			if(Bots.Length >= 1)
			{
				for (var i = 0; i < Bots.Length; i++) 
				{
					if(Bots[i]!= null)
					{
						BotsHealthBars[i].fillAmount = ((Bots[i].GetComponent<Character>().Vida * 100f) / Bots[i].GetComponent<Character>().VidaMax)/100f;
						if(Bots[i].GetComponent<Character>().Vida <=0)
						{
							BotsIcons[i].color = Color.gray;
						}
						if(Bots[i].GetComponent<Character>().Vida >0)
						{
							BotsIcons[i].color = Color.white;
						}
					}
				}
			}
		}

	}
	IEnumerator fillSpaces()
	{
		yield return new WaitForSeconds(0.1f);
		Bots = new GameObject[4];
		botBlue.SetActive(false);
		
	}

}
