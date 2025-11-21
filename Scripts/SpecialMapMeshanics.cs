using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpecialMapMeshanics : MonoBehaviour 
{
	public float ScoreBlue;
	public float ScorePurple;
	public float InitialTimer;

	public Image Blue;
	public Image Purple;
	public Text BlueText;
	public Text PurpleText;

	public GameObject victory;
	public GameObject Defeat;
	
	public Character BlueNexus;
	public Character RedNexus;

	public GameObject[] Blockers;
	public Animator[] StairsAnim;
	
	public MobaCam cam;
	public int teamchar;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine (BattlePrepare ());
		teamchar = GameObject.Find ("ChampContainer").GetComponent<ChampContainer> ().team;

		//cam = gameObject.GetComponent<MobaCam> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		BlueText.text = ScoreBlue.ToString ();
		PurpleText.text = ScorePurple.ToString ();
		float ImageScoreBlue = ((ScoreBlue * 100f) / 400f)/100f;
		Blue.fillAmount = ImageScoreBlue;
		float ImageScorePurple = ((ScorePurple * 100f) / 400f)/100f;
		Purple.fillAmount = ImageScorePurple;

		if (ScoreBlue == 0f && BlueNexus.IsAlive == true) 
		{
			ScoreBlue = 0f;
			BlueNexus.Vida =-1f;
			BlueNexus.IsAlive = false;
		}
		else if(ScorePurple == 0 && RedNexus.IsAlive == true)
		{
			ScorePurple = 0f;
			RedNexus.Vida =-1f;
			RedNexus.IsAlive = false;
		}
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

	IEnumerator BattlePrepare()
	{
		yield return new WaitForSeconds (39f);
		for (var i = 0; i < StairsAnim.Length; i++) 
		{
			StairsAnim[i].enabled = true;
		}
		yield return new WaitForSeconds (51f);
		for (var i = 0; i < Blockers.Length; i++) 
		{
			Blockers[i].SetActive(false);
		}

	}

}
