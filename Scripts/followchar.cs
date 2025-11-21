using UnityEngine;
using System.Collections;

public class followchar : MonoBehaviour 
{

	public Transform target;
	void Start () {
	
	}
	

	void Update () 
	{
		transform.position = new Vector3 (target.position.x, target.position.y, target.position.z);
	}
}
