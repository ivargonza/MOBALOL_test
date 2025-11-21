using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shop : MonoBehaviour 
{

	public GameObject ShopMenu;
	public LayerMask ShopLayer;

	public AudioClip Open;
	public AudioClip Close;
	public AudioClip Buy;
	public AudioClip Sell;
	public AudioSource Sounds;

	public bool openDia = false;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Mouse0)) 
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, 1000, ShopLayer))
			{
				Sounds.clip = Open;
				Sounds.Play();
				ShopMenu.SetActive(true);
				openDia = true;
			}
		}
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			ShopButtonInteract();
		}
	}

	public void ShopButtonInteract()
	{
		if(openDia)
		{
			Sounds.clip = Close;
			Sounds.Play();
			ShopMenu.SetActive(false);
			openDia = false;
			
		}
		else
		{
			Sounds.clip = Open;
			Sounds.Play();
			ShopMenu.SetActive(true);
			openDia = true;
		}
	}
	
}
