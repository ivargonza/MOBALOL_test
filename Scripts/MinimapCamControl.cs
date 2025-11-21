using UnityEngine;
using System.Collections;

public class MinimapCamControl : MonoBehaviour 
{
	public GameObject MobaCamara;
	public Camera MinimapCamera;
	public LayerMask map;
	public bool mouseEntered { get; set; }


	void Update () 
	{
		if(mouseEntered)
		{
			if(Input.GetMouseButton(0))
			{
				RaycastHit hit;
				Ray ray = MinimapCamera.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray,out hit,Mathf.Infinity,map))
				{
					MobaCamara.transform.position = new Vector3(hit.point.x,MobaCamara.transform.position.y,hit.point.z);
				}
			}
		}
	}
}
