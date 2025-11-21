using UnityEngine;
using System.Collections;

public class RenderMinimap : MonoBehaviour 
{

	public RenderTexture minimap;

	// Use this for initialization
	void Update () 
	{
		if (!minimap.IsCreated()) 
		{
			minimap.Create();
		}
	}
}
