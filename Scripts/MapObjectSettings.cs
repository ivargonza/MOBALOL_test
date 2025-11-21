using UnityEngine;
using System.Collections;

public class MapObjectSettings : MonoBehaviour 
{
	MeshRenderer mesh;

	// Use this for initialization
	void OnEnable () 
	{
		mesh = GetComponent<MeshRenderer> ();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		int CurrentQLevel = QualitySettings.GetQualityLevel ();
		if(CurrentQLevel == 0 || CurrentQLevel == 1 )
		{
			if(mesh != null)
				mesh.enabled = false;
		}
		else
		{
			if(mesh != null)
				mesh.enabled = true;
		}
	}
}
