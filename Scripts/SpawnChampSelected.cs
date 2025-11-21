using UnityEngine;
using System.Collections;

public class SpawnChampSelected : MonoBehaviour 
{
	public ChampContainer Things;
	public bool initialized = false;
	// Use this for initialization
	void OnEnable () 
	{
		if(!initialized)
		{
			Things = GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ();
			Things.SpawnChamp ();
			initialized = true;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{

	}
}
