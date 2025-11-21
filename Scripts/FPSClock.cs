using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSClock : MonoBehaviour 
{
	int Minuto;
	float Segundo;

	float deltaTime = 0.0f;
	float msec;
	float fps;

	public Text clock;
	public Text Fps;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//clock
		Segundo += Time.deltaTime;
		if(Segundo > 59.9f)
		{
			Minuto += 1;
			Segundo = 0;
		}

		clock.text = Minuto.ToString() + " : " + Segundo.ToString ("0");

		//fps
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		msec = deltaTime * 1000.0f;
		fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		Fps.text = text;

	}
}
