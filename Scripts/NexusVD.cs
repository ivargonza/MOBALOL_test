using UnityEngine;
using System.Collections;

public class NexusVD : MonoBehaviour 
{
	public GameObject NexusTurret1;
	public GameObject NexusTurret2;

	public bool specialMap;
	public GameObject NexusCanvas;
	
	// Use this for initialization
	void Start () 
	{
		if (!specialMap) 
		{
			if(NexusTurret1.GetComponent<Character>().Vida >0 && NexusTurret2.GetComponent<Character>().Vida >0)
			{
				gameObject.GetComponent<CapsuleCollider>().enabled = false;
				NexusCanvas.SetActive (false);
			}
		}
		else
		{
			gameObject.GetComponent<CapsuleCollider>().enabled = false;
			NexusCanvas.SetActive (false);
		}


	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!specialMap) 
		{
			if(NexusTurret1.GetComponent<Character>().Vida <=0 && NexusTurret2.GetComponent<Character>().Vida <=0 && gameObject.GetComponent<Character>().Vida > 0)
			{
				gameObject.GetComponent<CapsuleCollider>().enabled = true;
				NexusCanvas.SetActive (true);
			}
		}
	}
}
