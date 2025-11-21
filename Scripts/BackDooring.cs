using UnityEngine;
using System.Collections;

public class BackDooring : MonoBehaviour 
{
	public GameObject nextBuildingCanvas;
	public GameObject nextBuilding;
	public GameObject TurretNexus1;
	public GameObject TurretNexus2;
	public GameObject CanvasTurretNexus1;
	public GameObject CanvasTurretNexus2;
	public bool AllTurretsNexus;
	public bool Nexusturret;
	public int AmountNT;

	// Use this for initialization
	void Start () 
	{
		if(gameObject.GetComponent<TorretaRojaAI>().IsTurret)
		{
			nextBuilding.GetComponent<CapsuleCollider> ().enabled = false;
			nextBuildingCanvas.SetActive (false);
		}
		else if(!gameObject.GetComponent<TorretaRojaAI>().IsNexus &&!gameObject.GetComponent<TorretaRojaAI>().IsTurret)
		{
			TurretNexus1.GetComponent<CapsuleCollider> ().enabled = false;
			TurretNexus2.GetComponent<CapsuleCollider> ().enabled = false;
			CanvasTurretNexus1.SetActive (false);
			CanvasTurretNexus2.SetActive (false);
		}

	}

	public void Ondeath()
	{
		if(gameObject.GetComponent<TorretaRojaAI>().IsTurret)
		{
			nextBuilding.GetComponent<CapsuleCollider> ().enabled = true;
			nextBuildingCanvas.SetActive (true);
		}
		else if(!gameObject.GetComponent<TorretaRojaAI>().IsNexus &&!gameObject.GetComponent<TorretaRojaAI>().IsTurret)
		{
			if(TurretNexus1.GetComponent<Character>().Vida > 0 && TurretNexus2.GetComponent<Character>().Vida > 0)
			{
				TurretNexus1.GetComponent<CapsuleCollider> ().enabled = true;
				TurretNexus2.GetComponent<CapsuleCollider> ().enabled = true;
				CanvasTurretNexus1.SetActive (true);
				CanvasTurretNexus2.SetActive (true);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
