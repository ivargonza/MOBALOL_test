using UnityEngine;
using System.Collections;

public class Voice : MonoBehaviour 
{
	//movement
	public AudioClip[] VoiceMove;
	public AudioClip[] VoiceAttack;
	float VoiceTime = 10;
	

	void Update () 
	{
		VoiceTime += Time.deltaTime;
	}
	public void VoiceSounds()
	{
		if(VoiceTime >= Random.Range(10f,20f))
		{
			if (gameObject.GetComponent<AudioSource>().isPlaying)return;
			{
				gameObject.GetComponent<AudioSource>().clip = VoiceMove [Random.Range (0, VoiceMove.Length)];
				gameObject.GetComponent<AudioSource>().Play ();
				VoiceTime = 0;
			}
		}
	}
	public void AttackSounds()
	{
		if (VoiceTime >= Random.Range (5f, 10f)) 
		{
			if (gameObject.GetComponent<AudioSource> ().isPlaying)
				return;
			{
				gameObject.GetComponent<AudioSource> ().clip = VoiceAttack [Random.Range (0, VoiceAttack.Length)];
				gameObject.GetComponent<AudioSource> ().Play ();
				VoiceTime = 0;
			}
		}
	}
}
