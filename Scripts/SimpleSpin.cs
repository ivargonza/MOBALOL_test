using UnityEngine;
using System.Collections;

public class SimpleSpin : MonoBehaviour 
{
	public float x;
	public float y;
	public float z;
	
	void Update () 
	{
		transform.Rotate(x,y,z);
	}
}
