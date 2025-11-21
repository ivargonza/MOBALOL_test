using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class toggleCamLock : MonoBehaviour 
{

	public Toggle LockCam;
	public MobaCam free;
	
	void Update () 
	{
		if(LockCam.isOn)
		{
			free.IsFree = false;
		}
		if(!LockCam.isOn)
		{
			free.IsFree = true;
		}
	}
}
