using UnityEngine;
using System.Collections;

public class Autodisable : MonoBehaviour 
{
    public float timer = 0.2f;

	public void Diss () 
	{
		StartCoroutine (desactivate ());
	}
	
	IEnumerator desactivate()
	{
		yield return new WaitForSeconds (timer);
		gameObject.SetActive (false);
	}
}
