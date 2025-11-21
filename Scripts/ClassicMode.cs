using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClassicMode : MonoBehaviour 
{
	public GameObject victory;
	public GameObject Defeat;

	public Character BlueNexus;
	public Character RedNexus;

	public MobaCam cam;
	public int teamchar;

	public int BlueScore;
	public int RedScore;
	public Text BlueScoreTXT;
	public Text BlueScoreTXT2;
	public Text RedScoreTXT;
	public Text RedScoreTXT2;

	// Use this for initialization
	void Start () 
	{
		teamchar = GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().team;
		cam = GameObject.Find ("FollowCam").GetComponent<MobaCam> ();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (teamchar == 0)
		{
			RedScoreTXT.text = RedScore.ToString();
			BlueScoreTXT.text = BlueScore.ToString();
			RedScoreTXT2.text = RedScore.ToString();
			BlueScoreTXT2.text = BlueScore.ToString();

		}
		else
		{
			RedScoreTXT.text = BlueScore.ToString();
			BlueScoreTXT.text = RedScore.ToString();
			RedScoreTXT2.text = BlueScore.ToString();
			BlueScoreTXT2.text = RedScore.ToString();

		}

		if(Application.loadedLevel == 2 || Application.loadedLevel == 3 || Application.loadedLevel == 5)
		{
			if(BlueNexus.Vida <=0)
			{
				cam.IsFree= false;
				cam.target = BlueNexus.gameObject.transform;
				if(teamchar == 0)
				{
					Defeat.SetActive(true);
					victory.SetActive(false);
				}
				else
				{
					Defeat.SetActive(false);
					victory.SetActive(true);
				}
				
			}
			if(RedNexus.Vida <=0)
			{
				cam.IsFree= false;
				cam.target = RedNexus.gameObject.transform;
				if(teamchar == 0)
				{
					Defeat.SetActive(false);
					victory.SetActive(true);
				}
				else
				{
					Defeat.SetActive(true);
					victory.SetActive(false);
				}
				
			}
		}


	}
}
