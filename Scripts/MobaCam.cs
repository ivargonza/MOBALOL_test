using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MobaCam : MonoBehaviour 
{

	public Transform target;
	public float CamSpeed;
	public float GUIsize;
	public bool IsFree;
	public Toggle LockCam;
	public float minX;
	public float maxX;
	public float minZ;
	public float maxz;

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space)&& !LockCam.isOn)
		{
			IsFree = false;
		}
		if(Input.GetKeyUp(KeyCode.Space)&& !LockCam.isOn)
		{
			IsFree = true;
		}
		if (IsFree ) 
		{
			Rect recdown = new Rect (0, 0, Screen.width, GUIsize);
			Rect recup = new Rect (0, Screen.height-GUIsize, Screen.width, GUIsize);
			Rect recleft = new Rect (0, 0, GUIsize, Screen.height);
			Rect recright = new Rect (Screen.width-GUIsize, 0, GUIsize, Screen.height);
			if (recdown.Contains(Input.mousePosition))
			{
				if(transform.position.z > minZ)
				{
					transform.Translate(0, 0, -CamSpeed, Space.World);
				}
			}
			if (recup.Contains(Input.mousePosition))
			{
				if(transform.position.z < maxz)
				{
					transform.Translate(0, 0, CamSpeed, Space.World);
				}
			}
			
			if (recleft.Contains(Input.mousePosition))
			{
				if(transform.position.x > minX)
				{
					transform.Translate(-CamSpeed, 0, 0, Space.World);
				}
			}
			
			if (recright.Contains(Input.mousePosition))
			{
				if(transform.position.x < maxX)
				{
					transform.Translate(CamSpeed, 0, 0, Space.World);
				}
			}
			
		}
		else 
		{
			transform.position = new Vector3 (target.position.x, transform.position.y, target.position.z);
		}	


	}
	public void ToggleInteract()
	{
		if(LockCam.isOn)
		{
			IsFree = false;
		}
		else
		{
			IsFree = true;
		}
	}

}
