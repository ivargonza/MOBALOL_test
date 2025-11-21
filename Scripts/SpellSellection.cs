using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellSellection : MonoBehaviour 
{
	public GameObject Submenu;
	public int SpellIndex { get; set; }

	public int DIndex;
	public int FIndex;

	public Button[] SpellButton;
    public GameObject[] SpellPrefab;

    public Image ButtonD;
	public Image ButtonF;
	public Button Db;
	public Button Fb;

	public bool isD { get; set; }

	public List<int> SpellSelected = new List<int>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenDialog()
	{
		Submenu.SetActive (true);
		Fb.interactable = false;
		Db.interactable = false;

	}
	public void CloseDialog()
	{
		Submenu.SetActive (false);
		Fb.interactable = true;
		Db.interactable = true;

	}

	public void IndexSet()
	{
		for (var i = 0; i < SpellButton.Length; i++) 
		{
			if(i != SpellIndex)
			{
				SpellButton[i].interactable = true;
				if(isD)
				{
					DIndex = SpellIndex;
					Db.image.sprite = SpellButton [SpellIndex].image.sprite;
				}
				else
				{
					FIndex = SpellIndex;
					Fb.image.sprite = SpellButton [SpellIndex].image.sprite;
				}
			}
		}
		SpellButton[SpellIndex].interactable = false;

	}
}
