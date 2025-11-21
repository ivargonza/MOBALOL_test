using UnityEngine;
using System.Collections;

public class ClickEfecto : MonoBehaviour 
{
	public float Timer = 0.25f;
	
	void OnEnable()  
	{
		StartCoroutine (desactivate ());
	}

	IEnumerator desactivate()
	{
		yield return new WaitForSeconds (Timer);
		gameObject.SetActive (false);
	}
}
