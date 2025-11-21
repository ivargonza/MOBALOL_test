using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	public AudioClip[] musics;
	public float intervalTime = 5f;
	public float lastPlayingTime;
	public int lastIndex;

	void Start() 
	{
		lastIndex = -1;
		lastPlayingTime = Time.time - 2 * intervalTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!gameObject.GetComponent<AudioSource>().isPlaying && (Time.time - lastPlayingTime) > intervalTime) 
		{
			int nextIndex = -1;
			do 
			{
				nextIndex = GetNextIndex();
			} 
			while(nextIndex == lastIndex);
			lastIndex = nextIndex;
			gameObject.GetComponent<AudioSource>().clip = musics[nextIndex];
			gameObject.GetComponent<AudioSource>().Play();
		}
	}
	
	private int GetNextIndex() 
	{
		return Random.Range(0, musics.Length);
	}
}