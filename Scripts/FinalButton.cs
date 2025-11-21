using UnityEngine;
using System.Collections;

public class FinalButton : MonoBehaviour 
{

	public void ContinueButton()
	{
		StartCoroutine (CloseMap ());
	}

	IEnumerator CloseMap()
	{
		Destroy (GameObject.Find ("ChampContainer"));
		yield return new WaitForSeconds (0.5f);
		Application.LoadLevel (0);
	}
}

