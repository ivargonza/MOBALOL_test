using UnityEngine;
using System.Collections;

public class MisilGenericoSinTarget : MonoBehaviour 
{
	public float moveSpeed;
	public float Rango;
	public Transform MyOwner;

	public float damage;

	void Update () 
	{
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		if (Vector3.Distance (transform.position, MyOwner.position) > Rango) 
		{
			Destroy(this.gameObject);
		}

	}
}
